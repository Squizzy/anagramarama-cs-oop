using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;


// Adjust to AaOop if I want to use this logger
namespace AgOop1
// namespace AgOopMSLogging
{

    // public class AgOopLogger
    public class MicrosoftLogger<T> : ILogger<T>
    {

        // Implement the Microsoft logger interface

        public IDisposable BeginScope<TState>(TState state) => null!;

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => _logger.IsEnabled(LogLevel.Trace),
                LogLevel.Debug => _logger.IsEnabled(LogLevel.Debug),
                LogLevel.Information => _logger.IsEnabled(LogLevel.Information),
                LogLevel.Warning => _logger.IsEnabled(LogLevel.Warning),
                LogLevel.Error => _logger.IsEnabled(LogLevel.Error),
                LogLevel.Critical => _logger.IsEnabled(LogLevel.Critical),
                _ => false
            };
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);

            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.LogTrace(exception, message);
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(exception, message);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(exception, message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(exception, message);
                    break;
                case LogLevel.Error:
                    _logger.LogError(exception, message);
                    break;
                case LogLevel.Critical:
                    _logger.LogCritical(exception, message);
                    break;
            }
        }


        /// <summary> The logger instance used for logging messages.
        /// </summary>
        private ILogger _logger { get; set; }

        /// <summary> Creates a logger factory with a console logger provider.
        /// </summary>
        /// <remarks>
        /// This logger factory is used to create loggers for different categories.
        /// MS Logging console logger can only do one job (eg console, or to VSCode debug)
        /// MS Logging also cannot write to file
        /// If multiple handing or writing to file, use Serilog for example
        /// if multiple builder options are selected below, the logger will be built by the last one in the list
        // readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        // {
        //     // // DO NOT REMOVE THE BELOW COMMENTED LINES, 
        //     // // THEY GET UNCOMMENDED DEPENDING ON THE LOGGER REQUESTED

        //     // // NOTE: old console logger, output on two lines
        //     // builder.AddConsole(options =>
        //     // {
        //     //     // options.FormatterName = "customName"; // Set a custom formatter name
        //     //     options.FormatterName = "simple"; // Set a custom formatter name
        //     // });

        //     // // NOTE: new simple console logger, output on one line
        //     // builder.AddSimpleConsole(options =>
        //     // {
        //     //     options.SingleLine = true; // use single line output for console logs
        //     //     options.IncludeScopes = false; // Include scopes in console output
        //     //     options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
        //     // });

        //     // // NOTE: console logger outputting JSON
        //     // builder.AddJsonConsole(options =>
        //     // {
        //     //     options.IncludeScopes = false; // Include scopes in console output
        //     //     options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
        //     // });

        //     // // NOTE: console logger outputting using a custom prefix, output on one line
        //     builder.AddCustomConsole(options =>
        //     {
        //         options.CustomPrefix = GetColouredDate($"{DateTime.UtcNow:ddMMyyyy HH:mm:ss}"); // Set a custom prefix for all log messages
        //         options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
        //     });

        //     // // NOTE: vscode debut output logger - displays in vscode debug output window
        //     // // Use when running debug using the vscode debug button
        //     // builder.AddDebug();
        // });

        /// <summary> Creates a new instance of the AgOopLogger class.
        /// </summary>
        /// <param name="categoryName">The category name for the logger.</param>
        /// <remarks>
        /// This constructor initializes the logger with the specified category name.
        /// It uses the logger factory to create a logger instance for the given category.
        /// </remarks>
        internal MicrosoftLogger()
        // public AgOopLogger(string categoryName)
        {
            // Create a logger for the specified category
            // _logger = loggerFactory.CreateLogger();
            _logger = new MicrosoftLogger<T>();
        }

        /// <summary> Overrides the default logger to include line number, file path, and member name.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="lineNumber"></param>
        /// <param name="filePath"></param>
        /// <param name="memberName"></param>
        public void LogTrace(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
                =>
                _logger.LogTrace($"{GetPostfixMessage(LogLevel.Trace, message, memberName, lineNumber, 4)}"); // [{memberName}]:{lineNumber} - {GetColouredMessage(message)}");

        public void LogDebug(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
                =>
                // _logger.LogDebug($"[{memberName}]:{lineNumber} - {message}");
                _logger.LogDebug($"{GetPostfixMessage(LogLevel.Debug, message, memberName, lineNumber, 4)}"); // [{memberName}]:{lineNumber} - {GetColouredMessage(message)}");

        public void LogInformation(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
                =>
                // Log(LogLevel.Information, $"[{memberName}]:{lineNumber} - {message}");
                _logger.LogInformation($"{GetPostfixMessage(LogLevel.Information, message, memberName, lineNumber, 4)}"); // [{memberName}]:{lineNumber} - {GetColouredMessage(message)}");
                                                                                                                          // Log(LogLevel.Information, $"[{memberName}]:{lineNumber} - {message}");
                                                                                                                          // _logger.LogInformation($"[{memberName}]:{lineNumber} - {message}");

        public void LogWarning(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
                =>
                    _logger.LogWarning($"{GetPostfixMessage(LogLevel.Warning, message, memberName, lineNumber, 4)}"); // [{memberName}]:{lineNumber} - {GetColouredMessage(message)}");

        public void LogError(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
                =>
                    _logger.LogError($"{GetPostfixMessage(LogLevel.Error, message, memberName, lineNumber, 4)}"); // [{memberName}]:{lineNumber} - {GetColouredMessage(message)}");

        public void LogCritical(string message,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
                =>
                    _logger.LogCritical($"{GetPostfixMessage(LogLevel.Critical, message, memberName, lineNumber, 4)}"); // [{memberName}]:{lineNumber} - {GetColouredMessage(message)}");

        //override the Log method to include line number, file path, and member name
        // public void Log(LogLevel logLevel, string message)
        // {
        //     if (_logger.IsEnabled(logLevel))
        //     {
        //         _logger.Log(logLevel, message);

        //         // Console.WriteLine($"\x1b[32m{logLevel.ToString()[..4]}\x1b[0m {message}");
        //     }
        // }


        /// <summary> Gets a coloured string representation of the log level.
        /// </summary>
        /// <param name="logLevel">The log level to get the string representation for.</param>
        /// <param name="numberOfChars">The number of characters to include in the string representation.</param>
        /// <returns>A coloured string representation of the log level.</returns>
        public static String GetColouredLogLevelString(LogLevel logLevel, int numberOfChars = 4)
        {
            String msg = logLevel.ToString().Length > numberOfChars
                ? logLevel.ToString()[..numberOfChars]
                : logLevel.ToString();

            String colour = logLevel switch
            {
                LogLevel.Trace => "\x1b[37m", // White
                LogLevel.Debug => "\x1b[36m", // Cyan
                LogLevel.Information => "\x1b[32m", // Green
                LogLevel.Warning => "\x1b[33m", // Yellow
                LogLevel.Error => "\x1b[31m", // Red
                LogLevel.Critical => "\x1b[41m", // Background Red
                _ => "\x1b[0m" // Reset
            };

            String colourReset = "\x1b[0m";

            return $"{colour}{msg.PadRight(numberOfChars)}{colourReset}"; // Reset color after the message
        }

        /// <summary> Change the colour of the message.
        /// </summary>
        /// <param name="message">The message to colour.</param>
        /// <returns>A coloured string representation of the message.</returns>
        /// remarks>
        /// This method wraps the message in ANSI escape codes to change its color.
        public static String GetColouredMessage(String message)
        {
            String colour = "\x1b[36m"; // Cyan
            String colourReset = "\x1b[0m";

            return $"{colour}{message}{colourReset}";
        }

        /// <summary> Change the colour of the location (which method/line number).
        /// </summary>
        /// <param name="memberName">The name of the member (method or property).</param>
        /// <param name="lineNumber">The line number where the log was called.</param>
        /// <returns>A coloured string representation of the location.</returns>
        /// remarks>
        /// This method wraps the member name and line number in ANSI escape codes to change their color.
        public static String GetColouredLocation(String memberName, int lineNumber)
        {
            String colour = "\x1b[90m"; // Magenta
            // String colour = "\x1b[35m"; // Magenta
            String colourReset = "\x1b[0m";

            return $"{colour}{memberName}:{lineNumber}{colourReset}";
        }

        /// <summary> Change the colour of the date (used in prefix).
        /// </summary>
        /// <param name="date">The date to colour.</param>
        /// <returns>A coloured string representation of the date.</returns>
        /// remarks>
        /// This method wraps the date in ANSI escape codes to change its color.
        public static String GetColouredDate(String date)
        {
            String colour = "\x1b[90m"; // Dark Gray
            String colourReset = "\x1b[0m";

            return $"{colour}{date}{colourReset}";
        }

        /// <summary> Gets a formatted message with log level, location, and message.
        /// </summary>
        /// <param name="logLevel">The log level of the message.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="memberName">The name of the member (method or property) where the log was called.</param>
        /// <param name="lineNumber">The line number where the log was called.</param>
        /// <param name="numberOfLogLevelChars">The number of characters to include in the log level string representation.</param>
        /// <returns>A formatted string containing the log level, location, and message.</returns>
        /// remarks>
        /// This method constructs a formatted string that includes the log level, 
        /// location (member name and line number), and the message.
        public static String GetPostfixMessage(
            LogLevel logLevel,
            String message,
            String memberName = "",
            int lineNumber = 0,
            int numberOfLogLevelChars = 4
            )
        {
            return $" [{GetColouredLogLevelString(logLevel, numberOfLogLevelChars)}] [{GetColouredLocation(memberName, lineNumber)}]  {GetColouredMessage(message)}";
        }

    }

    /// <summary> Custom options for the custom console logger.
    /// </summary>
    /// <remarks>
    /// This class extends ConsoleFormatterOptions to allow for custom configuration options.
    /// </remarks>
    public sealed class CustomOptions : ConsoleFormatterOptions
    {
        public string? CustomPrefix { get; set; }
    }

    /// <summary> Extensions for adding a custom console logger to the logging builder.
    /// </summary>
    /// <remarks>
    /// This class provides an extension method to add a custom console logger with specific options.
    /// </remarks>
    public static class ConsoleLoggerExtensions
    {
        public static ILoggingBuilder AddCustomConsole(
            this ILoggingBuilder builder,
            Action<CustomOptions> configure) =>
            builder.AddConsole(options =>
                    options.FormatterName = "customName")
                .AddConsoleFormatter<CustomConsole, CustomOptions>(configure);
    }

    /// <summary> Custom console logger that formats log entries with a custom prefix.
    /// </summary>
    /// <remarks>
    /// This class extends ConsoleFormatter to provide custom formatting for log entries.
    /// It uses the CustomOptions class to allow for custom configuration options.
    /// </remarks>
    public sealed class CustomConsole : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable? _optionsReloadToken;
        private CustomOptions _formatterOptions;

        /// <summary> Constructor for CustomConsole.
        /// </summary>
        /// <param name="options"></param>
        public CustomConsole(IOptionsMonitor<CustomOptions> options)
            // Case insensitive
            : base("customName") =>
            (_optionsReloadToken, _formatterOptions) =
                (options.OnChange(ReloadLoggerOptions), options.CurrentValue);

        /// <summary> Reloads the logger options.
        /// </summary>
        /// <param name="options"></param>
        private void ReloadLoggerOptions(CustomOptions options) =>
            _formatterOptions = options;

        /// <summary> Writes a log entry to the console.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="logEntry">The log entry to write.</param>
        /// <param name="scopeProvider">The scope provider for the log entry.</param>
        /// <param name="textWriter">The text writer to write the log entry to.</param>
        /// <remarks>
        /// This method formats the log entry using the provided formatter,
        /// writes a custom prefix to the text writer,
        /// and then writes the formatted message to the text writer.
        /// If the formatted message is null, it does nothing.
        /// /// </remarks>
        public override void Write<TState>(
            in LogEntry<TState> logEntry,
            IExternalScopeProvider? scopeProvider,
            TextWriter textWriter)
        {
            string? message =
                logEntry.Formatter?.Invoke(
                    logEntry.State, logEntry.Exception);

            if (message is null)
            {
                return;
            }

            CustomLogicGoesHere(textWriter);
            textWriter.WriteLine(message);
        }

        /// <summary> Writes custom logic to the text writer.
        /// </summary>
        /// <param name="textWriter">The text writer to write to.</param>
        /// <remarks>
        /// This method writes a custom prefix to the text writer.
        /// </remarks>
        private void CustomLogicGoesHere(TextWriter textWriter)
        {
            textWriter.Write(_formatterOptions.CustomPrefix);
        }

        /// <summary> Disposes the CustomConsole instance.
        /// </summary>
        /// <remarks>
        /// This method disposes of the options reload token if it is not null.
        /// </remarks>
        public void Dispose() => _optionsReloadToken?.Dispose();

    }
}

