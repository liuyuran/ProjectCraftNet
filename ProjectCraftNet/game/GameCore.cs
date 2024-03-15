#define PURE_ECS
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using Arch.Core;
using Arch.System;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.client;
using ModManager.command;
using ModManager.config;
using ModManager.events;
using ModManager.logger;
using ModManager.network;
using ModManager.user;
using ProjectCraftNet.game.archive;
using ProjectCraftNet.game.components;
using ProjectCraftNet.game.systems;
using SysInfo;
using static ModManager.localization.LocalizationManager;
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
        GameEvents.ChatEvent += OnGameEventsOnChatEvent;
        GameEvents.ArchiveEvent += OnGameEventsOnArchiveEvent;
        var systems = new Group<float>(
            "core-system",
            new ChunkGenerateSystem(_world),
            new NetworkSyncSystem(_world),
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
                if (userInfo == null) continue;
                var position = userInfo.Value.Position;
                var gameMod = userInfo.Value.GameMode;
                _world.Set(entity, new Position { Val = position });
                _world.Set(entity, new Player { UserId = userInfo?.UserId ?? 0, GameMode = gameMod });
                
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

    private void OnGameEventsOnArchiveEvent()
    {
        ArchiveManager.SaveUserInfo(_world);
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var existChunkPosition = new Dictionary<long, List<Vector3>>();
        _world.Query(in chunkQuery, (ref ChunkBlockData data, ref Position position) => {
            if (!data.Changed) return;
            data.Changed = false;
            var worldId = data.WorldId;
            if (!existChunkPosition.TryGetValue(worldId, out var value))
            {
                value = [];
                existChunkPosition[worldId] = value;
            }

            value.Add(position.Val);
        });
        foreach (var (worldId, chunkPos) in existChunkPosition)
        {
            ArchiveManager.SaveChunkInfo(_world, worldId, chunkPos.ToArray());
        }
    }

    private static void OnGameEventsOnChatEvent(long socketId, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;
        var userInfo = UserManager.GetUserInfo(socketId);
        if (userInfo == null) return;
        var isCommand = CommandManager.TryParseAsCommand((UserInfo)userInfo, message);
        if (isCommand) return;
        var data = new ChatAndBroadcast { Msg = message };
        var buffer = data.ToByteArray();
        if (buffer == null)
        {
            Logger.LogError("{}", Localize(ModId, "Error when serializing message"));
            return;
        }

        NetworkEvents.FireSendEvent(socketId, PackType.Chat, buffer);
    }

    private void OnNetworkEventsOnReceiveEvent(ClientInfo info, PackType packType, byte[] data)
    {
        if (_stopping) return;
        switch (packType)
        {
            case PackType.Shutdown:
                Logger.LogInformation("{}", Localize(ModId, "Server shutting down"));
                _stopping = true;
                break;
            case PackType.Connect:
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
                var userInfo = UserManager.GetUserInfo(info.SocketId);
                GameEvents.FireUserLoginEvent(info.SocketId, (UserInfo) userInfo!);
                break;
            case PackType.Disconnect:
                GameEvents.FireUserLogoutEvent(info.SocketId, (UserInfo) UserManager.GetUserInfo(info.SocketId)!);
                UserManager.UserLogout(info.SocketId);
                break;
            case PackType.Chat:
                var chat = ChatAndBroadcast.Parser.ParseFrom(data);
                if (UserManager.GetUserInfo(info.SocketId) == null)
                {
                    Logger.LogDebug("{}", Localize(ModId, "chat from unauthenticated user {0}", info.Ip));
                    return;
                }
                Logger.LogInformation("{}", Localize(ModId, "Chat from {0}: {1}", info.Ip, chat.Msg));
                GameEvents.FireChatEvent(info.SocketId, chat.Msg);
                break;
            case PackType.ServerStatus:
                var currentProcess = Process.GetCurrentProcess();
                var status = new ServerStatus
                {
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
                    Name = config.Core!.Title,
                    MemoryUsed = (ulong)currentProcess.WorkingSet64,
                    MemoryTotal = Memory.TotalMemorySize * 1073741824UL,
                    MaxPlayers = config.Core!.MaxPlayer,
                    OnlinePlayers = UserManager.GetOnlineUserCount(),
                    Tps = (long)Math.Floor(_tickPerSecond)
                };
                NetworkEvents.FireSendEvent(info.SocketId, PackType.ServerStatus, status.ToByteArray());
                break;
            case PackType.ControlBlock:
                break;
            case PackType.ControlEntity:
                break;
            case PackType.Move:
                break;
            case PackType.OnlineList:
                break;
            case PackType.Chunk:
            case PackType.Ping:
            case PackType.Unknown:
            default:
                Logger.LogError("{}", Localize(ModId, "Unknown PackType: {0}", packType));
                break;
        }
    }
}