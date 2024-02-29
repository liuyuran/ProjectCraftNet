using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using ModManager.user;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.command;

/// <summary>
/// 聊天栏命令管理器
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CommandManager
{
    private ILogger Logger { get; } = SysLogger.GetLogger(typeof(CommandManager));
    private static CommandManager Instance { get; } = new();
    private Dictionary<string, ICommand> Commands { get; } = new();

    /// <summary>
    /// 注册命令
    /// </summary>
    /// <param name="command">命令实例</param>
    public static void RegisterCommand(ICommand command)
    {
        Instance.Commands[command.GetName()] = command;
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    /// <param name="userInfo">执行命令的用户信息</param>
    /// <param name="command">命令名称</param>
    /// <param name="args">命令参数</param>
    public static void ExecuteCommand(UserInfo userInfo, string command, string[] args)
    {
        if (!Instance.Commands.TryGetValue(command, out var value))
        {
            Instance.Logger.LogError("Command {} not found", command);
            return;
        }

        value.Execute(userInfo, args);
    }

    /// <summary>
    /// 删除命令注册信息，可用于禁用危险命令
    /// </summary>
    /// <param name="command">命令名称</param>
    public static void UnregisterCommand(string command)
    {
        Instance.Commands.Remove(command);
    }

    /// <summary>
    /// 获取指定用户可用的全部命令，及其简短描述
    /// </summary>
    /// <param name="userInfo">当前用户信息</param>
    /// <returns>可输出的结果字符串</returns>
    public static string GetCommandShortDescription(UserInfo userInfo)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in Instance.Commands)
        {
            var desc = value.GetShortDescription(userInfo);
            if (desc == null) continue;
            sb.Append(key).Append(": ").Append(desc).Append('\n');
        }

        return sb.ToString();
    }

    /// <summary>
    /// 获取指定命令的使用说明
    /// </summary>
    /// <param name="userInfo">执行命令的用户信息</param>
    /// <param name="command">命令名称</param>
    /// <param name="args">命令参数</param>
    /// <returns>可输出的结果字符串</returns>
    public static string GetCommandUsage(UserInfo userInfo, string command, string[] args)
    {
        if (!Instance.Commands.TryGetValue(command, out var value))
        {
            return Localize(ModId, "Command not found");
        }

        var usage = value.GetUsage(userInfo, args);
        return usage ?? Localize(ModId, "Command not valid");
    }

    /// <summary>
    /// 用在事件监听器上面的函数，尝试解析文本消息为命令
    /// </summary>
    /// <param name="userInfo">当前用户信息</param>
    /// <param name="message">待解析的文本消息</param>
    /// <returns>该条消息是否为命令，如果是，返回true</returns>
    public static bool TryParseAsCommand(UserInfo userInfo, string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return false;
        if (message[0] != '/') return false;
        var args = message.Split(' ');
        var command = args[0][1..];
        args = args.Skip(1).ToArray();
        ExecuteCommand(userInfo, command, args);
        return true;
    }
}