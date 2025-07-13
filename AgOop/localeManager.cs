using System.Globalization;
using Microsoft.Extensions.Logging;

namespace AgOop
{

    internal class LocaleSettings
    {
        // <summary> separator for the OS used </summary>
        // internal static char DIR_SEP = Path.DirectorySeparatorChar;
        // internal static string DIR_SEP;
        // internal static string DIR_SEP = "/"; //Path.DirectorySeparatorChar;

        /// <summary> Path to the resources (icon, images, audio, wordslist) </summary>
        internal string resourcesPath;

        /// <summary> Path to the audio files </summary>
        /// TODO: Change to audioPath
        internal string audioPath;

        /// <summary> Path to the icon file(s) </summary>
        internal string iconPath;

        /// <summary> Path to international content (images, wordslist) </summary>
        internal string i18nPath;

        /// <summary> Path of a specified locale to use (dict, images) </summary>
        internal string localePath;

        /// <value> Locale to be used when non-default language (where the worldlist.txt is)</value>
        internal string language;

        /// <summary> Images path </summary>
        internal string imagesPath;

        /// <summary> Default locale (Dictionary and images) </summary>
        internal string DEFAULT_LOCALE_PATH;

        /// <summary>the path to the locale data to use for the game. 
        /// Used for Gamerzilla, which is not implemented here</summary>
        /// https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
        internal string userPath;

        /// <summary>the Gamerzilla base path for the game
        /// which is not implemented here</summary>
        internal string basePath;

        /// <summary> wordslist path </summary>
        internal string wordslistPath;
        
        /// <summary> Path to the config.ini file </summary>
        internal string configFilePath;


        public LocaleSettings()
        {

            // TODO: Re-set the values if localePath changes

            // generic paths
            resourcesPath = Path.Join(LocaleConstants.ResourcesFolderName);

            // AUDIO: audio (wav) files path (not language specific)
            audioPath = Path.Join(resourcesPath, LocaleConstants.AudioFolderName);

            // ICONS: path to the icon file(s)
            iconPath = Path.Join(resourcesPath, LocaleConstants.IconFolderName);

            // i18n paths
            i18nPath = Path.Join(resourcesPath, LocaleConstants.I18nFolderName);

            // by default, point to english dictionary
            // TODO: Use the ini file and point to the last language used to play
            language = Path.Join(LocaleConstants.DefaultLocale);

            // Default locale path (eg "res/i18n/en-GB/") that will be used if no locale is specified
            DEFAULT_LOCALE_PATH = Path.Join(i18nPath, LocaleConstants.DefaultLocale);

            // locale folder (eg "res/i18n/en-GB/") that will be modified if the locale is changed
            localePath = Path.Join(i18nPath, language);

            // DICTIONARY: path to the dictionary file (it is called wordslist)
            wordslistPath = Path.Join(localePath, LocaleConstants.WordslistFolderName, LocaleConstants.WordslistDefaultName);

            // IMAGES: path to the images (background and letter banks)
            imagesPath = Path.Join(localePath, LocaleConstants.ImagesFolderName);

            // CONFIG.INI: path to the config.ini file
            configFilePath = Path.Join(localePath, LocaleConstants.ConfigFileName);



            // For Gamerzilla I think
            userPath = "";
            basePath = "";
        }
    }

    internal static class LocaleConstants
    {
        /// <summary> A default values for locale and folders</summary>
        internal const string ResourcesFolderName = "res";
        internal const string AudioFolderName = "audio";
        internal const string IconFolderName = "icons";
        internal const string I18nFolderName = "i18n";
        internal const string ImagesFolderName = "images";
        internal const string WordslistFolderName = "";
        internal const string DefaultLocale = "en-GB";
        internal const string WordslistDefaultName = "wordlist.txt";
        internal const string ConfigFileName = "config.ini";
    }

    internal class LocaleManager
    {

