using System.Numerics;
using Arch.Core;
using CoreMod.blocks;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using ModManager.ecs.components;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.game.block;
using ModManager.game.client;
using ModManager.game.user;
using ModManager.logger;
using ModManager.state;
using ModManager.state.world;
using ModManager.state.world.block;
using ModManager.state.world.chunk;
using ModManager.utils;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.network.handlers;

public partial class PackHandlers
{
    private static ILogger Logger { get; } = SysLogger.GetLogger(typeof(PackHandlers));

    public static void RegisterAllHandlers()
    {
        NetworkPackBus.Subscribe(PackType.Connect, ConnectHandler);
        NetworkPackBus.Subscribe(PackType.Disconnect, DisconnectHandler);
        NetworkPackBus.Subscribe(PackType.OnlineList, (info, _) =>
        {
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
        });
        NetworkPackBus.Subscribe(PackType.Move, (info, data) =>
        {
            // 用户移动
            var world = ProjectCraftNet.Instance.World.World;
            var move = PlayerMove.Parser.ParseFrom(data);
            var userInfo = UserManager.GetUserInfo(info.SocketId);
            if (userInfo == null) return;
            userInfo.Position = new LongVector3((long)move.X, (long)move.Y, (long)move.Z);
            var entity = userInfo.PlayerEntity;
            if (entity == null) return;
            world.Set(entity.Value, new Position
            {
                ChunkPos = new IntVector3(
                    move.ChunkX,
                    move.ChunkY,
                    move.ChunkZ
                ),
                InChunkPos = new Vector3(
                    move.X,
                    move.Y,
                    move.Z
                )
            });
            NetworkEvents.FireSendEvent(0, PackType.Move, data);
        });
        NetworkPackBus.Subscribe(PackType.Chunk, (info, data) =>
        {
            // 发送区块数据
            var chunk = ChunkData.Parser.ParseFrom(data);
            var chunkData = ProjectCraftNet.Instance.World.GetChunkData(chunk.WorldId, new ChunkPos
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

            NetworkEvents.FireSendEvent(info.SocketId, PackType.Chunk, chunk.ToByteArray());
        });
        NetworkPackBus.Subscribe(PackType.Chat, (info, data) =>
        {
            // 聊天消息
            var chat = ChatAndBroadcast.Parser.ParseFrom(data);
            Logger.LogInformation("{}", Localize(ModId, "Chat from {0}: {1}", info.Ip, chat.Msg));
            EventBus.Trigger(info.SocketId, new ChatEvent
            {
                Message = chat.Msg
            });
        });
        NetworkPackBus.Subscribe(PackType.ControlBlock, (info, data) =>
        {
            var world = ProjectCraftNet.Instance.World.World;
            var userInfo = UserManager.GetUserInfo(info.SocketId);
            if (userInfo == null) return;
            var controlBlock = PlayerControlBlock.Parser.ParseFrom(data);
            switch (controlBlock.Type)
            {
                case 1:
                {
                    // 挖掘
                    var chunkPos = new ChunkPos(controlBlock.ChunkX, controlBlock.ChunkY, controlBlock.ChunkZ);
                    var blockPos = new BlockPos(controlBlock.BlockX, controlBlock.BlockY, controlBlock.BlockZ);
                    ProjectCraftNet.Instance.World.SetBlockToChunk(userInfo.WorldId,
                        chunkPos, blockPos,
                        BlockManager.GetBlockId<Air>());
                    var chunkQuery = new QueryDescription().WithAll<ChunkBlockData, Position>();
                    world.Query(in chunkQuery, (ref ChunkBlockData chunkData, ref Position position) =>
                    {
                        if (position.ChunkPos.X != chunkPos.X || position.ChunkPos.Y != chunkPos.Y ||
                            position.ChunkPos.Z != chunkPos.Z) return;
                        chunkData.Changed = true;
                        chunkData.Data[GameWorld.GetIndexFromBlockPos(blockPos)] = BlockManager.GetBlockId<Air>();
                        var blockChange = new BlockChange
                        {
                            ChunkX = chunkPos.X,
                            ChunkY = chunkPos.Y,
                            ChunkZ = chunkPos.Z,
                            BlockX = blockPos.X,
                            BlockY = blockPos.Y,
                            BlockZ = blockPos.Z,
                            ChangeType = 0,
                            BlockId = BlockManager.GetBlockId<Air>(),
                            SubId = 0
                        };
                        NetworkEvents.FireSendEvent(0, PackType.BlockChange, blockChange.ToByteArray());
                    });
                    break;
                }
            }
        });
        NetworkPackBus.Subscribe(PackType.ControlEntity, (info, data) =>
        {
            //
        });
    }
}