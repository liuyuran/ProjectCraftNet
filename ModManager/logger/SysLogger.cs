using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ModManager.logger;

public static class SysLogger
{
    private static ILoggerFactory Factory { get; } = LoggerFactory.Create(builder =>
    {
        builder.AddConsole(options => options.FormatterName = nameof(LogFormatter))  
            .AddConsoleFormatter<LogFormatter, ConsoleFormatterOptions>();  
        builder.SetMinimumLevel(LogLevel.Debug);
        // builder.AddFilter("*", LogLevel.Debug);
    });
    public static ILogger GetLogger(Type source)
    {
        var fullName = source.FullName ?? "<unknown>";
        return Factory.CreateLogger(fullName);
    }
}