using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace AgOop
{

    internal class DependencyInjection
    {

        public ServiceProvider InitialiseServices(string[] args)
        {
            // string loggerSelected = "serilog";

            // Initialise the services
            var services = new ServiceCollection();

            // Log.Logger = new LoggerConfiguration()
            //     .WriteTo.Console()
            //     .CreateLogger();
            // Create the logger with the logger factory
            // services.AddLogging(builder => LoggerFactory.CreateLogger<T>());
            // services.AddLogging(builder => SerilogService.AddLogging(services));
            // services.AddLogging(builder => builder.AddSerilog());
            services.AddLogging(builder => builder.AddSerilog(new LoggerConfiguration().WriteTo.Console().CreateLogger()));
            // services.AddLogging(builder => builder.AddConsole().AddDebug());

            // services.AddLogging(builder => builder.AddConsole());

            // var provider = services.BuildServiceProvider();

 // Then build a temporary provider just for logging
    var loggingProvider = services.BuildServiceProvider();
    var loggerFactory = loggingProvider.GetRequiredService<ILoggerFactory>();


            // register the classes
            services.AddSingleton<AnagramsManager>(provider => 
        new AnagramsManager(loggerFactory.CreateLogger<AnagramsManager>()));
            services.AddSingleton<LocaleManager>(provider => 
        new LocaleManager(loggerFactory.CreateLogger<LocaleManager>()));
            //     provider =>
            // {
            //     var logger = provider.GetRequiredService<ILogger<LocaleManager>>();
            //     return new LocaleManager(logger, args);
            // });
            services.AddSingleton<SoundManager>(provider => 
        new SoundManager(loggerFactory.CreateLogger<SoundManager>()));

            services.AddSingleton<SpriteManager>(provider => 
        new SpriteManager(loggerFactory.CreateLogger<SpriteManager>()));

            services.AddSingleton<UIManager>(provider => 
        new UIManager(loggerFactory.CreateLogger<UIManager>()));

            services.AddSingleton<WordsList>(provider => 
        new WordsList(loggerFactory.CreateLogger<WordsList>()));

            services.AddSingleton<Func<GameManager>>(provider => 
            () => new GameManager(
                provider.GetRequiredService<ILogger<GameManager>>(),
                provider.GetRequiredService<AnagramsManager>(),
                // provider.GetRequiredService<LocaleManager>(),
                provider.GetRequiredService<SoundManager>(),
                provider.GetRequiredService<UIManager>(),
                provider.GetRequiredService<SpriteManager>()
                // provider.GetRequiredService<WordsList>()
            )
            );
            // build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // var serviceProvider = InitialiseLogging.InitialiseLoggingService<GameManager>();

            // resolving the services
            // must not be done here
            // var AnagramsManager = serviceProvider.GetRequiredService<AnagramsManager>();
            // var GameManager = serviceProvider.GetRequiredService<GameManager>();
            // var LocaleManager = serviceProvider.GetRequiredService<LocaleManager>();
            // var soundManager = serviceProvider.GetRequiredService<SoundManager>();
            // var SpriteManager = serviceProvider.GetRequiredService<SpriteManager>();
            // var WordsList = serviceProvider.GetRequiredService<WordsList>();

            return serviceProvider;
        }

    }
}