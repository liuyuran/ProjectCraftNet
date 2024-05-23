using System.Numerics;
using Arch.Core;
using Arch.System;
using Microsoft.Extensions.Logging;
using ModManager.config;
using ModManager.ecs.components;
using ModManager.game.user;
using ModManager.logger;
using ModManager.network;
using ModManager.utils;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.ecs.systems;

public class UserJoinSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly World _world = world;
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(UserJoinSystem));

    public override void Update(in float deltaTime)
    {
        var config = ConfigUtil.Instance.GetConfig();
        while (UserManager.WaitToJoin.Count > 0)
        {
            var entity = _world.Create(Archetypes.Player);
            var sockId = UserManager.WaitToJoin.Dequeue();
            var userInfo = UserManager.GetUserInfo(sockId);
            if (userInfo == null) continue;
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
            _world.Set(entity, new Player { UserId = userInfo.UserId, GameMode = gameMod, IsSystem = false });
            userInfo.PlayerEntity = entity;
            NetworkEvents.FireSendEvent(userInfo.ClientInfo.SocketId, PackType.ConnectPack, []);
            Logger.LogInformation("{}", Localize(ModId, "user {0} join game.", userInfo.UserId));
        }

        while (UserManager.WaitToLeave.Count > 0)
        {
            var entity = UserManager.WaitToLeave.Dequeue();
            _world.Destroy(entity);
        }
    }
}