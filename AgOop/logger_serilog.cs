
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Runtime.CompilerServices;

// Requires installation of:
//  - Serilog logging:  
//  dotnet add package serilog.Extensions.Logging
//
//  - Serilog output to console:  
//  dotnet add package serilog.Sinks.Console
//
//  - Serilog output to file:  
//  dotnet add package serilog.Sinks.File
//
//  - Serilog output to VSCode debug console:
//  dotnet add package serilog.Sinks.Debug

// Configuration options:
// - Adjust the colour of of the items:
//      Adjust individual colours in custom class AgOopConsoleThemes 
//          ripped from (https://github.com/serilog/serilog-sinks-console)
//      Or use a standard AnsiConsoleThemes 
//          (then change .WriteTo.Console theme: AgOopConsoleThemes.AgOop 
//          to AnsiConsoleTheme.Literate for example)
//
// - Information in log message and order:
//      change in GetTemplatedMessage()
//
// - Debugging output:
//      Adjust (comment out or in) logger constructor .WriteTo.Console, debug, ...
//
// - Debug Level
//      The minimum log level for each category 
//          (basically whatever string used when instantiating a logger):
//      Change in GetMinimumLevelConfiguration() method
//      The current one is very specific to this application
//
// - Logging to file
//      The log file path (when logging to file):
//      Change in GetLogFile() method
//
// Then you can instantiate loggers like this:
// - namespace
//      Make sure the namespace at the top of this file matches your project)
//          (like "namespace AgOop" at the begining of this file which is specific to this project)
//
// - instantiation
//      As many loggers as wanted can be created with different category names
//      var logger = new AgOopLogger("MyCategory");
//      "MyCategory" is basically what you want, and what is checked in GetMinimumLevelConfiguration()
//      (I tend to create at class level)
//
// Usage:
// - The logger can be used like a Microsoft.Extensions.Logging.ILogger
//      logger.LogTrace("This is a trace message.");
//      logger.LogDebug("This is a debug message.");
//      logger.LogInformation("This is an information message.");
//      logger.LogWarning("This is a warning message.");
//      logger.LogError("This is an error message.");
//      logger.LogCritical("This is a critical message.");
//

