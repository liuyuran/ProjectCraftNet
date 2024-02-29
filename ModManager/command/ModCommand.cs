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

    public string? GetShortDescription(UserInfo userInfo)
    {
        return Localize(ModId, "Show mod information");
    }

    public string? GetUsage(UserInfo userInfo, string[] args)
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
                // sb.AppendLine(Localize(ModId, "Available mods:"));
                // foreach (var mod in ModManager.AllMods)
                // {
                //     sb.AppendLine($"{mod.Id} - {mod.Name}");
                // }
                // NetworkEvents.FireSendEvent(clientInfo.SocketId, PackType.Chat, Encoding.UTF8.GetBytes(sb.ToString()));
                break;
            }
            case > 1:
                // var target = args[1];
                // var mod = ModManager.Mods.Find(m => m.Id == target);
                // if (mod == null)
                // {
                //     NetworkEvents.FireSendEvent(clientInfo.SocketId, PackType.Chat, Encoding.UTF8.GetBytes(Localize(ModId, "Mod not found")));
                //     return;
                // }
                // sb.AppendLine($"{mod.Name} - {mod.Description}");
                // sb.AppendLine(Localize(ModId, "Version: {0}", mod.Version));
                // sb.AppendLine(Localize(ModId, "Author: {0}", mod.Author));
                // NetworkEvents.FireSendEvent(clientInfo.SocketId, PackType.Chat, Encoding.UTF8.GetBytes(sb.ToString()));
                break;
        }
    }
}