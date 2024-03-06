#define PURE_ECS
using System.Numerics;
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
using static ModManager.localization.LocalizationManager;
using static ProjectCraftNet.Program;

namespace ProjectCraftNet.game;

public class GameCore(Config config)
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(GameCore));
    private bool _stopping;
    private readonly World _world = World.Create();

    public void Start()
    {
        var deltaTime = 0.05f;
        var millPerTick = 1000 / config.Core!.MaxTps;

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
            // 调节逻辑帧率，等待下一个Tick
            var now = DateTime.Now.Ticks;
            var elapsed = now - lastTickMillis;
            if (elapsed < millPerTick)
            {
                Thread.Sleep((int) (millPerTick - elapsed));
            }
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
            case PackType.Chunk:
                break;
            case PackType.ControlBlock:
                break;
            case PackType.ControlEntity:
                break;
            case PackType.Move:
                break;
            case PackType.ServerStatus:
                break;
            case PackType.OnlineList:
                break;
            case PackType.Ping:
            case PackType.Unknown:
            default:
                Logger.LogError("{}", Localize(ModId, "Unknown PackType: {0}", packType));
                break;
        }
    }
}