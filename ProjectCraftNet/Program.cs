﻿using CommandLine;
using Microsoft.Extensions.Logging;
using ModManager.config;
using ModManager.events;
using ModManager.logger;
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

        var cts = new CancellationTokenSource();
        var gameCore = new GameCore(config);
        Task.Run(() => gameCore.Start(), cts.Token);
        var server = new TcpServer();
        Task.Run(() => server.StartServer(config.NetworkTcp.Host, config.NetworkTcp.Port), cts.Token);
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
        catch (Exception e)
        {
            Logger.LogError(e, "{}", Localize(ModId, "Error occurred."));
            return -1;
        }
        finally
        {
            cts.Cancel(false);
        }
        return 0;
    }

    private static void CleanUp()
    {
        Logger.LogInformation("{}", Localize(ModId, "saving..."));
        GameEvents.FireArchiveEvent();
        Logger.LogInformation("{}", Localize(ModId, "saved."));
    }
}