        private readonly ILogger<LocaleManager> _logger;
        private LocaleSettings _localeSettings;

        private string _commandLineLocale = "";

        // /// <summary>Directory separator for the OS used </summary>
        // // internal static char DIR_SEP = Path.DirectorySeparatorChar;
        // internal char DIR_SEP;
        // // internal static string DIR_SEP = "/"; //Path.DirectorySeparatorChar;

        // /// <summary> Resources path </summary>
        // internal string RESOURCES_PATH;

        // /// <summary> Audio data path</summary>
        // internal string audioSubPath;

        // /// <summary> International content path</summary>
        // internal string i18nPath;

        // /// <summary> Path of a specified locale to use (dict, images) </summary>
        // internal string localePath;
        // // internal string localePath = i18nPath + language;

        // /// <value> Locale to be used when non-default language (where the worldlist.txt is)</value>
        // internal string language;

        // /// <summary> A default value for the locale (language) </summary>
        // internal string defaultLocale;

        // /// <summary> Images path </summary>
        // internal string imagesSubPath;

        // /// <summary> Default locale (Dictionary and images) </summary>
        // // internal static string DEFAULT_LOCALE_PATH = i18nPath + "en-GB" + DIR_SEP;
        // internal string DEFAULT_LOCALE_PATH;

        // /// <summary>the path to the locale data to use for the game. 
        // /// Used for Gamerzilla, which is not implemented here</summary>
        // /// https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
        // internal string userPath;

        // /// <summary>the base path for the game. Used for Gamerzilla, which is not implemented here</summary>
        // internal string basePath;

        public LocaleManager(ILogger<LocaleManager> logger, LocaleSettings localeSettings)
        {

            _logger = logger;
            Console.WriteLine("LocaleManager Constructor");
            // _logger = LoggerFactory.CreateLogger<LocaleManager>();

            _localeSettings = localeSettings;

            // // generic paths
            // // LocaleSettings.DIR_SEP = Path.DirectorySeparatorChar.ToString();
            // _localeSettings.resourcesPath = Path.Join(LocaleConstants.ResourcesFolderName);

            // // AUDIO: audio (wav) files path (not language specific)
            // _localeSettings.audioPath = Path.Join(_localeSettings.resourcesPath, LocaleConstants.AudioFolderName);

            // // ICONS: path to the icon file(s)
            // _localeSettings.iconPath = Path.Join(_localeSettings.resourcesPath, LocaleConstants.IconFolderName);

            // // i18n paths
            // _localeSettings.i18nPath = Path.Join(_localeSettings.resourcesPath, LocaleConstants.I18nFolderName);

            // // by default, point to english dictionary
            // // TODO: Use the ini file and point to the last language used to play
            // _localeSettings.language = Path.Join(LocaleConstants.defaultLocale);

            // // Default locale path (eg "res/i18n/en-GB/") that will be used if no locale is specified
            // _localeSettings.DEFAULT_LOCALE_PATH = Path.Join(_localeSettings.i18nPath + LocaleConstants.defaultLocale);

            // // locale folder (eg "res/i18n/en-GB/") that will be modified if the locale is changed
            // _localeSettings.localePath = Path.Join(_localeSettings.i18nPath, LocaleConstants.language);

            // // DICTIONARY: path to the dictionary file
            // _localeSettings.dictionaryPath = Path.Join(_localeSettings.localePath + LocaleConstants.wordslistFolderName);

            // // IMAGES: path to the images (background and letter banks)
            // _localeSettings.imagesPath = Path.Join(_localeSettings.localePath + LocaleConstants.ImagesFolderName);


            // // For Gamerzilla I think
            // _localeSettings.userPath = "";
            // _localeSettings.basePath = "";

            _logger.LogDebug($"imagesPath: {_localeSettings.imagesPath}");
        }

