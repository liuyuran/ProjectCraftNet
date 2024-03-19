using Arch.Core;
using Arch.System;
using Microsoft.Extensions.Logging;
using ModManager.events;
using ModManager.logger;

namespace ModManager.ecs.systems;

public class ArchiveSystem(World world) : BaseSystem<World, float>(world)
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(ArchiveSystem));
    private const long ArchiveInterval = 1000 * 60 * 5;
    private readonly World _world = world;
    private long _lastArchiveTime;

    public override void Update(in float deltaTime)
    {
        if (_lastArchiveTime + ArchiveInterval > DateTime.Now.Ticks) return;
        GameEvents.FireArchiveEvent();
        _lastArchiveTime = DateTime.Now.Ticks;
    }
}