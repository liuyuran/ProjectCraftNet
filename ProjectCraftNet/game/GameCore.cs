#define PURE_ECS
using Arch.Core;
using Arch.System;
using Microsoft.Extensions.Logging;
using ModManager;
using ModManager.client;
using ModManager.logger;
using ModManager.network;
using ModManager.user;
using ProjectCraftNet.game.systems;
using ProjectCraftNet.game.user;
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
        // for (var index = 0; index < 1000; index++) 
        //     world.Create(new Position{ X = 0, Y = 0}, new Velocity{ Dx = 1, Dy = 1});
        //
        // // Query and modify entities ( There are also alternatives without lambdas ;) ) 
        // var query = new QueryDescription().WithAll<Position,Velocity>(); // Targets entities with Position AND Velocity.
        // world.Query(in query, (ref Position pos, ref Velocity vel) => {
        //     pos.X += vel.Dx;
        //     pos.Y += vel.Dy;
        // });
        var systems = new Group<float>(
            "core-system",
            new ChunkGenerateSystem(_world)
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
        // TODO 保存游戏
        systems.Dispose();
        Logger.LogInformation("{}", Localize(ModId, "Server shutdown"));
        Environment.Exit(0);
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
                break;
            case PackType.Chat:
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