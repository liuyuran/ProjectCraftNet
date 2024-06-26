﻿using Arch.Core;
using Arch.System;
using Microsoft.Extensions.Logging;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.logger;

namespace ModManager.ecs.systems;

public class ArchiveSystem(World world) : BaseSystem<World, float>(world)
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(ArchiveSystem));
    private const long ArchiveInterval = 1000L * 60 * 1;
    private readonly World _world = world;
    private long _lastArchiveTime;

    public override void Update(in float deltaTime)
    {
        if (_lastArchiveTime + ArchiveInterval > DateTimeOffset.Now.ToUnixTimeMilliseconds()) return;
        EventBus.Trigger(new ArchiveEvent());
        _lastArchiveTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}