        internal void GetBasePathAndInitLocale(string[] args)
        {

            // Get the base path for Gamerzilla (not use in this implementation)
            GetBasePath();

            InitLocale(args);

            // No longer needed as Path.Join is used
            // if (LocaleSettings.language[LocaleSettings.language.Length - 1] != LocaleSettings.DIR_SEP)
            // {
            //     LocaleSettings.language += LocaleSettings.DIR_SEP;
            // }

        }


        /// <summary> Sets the Gamerzilla specific basepath, per OS
        /// The basepath is actually not used as Gamerzilla is not implemented</summary>
        internal void GetBasePath()
        {
            // Get the based path for the application data (although this is not really portable)
            string? env = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (env == null)
            {
                // env = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.local/share/";
                // env = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + DIR_SEP + ".local" + DIR_SEP + "share" + DIR_SEP;
                env = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
            }


            _localeSettings.basePath = Path.Join(env, "anagramarama");
        }

        /// <summary>Parses the command line arguments and act on them. </summary>
        /// <param name="args">the command line arguments, passed by Main</param>
        internal void ParseCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                // preparing for potential multiple arguments
                // currently this is not handled
                // TODO: possibility to specify the wordslist filename
                // TODO: possibility to specify the config.ini filename
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-h":
                        case "--help":
                            Console.WriteLine(GameConstants.HELP);
                            // _logger.LogInformation(GameConstants.HELP);
                            Environment.Exit(0);
                            break;

                        case "-v":
                        case "--version":
                            Console.WriteLine(GameConstants.VERSION);
                            // _logger.LogInformation(GameConstants.VERSION);
                            Environment.Exit(0);
                            break;

                        case "-l":
                        case "--locale":
                            // Console.WriteLine("here " +  args[i] + " " + args[i + 1]);
                            _logger.LogInformation("locale selected: " + args[i] + " " + args[i + 1]);
                            _commandLineLocale = args[i + 1];
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>process the locale provided by command line, if any </summary>
        /// <param name="args"></param>
        internal void InitLocaleFromCommandLine()
        {
            if (_commandLineLocale == "")
            {
                return;
            }

            if (GameConstants.AllowedLanguages.Contains(_commandLineLocale))
            {
                _localeSettings.localePath = Path.Join(_localeSettings.i18nPath, _commandLineLocale);
                return;
            }

            Console.WriteLine($"The language code requested ({_commandLineLocale} is not registered in the app, checking if it exists");
            // _logger.LogWarning($"The language code requested ({_commandLineLocale} is not registered in the app, checking if it exists");
            // Environment.Exit(0);


            // Check if the next argument is one of the available as defined in AllowedLanguages
            // if (args[i + 1] != null && GameConstants.AllowedLanguages.Contains(args[i + 1]))
            // {
            //     _localeSettings.localePath = Path.Join(_localeSettings.i18nPath, args[i + 1]);
            //     // _localeSettings.localePath = args[i + 1];
            //     break;
            // }
            // else
            // {
            //     Console.WriteLine($"the language code requested ({args[i + 1]} is not available");
            //     _logger.LogWarning($"the language code requested ({args[i + 1]} is not available");
            //     Environment.Exit(0);
            //     break;
            // }
            //             default:
            //                 break;
            //         }
            //     }

            // localePath = args[1].Trim();
            // TODO: remove any trailing or leading directory separators and mode more checks in general
            // NEW: no more need to reconstruct the path manually
            // LocaleSettings.language += LocaleSettings.basePath + "i18n" + LocaleSettings.DIR_SEP + LocaleSettings.localePath + LocaleSettings.DIR_SEP;

            // if a wordslist file exists at the given location, all good
            if (SetLocalpathIfWordslistExists(_localeSettings.localePath))
            // if (IsValidLocale(LocaleSettings.language))
            {
                return;
            }

            // otherwise call InitLocalePrefix
            InitLocalePrefix("");

        }


