using Arch.Core;
using Arch.System;

namespace ProjectCraftNet.game.systems;

public class NetworkSyncSystem(World world) : BaseSystem<World, float>(world)
{
    private readonly World _world = world;

    public override void Update(in float deltaTime) {}
}