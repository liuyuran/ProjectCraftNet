using System.Numerics;
using Arch.Core;
using Arch.System;
using ProjectCraftNet.game.components;

namespace ProjectCraftNet.game.systems;

public class ChunkGenerateSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly World _world = world;

    public override void Initialize()
    {
        var entity = _world.Create(Archetypes.Chunk);
        _world.Set(entity, new Position {Val = new Vector3(0, 0, 0)});
        _world.Set(entity, new ChunkBlockData());
    }

    public override void BeforeUpdate(in float deltaTime)
    {
        // throw new NotImplementedException();
    }

    public override void Update(in float deltaTime)
    {
        var query = new QueryDescription().WithAll<Player, Position>();
        _world.Query(in query, (ref Player player, ref Position position) => {
            // TODO 生成附近区块
        });
    }

    public override void AfterUpdate(in float deltaTime)
    {
        // throw new NotImplementedException();
    }
}