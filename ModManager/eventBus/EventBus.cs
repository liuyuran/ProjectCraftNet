using ModManager.game.user;

namespace ModManager.eventBus;

public delegate bool EventHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;

// 事件参数基类，用于添加公用属性和方法，不可单独使用
public abstract class BasicEventArgs : EventArgs
{
    // 可以根据需要添加属性和方法
    public long SocketId;
    public UserInfo? UserInfo;
}

// 事件总线类
public class EventBus
{
    // 字典用于存储不同类型的事件和它们的处理器列表
    private readonly Dictionary<Type, List<Delegate>> _eventHandlers = new();
    private static EventBus Instance { get; } = new();

    // 注册事件处理器
    public static void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler) where TEventArgs : BasicEventArgs
    {
        // 确保事件类型已注册
        if (!Instance._eventHandlers.TryGetValue(typeof(TEventArgs), out var handlers))
        {
            handlers = new List<Delegate>();
            Instance._eventHandlers[typeof(TEventArgs)] = handlers;
        }

        // 添加处理器到列表
        handlers.Add(handler);
    }

    // 触发事件
    public static void Trigger<TEventArgs>(long socketId, TEventArgs args) where TEventArgs : BasicEventArgs
    {
        // 尝试获取对应类型的处理器列表
        if (!Instance._eventHandlers.TryGetValue(typeof(TEventArgs), out var handlers)) return;
        // 遍历处理器列表并调用它们
        var userInfo = UserManager.GetUserInfo(socketId);
        args.SocketId = socketId;
        args.UserInfo = userInfo;
        foreach (var unused in handlers.Where(handler => !(bool)(handler.DynamicInvoke(args) ?? false)))
        {
            break;
        }
    }
    
    // 触发事件
    public static void Trigger<TEventArgs>(TEventArgs args) where TEventArgs : BasicEventArgs
    {
        // 尝试获取对应类型的处理器列表
        if (!Instance._eventHandlers.TryGetValue(typeof(TEventArgs), out var handlers)) return;
        // 遍历处理器列表并调用它们
        args.SocketId = -1;
        args.UserInfo = null;
        foreach (var unused in handlers.Where(handler => !(bool)(handler.DynamicInvoke(args) ?? false)))
        {
            break;
        }
    }
}