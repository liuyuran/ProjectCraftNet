using CommandLine;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using ProjectCraftNet.config;
using ProjectCraftNet.game;
using ProjectCraftNet.server;
using static ModManager.localization.LocalizationManager;

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
        ModManager.ModManager.LoadMods(config.Core.ModPath);
        if (config.NetworkTcp == null)
        {
            Logger.LogCritical("{}", Localize(ModId, "[{0}] not found", "network-tcp"));
            return -100;
        }

        var gameCore = new GameCore(config);
        Task.Run(() => gameCore.Start());
        var server = new TcpServer();
        Task.Run(() => server.StartServer(config.NetworkTcp.Host, config.NetworkTcp.Port));
        Console.CancelKeyPress += (_, _) =>
        {
            ClosingEvent.Set();
        };

        ClosingEvent.WaitOne();
        return 0;
    }
}