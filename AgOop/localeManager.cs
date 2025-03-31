using System.Globalization;

namespace AgOop
{

    internal class LocaleManager
    {
        /// <summary>Path to the locale dictionary </summary>
        // public const string DEFAULT_LOCALE_PATH = "i18n/en_GB";
        internal static string DEFAULT_LOCALE_PATH = "i18n" + DIR_SEP + "en_GB" + DIR_SEP;

        /// <summary>Directory separator for the OS used </summary>
        internal static char DIR_SEP = Path.DirectorySeparatorChar;

        /// <summary>Subdirectory where the audio is stored</summary>
        internal static string audioSubPath = "audio" + DIR_SEP;

        /// <summary>Subdirectory for internationalised content</summary>
        internal static string i18nPath = "i18n" + DIR_SEP;

        /// <summary>Subdirectory where the audio is stored</summary>
        internal static string imagesSubPath = "images" + DIR_SEP;

        /// <summary>Subdirectory where the internationalised content is stored (dict, images)</summary>
        internal static string localePath = "";

        /// <value>the path to the locale language to use for the game (where the worldlist.txt is)</value>
        internal static string language = "";

        /// <value>the path to the locale data to use for the game. Used for Gamerzilla, which is not implemented here</value>
        /// https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/
        internal static string userPath = "";

        /// <value>the base path for the game. Used for Gamerzilla, which is not implemented here</value>
        internal static string basePath = "";

        internal LocaleManager(string[] args)
        {
            GetBasePath();

            InitLocale(args);

            if (language[language.Length - 1] != DIR_SEP)
            {
                language += DIR_SEP;
            }
        }

        /// <summary>Get the current language string from the environment. 
        ///  This is used to location the wordlist and other localized resources.
        ///  Sets the global variable 'language' 
        ///  defaults to the default value
        /// </summary>
        /// <param name="prefix">a path prefix</param>
        /// <returns>true if language was set to a value that had valid "wordslist.txt" else false</returns>
        internal static bool InitLocalePrefix(string prefix)
        {
            // prefix is the local folder where the language specifics are stored (ie in addition to "i18n/")
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            string culture = currentCulture.Name; // eg en-GB
            localePath = culture + DIR_SEP;

            language = prefix;

            language += basePath + "i18n" + DIR_SEP + culture;
            if (IsValidLocale(language))
            {
                return true;
            }

            int lastIndexOfDot = culture.LastIndexOf('.');
            if (lastIndexOfDot != -1)
            {
                culture = culture[..lastIndexOfDot];
                language += basePath + "i18n" + DIR_SEP + culture;
                if (IsValidLocale(language))
                {
                    localePath = culture + DIR_SEP;
                    return true;
                }
            }

            int lastIndexOfUnderscore = culture.LastIndexOf('_');
            if (lastIndexOfUnderscore != -1)
            {
                culture = culture[..lastIndexOfUnderscore];
                language += basePath + "i18n" + DIR_SEP + culture;
                if (IsValidLocale(language))
                {
                    localePath = culture + DIR_SEP;
                    return true;
                }
            }

            // TODO: If there is a problem with windows, check the windows specific implementation in the original C app

            // last resort: use the default locale
            language = prefix;
            if (language == "")
            {
                language += DIR_SEP;
            }
            else if ((language[0] != 0) && (language[language.Length - 1] != DIR_SEP))
                {
                    // TODO: check if this is the most portable way to do this
                    // language += '/';
                    language += DIR_SEP;
                }
            language += DEFAULT_LOCALE_PATH;

            return IsValidLocale(language);
        }


        /// <summary>set the language string using command line argument if available.
        ///  This is used to location the wordlist and other localized resources.
        ///  Sets the global variable 'language' if command line argument was valid.
        ///  otherwise calls the InitLocalPrefix
        /// </summary>
        /// <param name="args">the command line arguments passed to the application</param>
        /// <returns>Nothing</returns>
        internal static void InitLocale(string[] args)
        {
            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-h":
                        case "--help":
                            Console.WriteLine(GameConstants.HELP);
                            Environment.Exit(0);
                            break;
                        case "-v":
                        case "--version":
                            Console.WriteLine(GameConstants.VERSION);
                            Environment.Exit(0);
                            break;
                        case "-l":
                        case "--locale":
                            Console.WriteLine("here " +  args[i] + " " + args[i + 1]);
                            if (args[i + 1] != null && GameConstants.AllowedLanguages.Contains(args[i + 1]))
                            {
                                localePath = args[i + 1];
                                break;
                            }
                            else
                            {
                                Console.WriteLine("A valid language code needs to be provided");
                                Environment.Exit(0);
                                break;
                            }
                        default: 
                            break;
                    }
                }

                // localePath = args[1].Trim();
                // TODO: remove any trailing or leading directory separators and mode more checks in general
                language += basePath + "i18n" + DIR_SEP + localePath + DIR_SEP;
                if (IsValidLocale(language))
                {
                    return;
                }
            }

            InitLocalePrefix("");
        }


        /// <summary> not used. Used for Gamerzilla, which is not implemented here</summary>
        /// <returns>Nothing</returns>
        internal static void GetUserPath()
        {
            string? env = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (env == null)
            {
                env = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + DIR_SEP + ".local" + DIR_SEP + "share" + DIR_SEP;
            }

            userPath = env + env + DIR_SEP + "anagramarama" + DIR_SEP;
        }


        /// <summary> not used.</summary>
        /// <returns>Nothing</returns>
        internal static void GetBasePath()
        {
            string? env = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (env == null)
            {
                // env = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.local/share/";
                env = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + DIR_SEP + ".local" + DIR_SEP + "share" + DIR_SEP;
            }

            basePath = env + DIR_SEP + "anagramarama" + DIR_SEP;
        }


        /// <summary> Check that the dictionary in the local language exists </summary>
        /// <param name="path">The path, including the locale, to the wordfile in the desired language</param>
        /// <returns>true if the file exists otherwise false</returns>
        internal static bool IsValidLocale(string path)
        {
            string filePath = path;
            if (filePath[filePath.Length - 1] != DIR_SEP)
            {
                filePath += DIR_SEP;
            }
            filePath += "wordlist.txt";

            return File.Exists(filePath);
        }



    }
}