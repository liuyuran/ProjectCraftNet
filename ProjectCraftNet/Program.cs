using CommandLine;
using Microsoft.Extensions.Logging;
using ModManager.logger;
using ProjectCraftNet.config;
using ProjectCraftNet.server;

namespace ProjectCraftNet;

public static class Program
{
    private static readonly ILogger Logger = SysLogger.GetLogger(typeof(Program));
    
    // ReSharper disable once ClassNeverInstantiated.Local
    private class CommandLineOptions {
        [Option('c', "config", Required = false, HelpText = "配置文件路径", Default = "/app/config/config.toml")]
        public required string ConfigPath { get; set; }
    }

    public static int Main(string[] args) {
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(o => { ConfigUtil.Instance.Init(o.ConfigPath); });
        var config = ConfigUtil.Instance.GetConfig();
        if (config.Core == null)
        {
            Logger.LogCritical("Config.Core is null.");
            return -100;
        }
        var manager = new ModManager.ModManager();
        manager.LoadMods(config.Core.ModPath);
        if (config.NetworkTcp == null)
        {
            Logger.LogCritical("Config.NetworkTcp is null.");
            return -100;
        }
        var server = new TcpServer();
        server.StartServer(config.NetworkTcp.Host, config.NetworkTcp.Port);
        return -100;
    }
}