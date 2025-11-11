using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Net.Http;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace WordslistAnalyser
{

    /// <summary> Handles loading and managing word frequency data. </summary>
    public class WordFrequencyLoader
    {
        const string FREQUENCY_URL = "https://raw.githubusercontent.com/hermitdave/FrequencyWords/master/content/2018/fr/fr_50k.txt";
        static string CACHE_FILE = "french_frequency.txt";

        /// <summary> Returns word frequency data, downloading and caching if necessary. </summary>
        /// <param name="cache_dir">location of the cache file</param>
        /// <returns>A dictionary of the words and their frequencies in that language</returns>
        public static Dictionary<string, float> LoadFrequencyData(string cache_dir)
        {
            string cache_path = Path.Join(cache_dir, CACHE_FILE);

            if (!Path.Exists(cache_path))
                DownloadFrequenciesToCache(cache_path);

            if (File.Exists(cache_path))
                return LoadFrequenciesFromCache(cache_path);
            else
                Console.WriteLine("Error: No frequency data available");
            return new Dictionary<string, float>();
        }

        /// <summary> Returns words frequency data from a cached file </summary>
        /// <param name="cache_path"> location of the cache file </param>
        /// <returns>A dictionary of the words and their frequencies in that language</returns>
        static Dictionary<string, float> LoadFrequenciesFromCache(string cache_path)
        {
            Dictionary<string, float> frequencies = [];
            using StreamReader sr = new(cache_path);
            if (sr == null)
                return frequencies;

            try
            {
                string? currentWordFrequency = sr.ReadLine();
                while (currentWordFrequency != null)
                {
                    string[] cwf = currentWordFrequency.Trim().Split(" ");
                    frequencies.Add(cwf[0], float.Parse(cwf[1]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e} trying to load the words frequencies from the cached file");
            }

            return frequencies;
        }


        /// <summary> Downloads word frequency data from a URL and saves it to a cache file. </summary>
        /// <param name="cache_path">The cache file to save to</param>
        static async void DownloadFrequenciesToCache(string cache_path)
        {
            string response;
            HttpClient client = new HttpClient();
            try
            {
                response = await client.GetStringAsync(FREQUENCY_URL);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e} trying to download the words frequencies from the URL");
                return;
            }

            using StreamWriter sw = new(cache_path);
            try
            {
                sw.Write(response);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e} trying to write the words frequencies to the cache file");
                return;
            }
        }

    }

    public class WordsAnalyser
    {
        const int MIN_WORD_LENGTH = 3;
        const int MAX_WORD_LENGTH = 7;
        const bool REMOVE_ACCENTS = true;

        /// <summary> Remove accents in words.
        /// NOTE: Not well tested, might not work well in a few languages?
        /// </summary>
        /// <param name="word"> the word from which to strip the accents </param>
        /// <returns> the word without the accents </returns>
        public static string RemoveAccents(string word)
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(word);
            string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            return asciiStr;
        }

        /// <summary> Return the word in lower case.
        /// Hopefully works well for multiple cultures
        /// </summary>
        /// <param name="word"> The word to lower case </param>
        /// <returns> The word lower cased </returns>
        public static string LowerCaseWord(string word)
        {
            return word.ToLowerInvariant();
        }

        /// <summary> Confirms that the word length is within range for the game </summary>
        /// <param name="word"> The word to check </param>
        /// <returns> true if the word length is between (incl) MIN_ and MAX_WORD_LENTH </returns>
        public static bool HasValidLength(string word)
        {
            return MIN_WORD_LENGTH <= word.Length && word.Length <= MAX_WORD_LENGTH;
            // for .net9+: return word.Length is >= MIN_WORD_LENGTH and <= MAX_WORD_LENGTH;
        }

        /// <summary> Confirms that the word is composed only of letters </summary>
        /// <param name="word"> The word to check </param>
        /// <returns> true if contains only letters </returns>
        public static bool ContainsOnlyLetters(string word)
        {
            return word.All(char.IsAsciiLetter);
        }

        /// <summary> Re-create the words list ensuring words of only letters, lower case, no accent (if desired) </summary>
        /// <param name="raw_wordslist"> The originally loaded list </param>
        /// <returns> The list to be used </returns>
        public static Dictionary<string, float> NormaliseWordsList(Dictionary<string, float> raw_wordslist)
        {
            Dictionary<String, float> normalisedWordslist = [];
            string this_word;

            foreach ((string word, float frequency) in raw_wordslist)
            {
                if (!ContainsOnlyLetters(word)) continue;
                if (!HasValidLength(word)) continue;
                this_word = LowerCaseWord(word);
                if (REMOVE_ACCENTS) this_word = RemoveAccents(this_word);

                if (normalisedWordslist.ContainsKey(this_word))
                {
                    normalisedWordslist[this_word] += frequency;
                }
                else
                {
                    normalisedWordslist.Add(this_word, frequency);
                }
            }

            return normalisedWordslist;
        }

        /// <summary> Returns a string with the word's letter sorted alphabetically </summary>
        /// <param name="word"> The word string to sort alphabetically </param>
        /// <returns> the string sorted alphabetically </returns>
        public static string GetAnagramKey(string word)
        {
            char[] characters = [];

            if (word != null)
            {
                characters = word.ToCharArray();
                Array.Sort(characters);
            }
            return new string(characters);
        }



    }

    internal static class WordslistAnalyser
    {
        internal static int Main()
        {
            Dictionary<string, float> wordsFrequencyData = [];
            WordFrequencyLoader wfl = new();
            wordsFrequencyData = WordFrequencyLoader.LoadFrequencyData(".");

            foreach ((string word, float frequency) in wordsFrequencyData)
            {
                Console.WriteLine($"{word}: {frequency}");
            }
            return 0;
        }        
    }

}
