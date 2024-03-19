using System.Text;
using ModManager.network;
using ModManager.user;
using static ModManager.localization.LocalizationManager;
using static ModManager.ModManager;

namespace ModManager.command;

public class ModCommand: ICommand
{
    public string GetName()
    {
        return "mod";
    }

    public string GetShortDescription(UserInfo userInfo)
    {
        return Localize(ModId, "Show mod information");
    }

    public string GetUsage(UserInfo userInfo, string[] args)
    {
        return Localize(ModId, "/mod [list|info] [modId]?");
    }

    public void Execute(UserInfo userInfo, string[] args)
    {
        var clientInfo = userInfo.ClientInfo;
        var sb = new StringBuilder();
        switch (args.Length)
        {
            case 0:
            {
                sb.AppendLine(Localize(ModId, "Available mods:"));
                foreach (var mod in AllMods)
                {
                    sb.AppendLine($"{mod.GetName()} - {mod.GetVersion()} - enabled: {mod.IsEnabled()}");
                    sb.AppendLine($"  {mod.GetDescription()}");
                    sb.AppendLine("------------------------------");
                }
                NetworkEvents.FireSendTextEvent(clientInfo.SocketId, sb.ToString());
                break;
            }
            case > 1:
            {
                var modId = args[1];
                var mod = AllMods.Find(m => m.GetName() == modId);
                if (mod == null)
                {
                    NetworkEvents.FireSendTextEvent(clientInfo.SocketId, Localize(ModId, "Mod not found"));
                    return;
                }
                sb.AppendLine($"{mod.GetName()} - {mod.GetVersion()}");
                sb.AppendLine($"  {mod.GetDescription()}");
                NetworkEvents.FireSendTextEvent(clientInfo.SocketId, sb.ToString());
                break;
            }
        }
    }
}