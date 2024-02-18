using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace ModManager.logger;

public sealed class LogFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable? _optionsReloadToken;
    private ConsoleFormatterOptions _formatterOptions;

    public LogFormatter(IOptionsMonitor<ConsoleFormatterOptions> options) : base(nameof(LogFormatter))
    {
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _formatterOptions = options.CurrentValue;
    }

    private void ReloadLoggerOptions(ConsoleFormatterOptions options)
    {
        _formatterOptions = options;
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null)
            return;
        var now = DateTime.Now;
        var time = now.ToString(_formatterOptions.TimestampFormat ?? "yyyy-MM-dd HH:mm:ss.fff");
        textWriter.WriteLine($"[{time}] [{logEntry.LogLevel}] {message}");
    }

    public void Dispose() => _optionsReloadToken?.Dispose();
}