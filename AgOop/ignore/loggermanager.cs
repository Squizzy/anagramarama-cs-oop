using Microsoft.Extensions.Logging;

namespace AgOop1
{


    public static class LoggerFactory
    {

        // Create a logger for a specific class
        // internal static ILogger<T> CreateLogger<T>()
        //TODO: Removed line above here
        void donothing()
        {
            // string loggerSelected = "serilog";

            // switch (loggerSelected)
            // {
            //     case "serilog":
            //         return new SerilogLogger<T>();
            //     case "microsoft":
            //     default:
            //         return new MicrosoftLogger<T>();
            // }
        }
    }


    // public class InitialiseLogging
    // {
    //     // private static ILogger _logger;

    //     // public static ILogger ProvideLogger<T>()
    //     // {
    //     //     _logger = LoggerFactory.CreateLogger<T>();
    //     //     return _logger;
    //     // }

    //     public static ServiceProvider InitialiseLoggingService<T>()
    //     {
    //         // string loggerSelected = "serilog";

    //         // Initialise the services
    //         var services = new ServiceCollection();


    //         // Create the logger with the logger factory
    //         services.AddLogging(builder => LoggerFactory.CreateLogger<T>());
    //         // services.AddLogging(_logger<T>);
    //         // switch (loggerSelected)
    //         // {
    //         //     case "serilog":
    //         // ILogger<T> logger = LoggerFactory.CreateLogger<T>();
    //         //         services.AddLogging(builder => new SerilogLogger<T>());
    //         //         break;
    //         //     case "microsoft":
    //         //     default:
    //         //         services.AddLogging(builder => new MicrosoftLogger<T>());
    //         //         break;
    //         // }

    //         // register the classes
    //         services.AddSingleton<AnagramsManager>();
    //         services.AddSingleton<GameManager>();
    //         services.AddSingleton<SoundManager>();

    //         // build the service provider
    //         var serviceProvider = services.BuildServiceProvider();

    //         return serviceProvider;
    //     }
    // }
}