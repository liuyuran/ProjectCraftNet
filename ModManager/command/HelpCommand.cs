using System.Text;
using ModManager.network;
using ModManager.user;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.command;

public class HelpCommand: ICommand
{
    public string GetName()
    {
        return "help";
    }

    public string? GetShortDescription(UserInfo userInfo)
    {
        return Localize(ModId, "Show help message");
    }

    public string? GetUsage(UserInfo userInfo, string[] args)
    {
        return Localize(ModId, "/help [command]? [arg1]? [arg2]? ...");
    }

    public void Execute(UserInfo userInfo, string[] args)
    {
        var clientInfo = userInfo.ClientInfo;
        switch (args.Length)
        {
            case 0:
            {
                var sb = new StringBuilder();
                sb.AppendLine(Localize(ModId, "Available commands:"));
                sb.AppendLine(CommandManager.GetCommandShortDescription(userInfo));
                NetworkEvents.FireSendTextEvent(clientInfo.SocketId, sb.ToString());
                break;
            }
            case > 1:
                var target = args[1];
                var command = CommandManager.GetCommandUsage(userInfo, target, args[2..]);
                NetworkEvents.FireSendTextEvent(clientInfo.SocketId, command);
                break;
        }
    }
}