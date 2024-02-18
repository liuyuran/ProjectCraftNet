using CommandLine;
using ProjectCraftNet.config;

namespace ProjectCraftNet;

public static class Program {
    // ReSharper disable once ClassNeverInstantiated.Local
    private class CommandLineOptions {
        [Option('c', "config", Required = false, HelpText = "配置文件路径", Default = "/app/config/config.toml")]
        public required string ConfigPath { get; set; }
    }

    public static int Main(string[] args) {
        Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(o => {
            ConfigUtil.Instance.Init(o.ConfigPath);
        });
        return -100;
    }
}