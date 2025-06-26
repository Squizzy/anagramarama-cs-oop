using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

// Adjust to AaOop if I want to use this logger
namespace AgOop
// namespace AgOopMSLogging
{

    public class AgOopLogger
    {
        /// <summary>
        /// The logger instance used for logging messages.
        /// </summary>
        private ILogger _logger { get; set; }

        // readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        // {

        //     // builder.AddSimpleConsole(options =>
        //     //     {
        //     //         options.SingleLine = true; // use single line output for console logs
        //     //         options.IncludeScopes = false; // Include scopes in console output
        //     //         options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
        //     //     });

        //     // builder.AddJsonConsole(options =>
        //     //     {
        //     //         // options.SingleLine = true; // use single line output for console logs
        //     //         options.IncludeScopes = false; // Include scopes in console output
        //     //         options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
        //     //     });

        //     // builder.AddCustomConsole(options =>
        //     // {
        //     //     options.CustomPrefix = $"[AgOop]{DateTime.UtcNow}:"; // Set a custom prefix for all log messages
        //     //     options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
        //     // });

        //     // Add console logger provider
        //     // builder.AddConsole(options =>
        //     //     {
        //     //         options.FormatterName = "customName"; // Set a custom formatter name
        //     //     }
        //     // );  // Add debug logger provider
        //     //     // config.AddDebug();  // Add debug logger provider
        //     //     // config.AddCustomFormatter(options =>
        //     //     //     {
        //     //     //         options.CustomPrefix = "AgOop"; // Set a custom prefix for all log messages

        //     //     //     });
        // });


        /// <summary>
        /// Creates a logger factory with a console logger provider.
        /// </summary>
        /// <remarks>
        /// This logger factory is used to create loggers for different categories.
        /// MS Logging console logger can only do one job (eg console, or to VSCode debug)
        /// MS Logging also cannot write to file
        /// If multiple handing or writing to file, use Serilog for example
        /// if multiple builder options are selected below, the logger will be built by the last one in the list

        readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {

            // // old console logger, output on two lines
            // builder.AddConsole(options =>
            // {
            //     // options.FormatterName = "customName"; // Set a custom formatter name
            //     options.FormatterName = "simple"; // Set a custom formatter name
            // });

            // // new simple console logger, output on one line
            // builder.AddSimpleConsole(options =>
            // {
            //     options.SingleLine = true; // use single line output for console logs
            //     options.IncludeScopes = false; // Include scopes in console output
            //     options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
            // });

            // // console logger outputting JSON
            // builder.AddJsonConsole(options =>
            // {
            //     options.IncludeScopes = false; // Include scopes in console output
            //     options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
            // });

            // // console logger outputting using a custom prefix, output on one line
            builder.AddCustomConsole(options =>
            {
                // options.CustomPrefix = $"[{DateTime.UtcNow:ddMMyyyy HH:mm:ss}] "; // Set a custom prefix for all log messages
                options.CustomPrefix = GetColouredDate($"{DateTime.UtcNow:ddMMyyyy HH:mm:ss}"); // Set a custom prefix for all log messages
                options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
            });

            // // vscode debut output logger - displays in vscode debug output window
            // // Use when running debug using the vscode debug button
            // builder.AddDebug();
        });




        public AgOopLogger(string categoryName)
        {
            // Create a logger for the specified category
            // _logger = loggerFactory.CreateLogger(categoryName);

            // // using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            // // {

            // //     // // original console logger, output on two lines
            // //     // builder.AddConsole(options =>
            // //     // {
            // //     //     // options.FormatterName = "customName"; // Set a custom formatter name
            // //     //     options.FormatterName = "simple"; // Set a custom formatter name
            // //     // });

            // //     // // simple console logger, output on one line
            // //     builder.AddSimpleConsole(options =>
            // //     {
            // //         options.SingleLine = true; // use single line output for console logs
            // //         options.IncludeScopes = false; // Include scopes in console output
            // //         options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
            // //     });

            // //     // console logger outputting JSON
            // //     builder.AddJsonConsole(options =>
            // //     {
            // //         options.IncludeScopes = false; // Include scopes in console output
            // //         options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output
            // //     });

            // //     // // custom console logger, output on one line with custom prefix
            // //     builder.AddCustomConsole(options =>
            // //     {
            // //         options.CustomPrefix = $"[AgOop]{DateTime.UtcNow}:"; // Set a custom prefix for all log messages
            // //         options.TimestampFormat = "yyMMdd HH:mm:ss "; // Set timestamp format for console output

            // //     });

            //     // // provide debug in vscode debug output
            //     builder.AddDebug();


            // });


            _logger = loggerFactory.CreateLogger(categoryName);
        }

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

        public static String GetColouredMessage(String message)
        {
            String colour = "\x1b[36m"; // Cyan
            String colourReset = "\x1b[0m";

            return $"{colour}{message}{colourReset}";
        }

        public static String GetColouredLocation(String memberName, int lineNumber)
        {
            String colour = "\x1b[90m"; // Magenta
            // String colour = "\x1b[35m"; // Magenta
            String colourReset = "\x1b[0m";

            return $"{colour}{memberName}:{lineNumber}{colourReset}";
        }

        public static String GetColouredDate(String date)
        {
            String colour = "\x1b[90m"; // Dark Gray
            String colourReset = "\x1b[0m";

            return $"{colour}{date}{colourReset}";  
        }

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
        // {

        // /// <summary>
        // /// set the interface defining the logging level and methods for the logger
        // /// </summary>
        // interface ILogger
        // {
        //     bool IsEnabled(LogLevel logLevel);
        //     IDisposable BeginScope<TState>(TState state);

        //     void Log(LogLevel logLevel, string message);
        //     void LogTrace(string message);
        //     void LogDebug(string message);
        //     void LogInformation(string message);
        //     void LogWarning(string message);
        //     void LogError(string message);
        //     void LogCritical(string message);
        // }

        // /// <summary>
        // /// Set the interface needed to create a logger provider
        // /// </summary> 
        // interface ILoggerProvider : IDisposable
        // {
        //     ILoggerProvider CreateLogger(string categoryName);
        // }

        // /// <summary>
        // /// set the interface needed to create a logger factory
        // /// </summary>
        // interface ILoggerFactory : IDisposable
        // {
        //     ILogger CreateLogger(string categoryName);
        //     void AddProvider(ILoggerProvider provider);
        // }

        // /// <summary>
        // /// create a logger factory with a console logger provider
        // /// </summary>
        // readonly ILoggerFactory loggerFactory = LoggerFactory.Create(config =>
        // {
        //     config.AddConsole(); // Add console logger provider
        //     config.AddDebug();  // Add debug logger provider
        // });

        // ILogger logger = loggerFactory.CreateLogger("Category.Medium");

        public sealed class CustomOptions : ConsoleFormatterOptions
        {
            public string? CustomPrefix { get; set; }
        }

        public static class ConsoleLoggerExtensions
        {
            public static ILoggingBuilder AddCustomConsole(
                this ILoggingBuilder builder,
                Action<CustomOptions> configure) =>
                builder.AddConsole(options =>
                        options.FormatterName = "customName")
                    // options.FormatterName = "simple")
                    .AddConsoleFormatter<CustomConsole, CustomOptions>(configure);



            // public static ILoggingBuilder AddCustomFormatter(
            //     this ILoggingBuilder builder,
            //     Action<CustomOptions> configure) =>
            //     builder.AddConsole(options =>
            //         options.FormatterName = "customName")
            //         .AddConsoleFormatter<CustomFormatter, CustomOptions>(configure);
        }


    public sealed class CustomConsole : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable? _optionsReloadToken;
        private CustomOptions _formatterOptions;

        /// <summary>
        /// Constructor for CustomConsole.
        /// </summary>
        /// <param name="options"></param>
        public CustomConsole(IOptionsMonitor<CustomOptions> options)
            // Case insensitive
            : base("customName") =>
            (_optionsReloadToken, _formatterOptions) =
                (options.OnChange(ReloadLoggerOptions), options.CurrentValue);


        /// <summary>
        /// Reloads the logger options.
        /// </summary>
        /// <param name="options"></param>
        private void ReloadLoggerOptions(CustomOptions options) =>
            _formatterOptions = options;


        /// <summary>
        /// Writes a log entry to the console.
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


        /// <summary>
        /// Writes custom logic to the text writer.
        /// /// </summary>
        /// /// <param name="textWriter">The text writer to write to.</param>
        /// /// <remarks>
        /// This method writes a custom prefix to the text writer.
        /// /// </remarks>
        private void CustomLogicGoesHere(TextWriter textWriter)
        {
            textWriter.Write(_formatterOptions.CustomPrefix);
        }

        /// <summary>
        /// Disposes the CustomConsole instance.
        /// </summary>
        /// <remarks>
        /// This method disposes of the options reload token if it is not null.
        /// </remarks>
        public void Dispose() => _optionsReloadToken?.Dispose();

    }
}
// }

