#define PURE_ECS
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using Arch.Core;
using Arch.System;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.archive;
using ModManager.config;
using ModManager.ecs.components;
using ModManager.ecs.systems;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.game.client;
using ModManager.game.command;
using ModManager.game.user;
using ModManager.logger;
using ModManager.network;
using ModManager.state.world.chunk;
using ModManager.utils;
using SysInfo;
using static ModManager.game.localization.LocalizationManager;
using static ProjectCraftNet.Program;

namespace ProjectCraftNet.game;

public class GameCore(Config config)
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(GameCore));
    private bool _stopping;
    private readonly World _world = World.Create();
    private float _tickPerSecond;

    public void Start()
    {
        var deltaTime = 0.05f;
        var millPerTick = 10000000 / config.Core!.MaxTps;
        var tickPerSecond = 0;
        var baseMillis = DateTime.Now.Ticks;

        // 开始监听网络事件
        NetworkEvents.ReceiveEvent += OnNetworkEventsOnReceiveEvent;
        EventBus.Subscribe<ChatEvent>(OnGameEventsOnChatEvent);
        EventBus.Subscribe<ArchiveEvent>(OnGameEventsOnArchiveEvent);
        var systems = new Group<float>(
            "core-system",
            new ChunkGenerateSystem(_world),
            new ArchiveSystem(_world)
        );
        systems.Initialize();
        Logger.LogInformation("{}", Localize(ModId, "Server started"));
        while (!_stopping)
        {
            var lastTickMillis = DateTime.Now.Ticks;
            systems.BeforeUpdate(in deltaTime);
            systems.Update(in deltaTime);
            systems.AfterUpdate(in deltaTime);
            while (UserManager.WaitToJoin.Count > 0)
            {
                var entity = _world.Create(Archetypes.Player);
                var sockId = UserManager.WaitToJoin.Dequeue();
                var userInfo = UserManager.GetUserInfo(sockId);
                var position = userInfo.Position;
                var gameMod = userInfo.GameMode;
                var chunkSize = config.Core!.ChunkSize;
                _world.Set(entity, new Position
                {
                    ChunkPos = new IntVector3(
                        (int)(position.X / chunkSize),
                        (int)(position.Y / chunkSize),
                        (int)(position.Z / chunkSize)
                    ),
                    InChunkPos = new Vector3(
                        position.X % chunkSize,
                        position.Y % chunkSize,
                        position.Z % chunkSize
                    )
                });
                _world.Set(entity, new Player { UserId = userInfo.UserId, GameMode = gameMod });
                userInfo.PlayerEntity = entity;
            }
            while (UserManager.WaitToLeave.Count > 0)
            {
                var entity = UserManager.WaitToLeave.Dequeue();
                _world.Destroy(entity);
            }
            // 调节逻辑帧率，等待下一个Tick
            var now = DateTime.Now.Ticks;
            var elapsed = now - lastTickMillis;
            if (elapsed < millPerTick)
            {
                Thread.Sleep((int) (millPerTick - elapsed) / 10000);
            }
            tickPerSecond++;
            if (now - baseMillis <= 10000000) continue;
            _tickPerSecond = tickPerSecond;
            tickPerSecond = 0;
            baseMillis = now;
        }
        systems.Dispose();
        Logger.LogInformation("{}", Localize(ModId, "Server shutdown"));
        Environment.Exit(0);
    }

    private bool OnGameEventsOnArchiveEvent(ArchiveEvent @event)
    {
        ArchiveManager.SaveUserInfo(_world);
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var existChunkPosition = new Dictionary<long, List<IntVector3>>();
        _world.Query(in chunkQuery, (ref ChunkBlockData data, ref Position position) => {
            if (!data.Changed) return;
            data.Changed = false;
            var worldId = data.WorldId;
            if (!existChunkPosition.TryGetValue(worldId, out var value))
            {
                value = [];
                existChunkPosition[worldId] = value;
            }

            value.Add(position.ChunkPos);
        });
        foreach (var (worldId, chunkPos) in existChunkPosition)
        {
            ArchiveManager.SaveChunkInfo(_world, worldId, chunkPos.ToArray());
        }

        return true;
    }

    private static bool OnGameEventsOnChatEvent(ChatEvent @event)
    {
        var socketId = @event.SocketId;
        var message = @event.Message;
        if (string.IsNullOrWhiteSpace(message)) return false;
        var userInfo = UserManager.GetUserInfo(socketId);
        var isCommand = CommandManager.TryParseAsCommand(userInfo, message);
        if (isCommand) return false;
        var data = new ChatAndBroadcast { Msg = message };
        var buffer = data.ToByteArray();
        if (buffer == null)
        {
            Logger.LogError("{}", Localize(ModId, "Error when serializing message"));
            return false;
        }

        NetworkEvents.FireSendEvent(socketId, PackType.Chat, buffer);
        return true;
    }

    private void OnNetworkEventsOnReceiveEvent(ClientInfo info, PackType packType, byte[] data)
    {
        if (_stopping) return;
        switch (packType)
        {
            case PackType.Shutdown:
                // 关闭服务器
                Logger.LogInformation("{}", Localize(ModId, "Server shutting down"));
                _stopping = true;
                break;
            case PackType.Connect:
                // 用户连接
                var connect = Connect.Parser.ParseFrom(data);
                var clientType = (ClientType)connect.ClientType;
                Logger.LogInformation("{}", Localize(ModId, "Client [{0}]{1} connected", clientType, info.Ip));
                var id = UserManager.UserLogin(connect, info);
                if (id == 0)
                {
                    // 登录失败，通知客户端关闭连接
                    NetworkEvents.FireSendEvent(info.SocketId, PackType.Shutdown, Array.Empty<byte>());
                    return;
                }
                NetworkEvents.FireSendEvent(info.SocketId, PackType.Connect, Array.Empty<byte>());
                EventBus.Trigger(info.SocketId, new UserLoginEvent());
                break;
            case PackType.Disconnect:
                // 断开连接
                EventBus.Trigger(info.SocketId, new UserLogoutEvent());
                UserManager.UserLogout(info.SocketId);
                break;
            case PackType.Chat:
                // 聊天消息
                var chat = ChatAndBroadcast.Parser.ParseFrom(data);
                Logger.LogInformation("{}", Localize(ModId, "Chat from {0}: {1}", info.Ip, chat.Msg));
                EventBus.Trigger(info.SocketId, new ChatEvent
                {
                    Message = chat.Msg
                });
                break;
            case PackType.ServerStatus:
                // 发送服务器状态
                var currentProcess = Process.GetCurrentProcess();
                var status = new ServerStatus
                {
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
                    Name = config.Core!.Title,
                    MemoryUsed = (ulong)currentProcess.WorkingSet64,
                    MemoryTotal = Memory.TotalMemorySize * 1073741824UL,
                    MaxPlayers = config.Core!.MaxPlayer,
                    OnlinePlayers = UserManager.GetOnlineUserCount(),
                    Tps = (long)Math.Floor(_tickPerSecond),
                    Ping = UserManager.GetUserInfo(info.SocketId).ClientInfo.Ping
                };
                NetworkEvents.FireSendEvent(info.SocketId, PackType.ServerStatus, status.ToByteArray());
                break;
            case PackType.ControlBlock:
                // TODO 方块交互
                break;
            case PackType.ControlEntity:
                // TODO 实体交互
                break;
            case PackType.Move:
                // 用户移动
                var move = PlayerMove.Parser.ParseFrom(data);
                var userInfo = UserManager.GetUserInfo(info.SocketId);
                userInfo.Position = new LongVector3((long)move.X, (long)move.Y, (long)move.Z);
                var entity = userInfo.PlayerEntity;
                if (entity == null) break;
                var chunkSize = config.Core!.ChunkSize;
                _world.Set(entity.Value, new Position
                {
                    ChunkPos = new IntVector3(
                        (int)(userInfo.Position.X / chunkSize),
                        (int)(userInfo.Position.Y / chunkSize),
                        (int)(userInfo.Position.Z / chunkSize)
                    ),
                    InChunkPos = new Vector3(
                        userInfo.Position.X % chunkSize,
                        userInfo.Position.Y % chunkSize,
                        userInfo.Position.Z % chunkSize
                    )
                });
                break;
            case PackType.OnlineList:
                // 发送在线用户列表
                var onlineList = new OnlineList();
                foreach (var user in UserManager.GetOnlineUsers())
                {
                    onlineList.Players.Add(new PlayerItem
                    {
                        Id = user.UserId,
                        Name = user.NickName,
                        Ping = user.ClientInfo.Ping
                    });
                }
                NetworkEvents.FireSendEvent(info.SocketId, PackType.OnlineList, onlineList.ToByteArray());
                break;
            case PackType.Chunk:
                // 发送区块数据
                var chunk = ChunkData.Parser.ParseFrom(data);
                var chunkData = ModManager.state.ProjectCraftNet.Instance.World.GetChunkData(chunk.WorldId, new ChunkPos
                {
                    X = chunk.X,
                    Y = chunk.Y,
                    Z = chunk.Z
                });
                foreach (var blockId in chunkData)
                {
                    chunk.Blocks.Add(new BlockData
                    {
                        BlockId = blockId,
                        SubId = 0
                    });
                }
                NetworkEvents.FireSendEvent(info.SocketId, PackType.OnlineList, chunk.ToByteArray());
                break;
            case PackType.Ping:
            case PackType.Unknown:
            default:
                Logger.LogError("{}", Localize(ModId, "Unknown PackType: {0}", packType));
                break;
        }
    }
}