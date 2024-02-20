using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ModManager.logger;

public static class SysLogger
{
    private static LogLevel LogLevel { get; set; } = LogLevel.Information;
    private static ILoggerFactory Factory { get; } = LoggerFactory.Create(builder =>
    {
        builder.AddConsole(options => options.FormatterName = nameof(LogFormatter))  
            .AddConsoleFormatter<LogFormatter, ConsoleFormatterOptions>()
            .AddFilter<ConsoleLoggerProvider>(level => level >= LogLevel);  
    });
    
    public static void SetLogLevel(string level)
    {
        LogLevel = level switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            "information" => LogLevel.Information,
            "warning" => LogLevel.Warning,
            "error" => LogLevel.Error,
            "critical" => LogLevel.Critical,
            _ => LogLevel.Information
        };
    }
    
    public static ILogger GetLogger(Type source)
    {
        var fullName = source.FullName ?? "<unknown>";
        return Factory.CreateLogger(fullName);
    }
}