// Adjust this back to AgOop if I want to use this logger
namespace AgOop
// namespace AgOopSerilog
{

    /// <summary> Static class that to give names to ANSI  VT100 colours.
    /// This class provides constants for foreground and background colors, as well as text styles like bold, italic, and underline.
    /// It can be used to format console output with colors and styles in a cross-platform manner.
    /// </summary>
    public static class Colours
    {
        public const string Reset = "\x1b[0m";

        public const string foregroundBlack = "\x1b[30m";
        public const string foregroundRed = "\x1b[31m";
        public const string foregroundGreen = "\x1b[32m";
        public const string foregroundYellow = "\x1b[33m";
        public const string foregroundBlue = "\x1b[34m";
        public const string foregroundMagenta = "\x1b[35m";
        public const string foregroundCyan = "\x1b[36m";
        public const string foregroundWhite = "\x1b[37m";

        public const string backgroundBlack = "\x1b[40m";
        public const string backgroundRed = "\x1b[41m";
        public const string backgroundGreen = "\x1b[42m";
        public const string backgroundYellow = "\x1b[43m";
        public const string backgroundBlue = "\x1b[44m";
        public const string backgroundMagenta = "\x1b[45m";
        public const string backgroundCyan = "\x1b[46m";
        public const string backgroundWhite = "\x1b[47m";

        public const string brightForegroundBlack = "\x1b[90m";
        public const string brightForegroundRed = "\x1b[91m";
        public const string brightForegroundGreen = "\x1b[92m";
        public const string brightForegroundYellow = "\x1b[93m";
        public const string brightForegroundBlue = "\x1b[94m";
        public const string brightForegroundMagenta = "\x1b[95m";
        public const string brightForegroundCyan = "\x1b[96m";
        public const string brightForegroundWhite = "\x1b[97m";

        public const string brightBackgroundBlack = "\x1b[100m";
        public const string brightBackgroundRed = "\x1b[101m";
        public const string brightBackgroundGreen = "\x1b[102m";
        public const string brightBackgroundYellow = "\x1b[103m";
        public const string brightBackgroundBlue = "\x1b[104m";
        public const string brightBackgroundMagenta = "\x1b[105m";
        public const string brightBackgroundCyan = "\x1b[106m";
        public const string brightBackgroundWhite = "\x1b[107m";

        public const string bold = "\x1b[1m";
        public const string dim = "\x1b[2m";
        public const string italic = "\x1b[3m";
        public const string underline = "\x1b[4m";
        public const string blink = "\x1b[5m";
        public const string reverse = "\x1b[7m";
        public const string hidden = "\x1b[8m";
        public const string strikethrough = "\x1b[9m";
        public const string bright = "\x1b[22m";
        public const string dimText = "\x1b[2m";
    }


    /// <summary> Class to instantiate a Serilog logger with, with a specific category name.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <remarks>
    /// This constructor sets up the Serilog logger with a minimum log level of Debug,
    /// </remarks>
    /// <example>
    /// var logger = new AgOopLoggerSerilog("MyCategory");
    /// logger.LogInformation("This is an information message.");
    /// logger.LogError("This is an error message.");
    /// </example>
    public class AgOopLogger(string categoryName)
    {

        /// <summary> The logger instance used for logging messages.
        /// </summary>
        private ILogger _logger { get; set; } = new LoggerConfiguration()

                // Set the min log level to verbose(Trace) or Debug for development
                // .MinimumLevel.Verbose() // Set the minimum log level

                // Override the minimum log level for the specific category
                // defaults to Warning
                .MinimumLevel.Override(categoryName, GetMinimumLevelConfiguration(categoryName)) // Set the minimum log level based on the category name

                // Format the console output and output to console
                // Comment if not using console output
                .WriteTo.Console(outputTemplate: GetTemplatedMessage(), theme: AgOopConsoleThemes.AgOop)
                // .WriteTo.Console(outputTemplate: GetTemplatedMessage(), theme: AnsiConsoleTheme.Sixteen)

                // Output to vscode debug console
                // Comment if not using debug output
                .WriteTo.Debug(outputTemplate: GetTemplatedMessage())

                // // Output to a local log file
                // Comment if not using file output
                // .WriteTo.File(GetLogFile(), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5) // File output with rolling interval

                // Create the logger with dedicated category name
                .CreateLogger()

                    .ForContext("SourceContext", categoryName);

        /// <summary> A static class that defines the AnsiConsole themes used for Serilog Console logging.
        /// /// </summary>
        /// /// <remarks>
        /// This class currently contains a single theme called Literate
        /// Colours can me modified using the Colour class above.
        static class AgOopConsoleThemes
        {
            public static AnsiConsoleTheme AgOop { get; } = new AnsiConsoleTheme(
                new Dictionary<ConsoleThemeStyle, string>
                {
                    // [ConsoleThemeStyle.Text] = "\x1b[38;5;0015m",
                    [ConsoleThemeStyle.Text] = Colours.brightForegroundCyan,
                    [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0007m",
                    [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0008m",
                    [ConsoleThemeStyle.Invalid] = "\x1b[38;5;0011m",
                    [ConsoleThemeStyle.Null] = "\x1b[38;5;0027m",
                    // [ConsoleThemeStyle.Name] = "\x1b[38;5;0007m",
                    [ConsoleThemeStyle.Name] = Colours.foregroundBlue,
                    // [ConsoleThemeStyle.String] = "\x1b[38;5;0045m",
                    [ConsoleThemeStyle.String] = Colours.foregroundBlue,
                    [ConsoleThemeStyle.Number] = "\x1b[38;5;0200m",
                    [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0027m",
                    [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0085m",
                    [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;5;0007m",
                    [ConsoleThemeStyle.LevelDebug] = Colours.foregroundGreen,
                    [ConsoleThemeStyle.LevelInformation] = Colours.foregroundBlue,
                    [ConsoleThemeStyle.LevelWarning] = Colours.foregroundYellow,
                    [ConsoleThemeStyle.LevelError] = Colours.foregroundRed,
                    [ConsoleThemeStyle.LevelFatal] = Colours.foregroundMagenta,
                });
        }

        /// <summary> The formatting of the console output.
        /// This excludes colours which are dealt using the theme
        /// </summary>
        /// <returns>The message to be logged</returns>
        private static string GetTemplatedMessage()
        {
            String consoleTime = "{Timestamp:ddMMyyyy HH:mm:ss}"; // the timestamp format
            String consoleLevel = "{Level:u}"; // Uppercase 4 characters for level
            String consoleSourceContext = "{SourceContext}"; // Context of the log
            String consoleMessage = "{Message:lj}"; // Long message
            String consoleException = "{Exception}"; // Exception details
            String consoleLineNumber = "{lineNumber}"; // Line number of the instantiation of the logger?
            String consoleMemberName = "{memberName}"; // Member name

            return
              $"[{consoleTime}] {consoleLevel} [{consoleSourceContext}>{consoleMemberName}:{consoleLineNumber}] {consoleMessage}{Environment.NewLine}{consoleException}";
        }

        /// <summary> The location of the log file.
        /// </summary>
        /// <returns>The path to the log file.</returns>
        private static string GetLogFile()
        {
            String logFile = "log/AgOopLog.txt";
            return logFile;
            // return "AgOopLog.txt";
        }

        /// <summary> Sets the minimum log level configuration based on the category name.
        /// </summary>
        /// <param name="categoryName">the category (like class initiating)</param>
        /// <returns>the log level</returns>
        private static Serilog.Events.LogEventLevel GetMinimumLevelConfiguration(string categoryName)
        {
            switch (categoryName)
            {
                case "GameManager":
                    return Serilog.Events.LogEventLevel.Debug; // Set minimum level to Debug for GameManager
                case "WordsList":
                    return Serilog.Events.LogEventLevel.Information; // Set minimum level to Information for WordsList
                case "Anagrams":
                    return Serilog.Events.LogEventLevel.Debug;
                default:
                    return Serilog.Events.LogEventLevel.Warning;
            }
            // This method can be used to set the minimum log level dynamically if needed
        }

        // The below are added for compatibility with Microsoft.Extensions.Logging
        public void LogTrace(
                    string message,
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string filePath = "",
                    [CallerLineNumber] int lineNumber = 0,
                    params object[] args)
        {
            _logger
                .ForContext("memberName", memberName)
                .ForContext("filePath", filePath)
                .ForContext("lineNumber", lineNumber)
                .Verbose(message, args);
        }

        public void LogDebug(
                    string message,
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string filePath = "",
                    [CallerLineNumber] int lineNumber = 0,
                    params object[] args)
        {
            _logger
                .ForContext("memberName", memberName)
                .ForContext("filePath", filePath)
                .ForContext("lineNumber", lineNumber)
                .Debug(message, args);
        }

        public void LogInformation(
                    string message,
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string filePath = "",
                    [CallerLineNumber] int lineNumber = 0,
                    params object[] args)
        {
            _logger
                .ForContext("memberName", memberName)
                .ForContext("filePath", filePath)
                .ForContext("lineNumber", lineNumber)
                .Information(message, args);
        }

        public void LogWarning(
                    string message,
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string filePath = "",
                    [CallerLineNumber] int lineNumber = 0,
                    params object[] args)
        {
            _logger
                .ForContext("memberName", memberName)
                .ForContext("filePath", filePath)
                .ForContext("lineNumber", lineNumber)
                .Warning(message, args);
        }

        public void LogError(
                    string message,
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string filePath = "",
                    [CallerLineNumber] int lineNumber = 0,
                    params object[] args)
        {
            _logger
                .ForContext("memberName", memberName)
                .ForContext("filePath", filePath)
                .ForContext("lineNumber", lineNumber)
                .Error(message, args);
        }

        public void LogCritical(
                    string message,
                    [CallerMemberName] string memberName = "",
                    [CallerFilePath] string filePath = "",
                    [CallerLineNumber] int lineNumber = 0,
                    params object[] args)
        {
            _logger
                .ForContext("memberName", memberName)
                .ForContext("filePath", filePath)
                .ForContext("lineNumber", lineNumber)
                .Fatal(message, args);
        }

    }
}