        /// <summary> Parses the command line arguments if any set the language if available.
        ///  This is used to location the wordlist and other localized resources.
        ///  Sets the global variable 'language' if command line argument was valid.
        ///  otherwise calls the InitLocalPrefix
        /// </summary>
        /// <param name="args">the command line arguments passed to the application</param>
        /// <returns>Nothing</returns>
        internal void InitLocale(string[] args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-h":
                        case "--help":
                            // Console.WriteLine(GameConstants.HELP);
                            _logger.LogInformation(GameConstants.HELP);
                            Environment.Exit(0);
                            break;
                        case "-v":
                        case "--version":
                            // Console.WriteLine(GameConstants.VERSION);
                            _logger.LogInformation(GameConstants.VERSION);
                            Environment.Exit(0);
                            break;
                        case "-l":
                        case "--locale":
                            // Console.WriteLine("here " +  args[i] + " " + args[i + 1]);
                            _logger.LogInformation("here " + args[i] + " " + args[i + 1]);
                            if (args[i + 1] != null && GameConstants.AllowedLanguages.Contains(args[i + 1]))
                            {
                                _commandLineLocale = args[i + 1];
                                // LocaleSettings.localePath = args[i + 1];
                                break;
                            }
                            else
                            {
                                // Console.WriteLine("A valid language code needs to be provided");
                                _logger.LogWarning("A valid language code needs to be provided");
                                Environment.Exit(0);
                                break;
                            }
                        default:
                            break;
                    }
                }

