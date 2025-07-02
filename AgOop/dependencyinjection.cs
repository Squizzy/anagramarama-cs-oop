using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

using Serilog.Events;
using System.Diagnostics;
using Serilog.Configuration;
using Serilog.Core;
// using Serilog.Enrichers.WithCaller;
using Serilog.Enrichers.CallerInfo;

using System.Xml;
using Serilog.Sinks.SystemConsole.Themes;

namespace AgOop
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
    // https://github.com/pm4net/serilog-enrichers-callerinfo/tree/master/Serilog.Enrichers.CallerInfo
    // class CallerEnricher : ILogEventEnricher
    // {
    //     public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    //     {
    //         var skip = 3;
    //         while (true)
    //         {
    //             var stack = new StackFrame(skip);
    //             if (!stack.HasMethod())
    //             {
    //                 logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue("<unknown method>")));
    //                 return;
    //             }

    //             var method = stack.GetMethod();
    //             if (method.DeclaringType.Assembly != typeof(Log).Assembly)
    //             {
    //                 var caller = $"{method.DeclaringType.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(pi => pi.ParameterType.FullName))})";
    //                 logEvent.AddPropertyIfAbsent(new LogEventProperty("Caller", new ScalarValue(caller)));
    //                 return;
    //             }

    //             skip++;
    //         }
    //     }
    // }

    // static class LoggerCallerEnrichmentConfiguration
    // {
    //     public static LoggerConfiguration WithCaller(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    //     {
    //         return enrichmentConfiguration.With<CallerEnricher>();
    //     }
    // }

    internal class DependencyInjection
    {

        private class AgOopConsoleThemes
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

        private string GetTemplatedMessage()
        {
            String consoleTime = "{Timestamp:ddMMyyyy HH:mm:ss}"; // the timestamp format
            String consoleLevel = "{Level:u3}"; // Uppercase 4 characters for level
            String consoleSourceContext = "{SourceContext}"; // Context of the log
            String consoleMessage = "{Message:lj}"; // Long message
            String consoleException = "{Exception}"; // Exception details
                                                     // String consoleLineNumber = "{callerLineNo}"; // Line number of the instantiation of the logger?
                                                     // String consoleMemberName = "{Member}"; // Member name
                                                     // from Serilog.Enrichers.CallerInfo:
            String consoleCallerLineNumber = "{LineNumber}"; // Line number of the instantiation of the logger?
            String consoleCallerColumnNumber = "{ColumnNumber}";
            String consoleCallerMethod = "{Method}";
            String consoleCallerSourceFile = "{SourceFile}";
            String consoleCallerNameSpace = "{Namespace}";

            return
              $"[{consoleTime}] {consoleLevel} [{consoleSourceContext}>{consoleCallerMethod}:{consoleCallerLineNumber}.{consoleCallerColumnNumber}] {consoleMessage}{Environment.NewLine}{consoleException}";
        }

        public ServiceProvider InitialiseServices(string[] args)
        {

            // Initialise the services
            var services = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Verbose()
                                .Enrich.WithCallerInfo(
                                    includeFileInfo: true,
                                    filePathDepth: 3,
                                    assemblyPrefix: "AgOop"
                                // ,
                                // prefix: "myprefix_"


                                // includeFileInfo: true,
                                // assemblyPrefix: null,
                                // filePathDepth: 3
                                )
                                .WriteTo.Console(theme: AgOopConsoleThemes.AgOop, outputTemplate: GetTemplatedMessage())
                                .CreateLogger();

            // // Use serilog logging
            services.AddLogging(builder => builder.AddSerilog(
                Log.Logger, dispose: true)
                                    // new LoggerConfiguration()
                                    //     .MinimumLevel.Verbose()
                                    //     .Enrich.WithCallerInfo(
                                    //         includeFileInfo: true,
                                    //         assemblyPrefix: "AgOop",
                                    //         // prefix: "myAss_",
                                    //         filePathDepth: 3
                                    // )
                                    // .WriteTo.Console(outputTemplate: GetTemplatedMessage())
                                    // .CreateLogger())
                                    );

            // // Use MS logging
            // services.AddLogging(builder =>
            // {
            //     builder.AddSimpleConsole(options =>
            //     {
            //         options.SingleLine = true;
            //         options.TimestampFormat = "[ddMMyyyy HH:mm:ss] ";
            //     });

            //     builder.AddDebug();
            //     builder.SetMinimumLevel(LogLevel.Information);
            // });

            // services.AddLogging(builder => builder.AddConsole().AddDebug());
            // services.AddLogging(builder => builder.AddConsole());
            // var provider = services.BuildServiceProvider();

            // Then build a temporary provider just for logging

            //     var loggingProvider = services.BuildServiceProvider();
            //     var loggerFactory = loggingProvider.GetRequiredService<ILoggerFactory>();


            //     // register the classes
            //     services.AddSingleton<AnagramsManager>(provider =>
            // new AnagramsManager(loggerFactory.CreateLogger<AnagramsManager>()));

            //     services.AddSingleton<LocaleManager>(provider =>
            // new LocaleManager(loggerFactory.CreateLogger<LocaleManager>()));

            //     services.AddSingleton<SoundManager>(provider =>
            // new SoundManager(loggerFactory.CreateLogger<SoundManager>()));

            //     services.AddSingleton<SpriteManager>(provider =>
            // new SpriteManager(loggerFactory.CreateLogger<SpriteManager>()));

            //     services.AddSingleton<UIManager>(provider =>
            // new UIManager(loggerFactory.CreateLogger<UIManager>()));

            //     services.AddSingleton<WordsList>(provider =>
            // new WordsList(loggerFactory.CreateLogger<WordsList>()));

            //     services.AddSingleton<Func<GameManager>>(provider =>
            //     () => new GameManager(
            //         provider.GetRequiredService<ILogger<GameManager>>(),
            //         provider.GetRequiredService<AnagramsManager>(),
            //         // provider.GetRequiredService<LocaleManager>(),
            //         provider.GetRequiredService<SoundManager>(),
            //         provider.GetRequiredService<UIManager>(),
            //         provider.GetRequiredService<SpriteManager>()
            //     // provider.GetRequiredService<WordsList>()
            //     )
            //     );

            // NOTE: The constructors for the below must be public
            services.AddSingleton<LocaleManager>();
            services.AddSingleton<AnagramsManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<SoundManager>();
            services.AddSingleton<SpriteManager>();
            services.AddSingleton<UIManager>();
            services.AddSingleton<WordsList>();

            // build the service provider
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

    }

}