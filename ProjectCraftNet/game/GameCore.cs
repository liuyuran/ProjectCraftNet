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
using ModManager.network.handlers;
using ModManager.utils;
using SysInfo;
using static ModManager.game.localization.LocalizationManager;
using static ProjectCraftNet.Program;

namespace ProjectCraftNet.game;

public class GameCore(Config config)
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(GameCore));
    private bool _stopping;
    private float _tickPerSecond;

    public void Start()
    {
        var deltaTime = 0.05f;
        var millPerTick = 1000 / config.Core!.MaxTps;
        var tickPerSecond = 0;
        var baseMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var world = ModManager.state.CraftNet.Instance.World.World;
        // 开始监听网络事件
        PackHandlers.RegisterAllHandlers();
        RegistryCorePackEvent();
        EventBus.Subscribe<ChatEvent>(OnGameEventsOnChatEvent);
        EventBus.Subscribe<ArchiveEvent>(OnGameEventsOnArchiveEvent);
        var systems = new Group<float>(
            "core-system",
            new ChunkGenerateSystem(world),
            new ArchiveSystem(world)
        );
        systems.Initialize();
        Logger.LogInformation("{}", Localize(ModId, "startup.loading_init_chunk"));
        GeneratorInitPlayer(world);
        systems.BeforeUpdate(in deltaTime);
        systems.Update(in deltaTime);
        systems.AfterUpdate(in deltaTime);
        Logger.LogInformation("{}", Localize(ModId, "startup.done"));
        while (!_stopping)
        {
            var lastTickMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            systems.BeforeUpdate(in deltaTime);
            systems.Update(in deltaTime);
            systems.AfterUpdate(in deltaTime);
            Logger.LogDebug("{}", Localize(ModId, "Tick {0}ms", DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastTickMillis));
            while (UserManager.WaitToJoin.Count > 0)
            {
                var entity = world.Create(Archetypes.Player);
                var sockId = UserManager.WaitToJoin.Dequeue();
                var userInfo = UserManager.GetUserInfo(sockId);
                if (userInfo == null) continue;
                var position = userInfo.Position;
                var gameMod = userInfo.GameMode;
                var chunkSize = config.Core!.ChunkSize;
                world.Set(entity, new Position
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
                world.Set(entity, new Player { UserId = userInfo.UserId, GameMode = gameMod, IsSystem = false});
                userInfo.PlayerEntity = entity;
            }

            while (UserManager.WaitToLeave.Count > 0)
            {
                var entity = UserManager.WaitToLeave.Dequeue();
                world.Destroy(entity);
            }

            // 调节逻辑帧率，等待下一个Tick
            var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var elapsed = now - lastTickMillis;
            if (elapsed < millPerTick)
            {
                Thread.Sleep((int)(millPerTick - elapsed));
            }

            tickPerSecond++;
            if (now - baseMillis <= 1000) continue;
            _tickPerSecond = tickPerSecond;
            tickPerSecond = 0;
            baseMillis = now;
        }

        systems.Dispose();
        Logger.LogInformation("{}", Localize(ModId, "Server shutdown"));
        Environment.Exit(0);
    }

    private static void GeneratorInitPlayer(World world)
    {
        var entity = world.Create(Archetypes.Player);
        world.Set(entity, new Position
        {
            ChunkPos = new IntVector3(0, 0, 0),
            InChunkPos = new Vector3(0, 0, 0)
        });
        world.Set(entity, new Player { UserId = -1, GameMode = GameMode.Creative, IsSystem = true });
    }

    private static bool OnGameEventsOnArchiveEvent(ArchiveEvent @event)
    {
        var world = ModManager.state.CraftNet.Instance.World.World;
        ArchiveManager.SaveUserInfo(world);
        var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
        var existChunkPosition = new Dictionary<long, List<IntVector3>>();
        world.Query(in chunkQuery, (ref ChunkBlockData data, ref Position position) =>
        {
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
            ArchiveManager.SaveChunkInfo(world, worldId, chunkPos.ToArray());
        }

        return true;
    }

    private static bool OnGameEventsOnChatEvent(ChatEvent @event)
    {
        var socketId = @event.SocketId;
        var message = @event.Message;
        if (string.IsNullOrWhiteSpace(message)) return false;
        var userInfo = UserManager.GetUserInfo(socketId);
        if (userInfo == null) return false;
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

    private void RegistryCorePackEvent()
    {
        NetworkPackBus.Subscribe(PackType.ServerStatus, (info, _) =>
        {
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
                Ping = UserManager.GetUserInfo(info.SocketId)?.ClientInfo.Ping ?? 0
            };
            NetworkEvents.FireSendEvent(info.SocketId, PackType.ServerStatus, status.ToByteArray());
        });
        NetworkPackBus.Subscribe(PackType.Shutdown, (_, _) =>
        {
            Logger.LogInformation("{}", Localize(ModId, "Server shutting down"));
            _stopping = true;
        });
    }
}