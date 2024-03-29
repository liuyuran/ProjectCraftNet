using CommandLine;
using CoreMod.blocks;
using Microsoft.Extensions.Logging;
using ModManager.config;
using ModManager.eventBus;
using ModManager.eventBus.events;
using ModManager.game.block;
using ModManager.logger;
using ModManager.state;
using ProjectCraftNet.game;
using ProjectCraftNet.server;
using static ModManager.game.localization.LocalizationManager;

namespace ProjectCraftNet;

public static class Program
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(Program));
    public static string ModId => "core-system";
    private static readonly AutoResetEvent ClosingEvent = new(false);

    // ReSharper disable once ClassNeverInstantiated.Local
    private class CommandLineOptions
    {
        [Option('c', "config", Required = false, HelpText = "配置文件路径", Default = "/app/config/config.toml")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public required string ConfigPath { get; set; }
    }

    public static int Main(string[] args)
    {
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(o => { ConfigUtil.Instance.Init(o.ConfigPath); });
        var config = ConfigUtil.Instance.GetConfig();
        if (config.Core == null)
        {
            Logger.LogCritical("Config.Core is null.");
            return -100;
        }

        SysLogger.SetLogLevel(config.Core.LogLevel);
        GetLocalizationManager(ModId).LoadLocalization(config.Core.LocalizationPath);
        BlockManager.RegisterBlock<Air>();
        ModManager.mod.ModManager.LoadMods(config.Core.ModPath);
        if (config.NetworkTcp == null)
        {
            Logger.LogCritical("{}", Localize(ModId, "[{0}] not found", "network-tcp"));
            return -100;
        }
        
        var gameCore = new GameCore(config);
        Task.Run(() => gameCore.Start(), CraftNet.Instance.CancelToken.Token);
        var server = new TcpServer();
        Task.Run(() => server.StartServer(config.NetworkTcp.Host, config.NetworkTcp.Port), CraftNet.Instance.CancelToken.Token);
        Console.CancelKeyPress += (_, _) =>
        {
            CleanUp();
            ClosingEvent.Set();
        };
        
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            CleanUp();
            ClosingEvent.Set();
        };

        try
        {
            ClosingEvent.WaitOne();
        }
        catch (ThreadInterruptedException e)
        {
            Logger.LogInformation(e, "{}", Localize(ModId, "main-thread.interrupt"));
            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "{}", Localize(ModId, "main-thread.error"));
            return -1;
        }
        finally
        {
            CraftNet.Instance.CancelToken.Cancel(false);
        }
        return 0;
    }

    private static void CleanUp()
    {
        Logger.LogInformation("{}", Localize(ModId, "saving..."));
        EventBus.Trigger(new ArchiveEvent());
        Logger.LogInformation("{}", Localize(ModId, "saved."));
    }
}