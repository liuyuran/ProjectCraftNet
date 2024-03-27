using Microsoft.Extensions.Logging;
using ModManager.logger;
using ModManager.network;
using static ModManager.game.localization.LocalizationManager;
using static ModManager.mod.ModManager;

namespace ModManager.eventBus;

public delegate void PackHandler(ClientInfo info, byte[] data);

/// <summary>
/// 使用类似于事件总线的方式处理网络消息
/// </summary>
public class NetworkPackBus
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(NetworkPackBus));
    private readonly Dictionary<uint, List<Delegate>> _eventHandlers = new();
    private static NetworkPackBus Instance { get; } = new();

    // 注册事件处理器
    public static void Subscribe(uint type, PackHandler handler)
    {
        // 确保事件类型已注册
        if (!Instance._eventHandlers.TryGetValue(type, out var handlers))
        {
            handlers = new List<Delegate>();
            Instance._eventHandlers[type] = handlers;
        }

        Logger.LogDebug("{}", Localize(ModId, "Pack[%s] handler registered", type));
        // 添加处理器到列表
        handlers.Add(handler);
    }

    // 触发事件
    public static void Trigger(uint type, ClientInfo info, byte[] data)
    {
        // 尝试获取对应类型的处理器列表
        if (!Instance._eventHandlers.TryGetValue(type, out var handlers)) return;
        // 遍历处理器列表并调用它们
        foreach (var unused in handlers.Where(handler => !(bool)(handler.DynamicInvoke(info, data) ?? false)))
        {
            break;
        }
    }
}