using System.Text.RegularExpressions;

namespace AgOop
{

    internal class ConfigurationManager
    {
        private readonly LocaleSettings _localeSettings;
        private string configFilePath;

        internal ConfigurationManager(LocaleSettings localeSettings)
        {
            _localeSettings = localeSettings;

            // configFilePath = _localeManager.basePath + _localeManager.i18nPath + _localeManager.localePath;
            configFilePath = _localeSettings.localePath;
            // string configFilePath = LocaleManager.basePath + LocaleManager.i18nPath + LocaleManager.localePath;
            LoadConfig(configFilePath + "config.ini");
        }

        /// <summary>The name of the hotboxes in the config.ini file - used by Configuration</summary>
        internal static string[] boxnames = new[] {"solve", "new", "quit", "shuffle", "enter", "clear"};

        /// <summary> parse a config line eg: solve = 555 30 76 20
        /// Modified from the original to 
        ///  1) use regex
        ///  2) identify the box
        ///  3) make use of return value to apply the config or not (safer)
        /// </summary>
        /// <param name="line">the line to be parsed</param>
        /// <returns>true if the config was found and applied, otherwise false</returns>
        internal static (bool, int?, Box?) ConfigBox(string line)
        {
            string configRegex = @"^\b(?<box>(?i)solve|new|quit|shuffle|enter|clear(?-i))\b\s=\s(?<x>[0-9]{1,3})\s(?<y>[0-9]{1,3})\s(?<width>[0-9]{1,3})\s(?<height>[0-9]{1,3})";
            Regex rg = new Regex(configRegex);

            Match configMatch = rg.Match(line);

            if (configMatch.Success)
            {
                // Read the Box component from the configfile line
                string configHotboxName = configMatch.Groups["box"].ToString();

                // Check if that Box name is one of the ones in the application's definition (in "boxnames")
                int configHotboxIndex = Array.FindIndex(boxnames, x => x == configHotboxName);

                // If it isn't, exit
                if (configHotboxIndex == -1) // not a Hotbox config
                {
                    return (false, null, null);
                }

                // if it is, assign the regex match values to it, read the config into a new box
                Box configHotbox = new Box
                (
                    x: int.Parse(configMatch.Groups["x"].ToString()),
                    y: int.Parse(configMatch.Groups["y"].ToString()),
                    width: int.Parse(configMatch.Groups["width"].ToString()),
                    height: int.Parse(configMatch.Groups["height"].ToString())
                );

                // and send this value back (the in index of the name of the box and its config)
                return (true, configHotboxIndex, configHotbox);
            }

            // This line does not match the regex, return.
            return (false, null, null);
        }


        /// <summary> read any locale-specific configuration information from an ini file. 
        /// This can reconfigure the positions of the boxes to account for different word sizes 
        /// or alternative background layouts
        /// </summary>
        /// <param name="configFile"></param>
        internal static void LoadConfig(string configFile)
        {
            if (File.Exists(configFile))
            {
                using StreamReader sr = new StreamReader(configFile);
                string? line = sr.ReadLine();
                while (line != null)
                {
                    (bool isConfigBox, int? boxIndex, Box? boxDimensions) = ConfigBox(line);
                    if (isConfigBox && boxIndex != null && boxDimensions != null)
                    {
                       HotBoxes.hotbox[(int)boxIndex] = boxDimensions;
                    }
                }
            }
        }

    }
}