                // localePath = args[1].Trim();
                // TODO: remove any trailing or leading directory separators and mode more checks in general
                _localeSettings.localePath = Path.Join(_localeSettings.basePath, _localeSettings.i18nPath, _localeSettings.language);
                // LocaleSettings.language += LocaleSettings.basePath + "i18n" + LocaleSettings.DIR_SEP + LocaleSettings.localePath + LocaleSettings.DIR_SEP;
                // if (IsWordslistAtLocale(LocaleSettings.language))
                if (SetLocalpathIfWordslistExists(_localeSettings.localePath))
                {
                    return;
                }
            }

            InitLocalePrefix("");
        }


        /// <summary> Attempt to get the locale language from the OS. 
        ///  This is used to location the wordlist and other localized resources.</summary>
        /// <param name="prefix">a path prefix</param>
        internal void InitLocalePrefix(string prefix)
        {
            // prefix is the local folder where the language specifics are stored (ie in addition to "i18n/")

            // Extract the language from the OS
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string culture = currentCulture.Name; // eg en-GB

            // No longer needed with Path.Join
            // LocaleSettings.localePath = culture + LocaleSettings.DIR_SEP;

            // LocaleSettings.language = prefix;

            // // language += basePath + "i18n" + DIR_SEP + culture;
            // LocaleSettings.language += LocaleSettings.i18nPath + LocaleSettings.DIR_SEP + culture;

            _localeSettings.localePath = Path.Join(prefix, _localeSettings.i18nPath, culture);

            // if (IsWordslistAtLocale(LocaleSettings.language))
            if (SetLocalpathIfWordslistExists(_localeSettings.localePath))
            {
                return;
            }

            // Sometimes currentCulture.Name returns a culture with a dot, like "en-GB.UTF-8"
            int lastIndexOfDot = culture.LastIndexOf('.');
            if (lastIndexOfDot != -1)
            {
                culture = culture[..lastIndexOfDot];
                // LocaleSettings.language += LocaleSettings.i18nPath + LocaleSettings.DIR_SEP + culture;
                _localeSettings.localePath = Path.Join(prefix, _localeSettings.i18nPath, culture);
                // language += basePath + "i18n" + DIR_SEP + culture;
                if (SetLocalpathIfWordslistExists(_localeSettings.localePath))
                // if (IsWordslistAtLocale(LocaleSettings.language))
                {
                    // LocaleSettings.localePath = culture + LocaleSettings.DIR_SEP;
                    return;
                }
            }

            // Sometimes currentCulture.Name returns a culture with an underscore, like "en_GB"
            int lastIndexOfUnderscore = culture.LastIndexOf('_');
            if (lastIndexOfUnderscore != -1)
            {
                culture = culture[..lastIndexOfUnderscore];
                // language += basePath + "i18n" + DIR_SEP + culture;
                _localeSettings.localePath = Path.Join(prefix, _localeSettings.i18nPath, culture);
                // LocaleSettings.language += LocaleSettings.i18nPath + LocaleSettings.DIR_SEP + culture;
                // if (SetLocalpathIfWordslistExists(LocaleSettings.language))
                if (SetLocalpathIfWordslistExists(_localeSettings.localePath))
                {
                    // LocaleSettings.localePath = culture + LocaleSettings.DIR_SEP;
                    return;
                }
            }

            // TODO: If there is a problem with windows, check the windows specific implementation in the original C app

            // last resort: use the default locale
            // NEW: no longer needed with Path.Join
            // LocaleSettings.language = prefix;
            // if (LocaleSettings.language == "")
            // {
            //     // language += DIR_SEP;
            // }
            // else if ((LocaleSettings.language[0] != 0) && (LocaleSettings.language[LocaleSettings.language.Length - 1] != LocaleSettings.DIR_SEP))
            // {
            //     // TODO: check if this is the most portable way to do this
            //     // language += '/';
            //     LocaleSettings.language += LocaleSettings.DIR_SEP;
            // }

            _localeSettings.localePath += _localeSettings.DEFAULT_LOCALE_PATH;
            // LocaleSettings.language += LocaleSettings.DEFAULT_LOCALE_PATH;

            if (!SetLocalpathIfWordslistExists(_localeSettings.localePath))
            // if (!SetLocalpathIfWordslistExists(LocaleSettings.language))
            {
                // Console.WriteLine($"InitLocalePrefix Error: could not find wordlist.txt at location {_localeSettings.localePath}");
                _logger.LogError($"InitLocalePrefix Error: could not find wordlist.txt at location {_localeSettings.localePath}");
                // TODO: Terminate better?
                Environment.Exit(1);
            }

            // return SetLocalpathIfWordslistExists(LocaleSettings.language);
        }


        /// <summary> not used. Used for Gamerzilla, which is not implemented here</summary>
        /// <returns>Nothing</returns>
        internal void GetUserPath()
        {
            string? env = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (env == null)
            {
                env = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
                // env = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + LocaleSettings.DIR_SEP + ".local" + LocaleSettings.DIR_SEP + "share" + LocaleSettings.DIR_SEP;
            }

            // TODO: there seems to be a double "env" below, check if this is correct
            _localeSettings.userPath = Path.Join(env, "anagramarama");
            // LocaleSettings.userPath = env + env + LocaleSettings.DIR_SEP + "anagramarama" + LocaleSettings.DIR_SEP;
        }


        /// <summary> Check that the dictionary exists in the specified local language </summary>
        /// <param name="path">The path, including the locale, to the wordfile in the desired language</param>
        /// <returns>true if the file exists otherwise false</returns>
        internal bool SetLocalpathIfWordslistExists(string path)
        {
            // No longer needed when using Path.Join
            // string filePath = path;
            // if (filePath[filePath.Length - 1] != LocaleSettings.DIR_SEP)
            // {
            //     filePath += LocaleSettings.DIR_SEP;
            // }
            // filePath += "wordlist.txt";

            string filePath = Path.Join(path, LocaleConstants.WordslistFolderName, LocaleConstants.WordslistDefaultName);
            if (File.Exists(filePath))
            {
                _localeSettings.wordslistPath = filePath;
                // _logger.LogDebug($"wordslistPath set to: {_localeSettings.wordslistPath}");
            }
            else
            {
                // _logger.LogError($"wordslist.txt not found at {filePath}");
            }
            return File.Exists(filePath);
        }
    }
}