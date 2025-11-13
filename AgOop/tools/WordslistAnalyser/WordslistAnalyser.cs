using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Net.Http;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace WordslistAnalyser
{

    /// <summary> Handles loading and managing word frequency data. </summary>
    public class WordFrequenciesLoader
    {
        const string FREQUENCY_URL = "https://raw.githubusercontent.com/hermitdave/FrequencyWords/master/content/2018/fr/fr_50k.txt";
        static string CACHE_FILENAME = "french_frequency.txt";

        /// <summary> Returns word frequency data, downloading and caching if necessary. </summary>
        /// <param name="cache_dir">location of the cache file</param>
        /// <returns>A dictionary of the words and their frequencies in that language</returns>
        public static async Task<Dictionary<string, int>> LoadFrequencyDataAsync(string cache_dir)
        {
            string cache_file = Path.Join(cache_dir, CACHE_FILENAME);

            if (!Path.Exists(cache_file))
                await DownloadFrequenciesToCacheAsync(cache_file).ConfigureAwait(false);

            if (File.Exists(cache_file))
                return LoadFrequenciesFromCache(cache_file);
            else
                Console.WriteLine("Error: No frequency data available");
            return new Dictionary<string, int>();
        }

        /// <summary> Returns words frequency data from a cached file </summary>
        /// <param name="cache_path"> location of the cache file </param>
        /// <returns>A dictionary of the words and their frequencies in that language</returns>
        static Dictionary<string, int> LoadFrequenciesFromCache(string cache_path)
        {
            Dictionary<string, int> frequencies = [];
            using StreamReader sr = new(cache_path);
            if (sr == null)
                return frequencies;

            try
            {
                string? currentWordFrequency = sr.ReadLine();
                while (currentWordFrequency != null)
                {
                    string[] cwf = currentWordFrequency.Trim().Split(" ");
                    frequencies.Add(cwf[0], int.Parse(cwf[1]));
                    currentWordFrequency = sr.ReadLine();
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
        static async Task DownloadFrequenciesToCacheAsync(string cache_path)
        {
            string response;
            HttpClient client = new HttpClient();
            try
            {
                response = await client.GetStringAsync(FREQUENCY_URL).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e} trying to download the words frequencies from the URL");
                return;
            }
                      
            try
            {
                await File.WriteAllTextAsync(cache_path, response).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error {e} trying to write the words frequencies to the cache file");
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
            if (string.IsNullOrEmpty(word)) return word;

            return new string(word
                                .Normalize(NormalizationForm.FormD)
                                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                                .ToArray());
            
            // byte[] tempBytes;
            // tempBytes = System.Text.Encoding.UTF32.GetBytes(word);
            // string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);
            // return asciiStr;
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
            if (string.IsNullOrEmpty(word)) return false;

            return word.All(char.IsLetter);
        }

        /// <summary> Re-create the words list ensuring words of only letters, lower case, no accent (if desired) </summary>
        /// <param name="raw_wordslist"> The originally loaded list </param>
        /// <returns> The list to be used </returns>
        public static Dictionary<string, int> NormaliseWordsList(Dictionary<string, int> raw_wordslist)
        {
            Dictionary<string, int> normalisedWordslist = [];
            string this_word;

            foreach ((string word, int frequency) in raw_wordslist)
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

        public static void StoreWordsList(Dictionary<string, int> updatedWordsList)
        {
            
            FileStream stream = new FileStream("./uwl.txt", FileMode.Create);
            using StreamWriter sw = new StreamWriter(stream, encoding: Encoding.UTF8);

            foreach ((string word, int frequency) in updatedWordsList)
            {
                sw.WriteLine($"{word} {frequency}");
            }
        }

        public static void DisplayStatistics(Dictionary<string, int> wordsList)
        {
            List<int> wordsLengthCount = [0, 0, 0, 0, 0];
            int lowest_frequency = -1;
            int highest_frequency = -1;
            Dictionary<int, int> frequenciesList = [];

            foreach ((string word, int frequency) in wordsList)
            {
                wordsLengthCount[word.Length - 3] += 1;
                lowest_frequency = (lowest_frequency == -1)
                                        ? frequency
                                        : (frequency < lowest_frequency)
                                            ? frequency
                                            : lowest_frequency;
                highest_frequency = (highest_frequency == -1)
                                        ? frequency
                                        : (frequency > highest_frequency)
                                            ? frequency
                                            : highest_frequency;
                int frequencyThousands = Math.Abs(frequency / 1000);
                if (frequenciesList.ContainsKey(frequencyThousands))
                {
                    frequenciesList[frequencyThousands] += 1;
                }
                else
                {
                    frequenciesList.Add(frequencyThousands, 1);
                }
            }
            Console.WriteLine($"Words Length Count: 3: {wordsLengthCount[0]}, 4: {wordsLengthCount[1]}, 5: {wordsLengthCount[2]}, 6: {wordsLengthCount[3]}, 7: {wordsLengthCount[4]}");
            Console.WriteLine($"Lowest Freq: {lowest_frequency}, Highest Freq: {highest_frequency}");
            Console.WriteLine($"Number of frequencies:  {frequenciesList.Count}");

            FileStream stream = new FileStream("./frequenciesList.csv", FileMode.Create);
            using StreamWriter sw = new StreamWriter(stream, encoding: Encoding.UTF8);

            sw.WriteLine("\"frequency\",\"count\"");
            foreach ((int frequency, int count) in frequenciesList)
            {
                sw.WriteLine($"{frequency},{count}");
            }
        }

    }

    internal static class WordslistAnalyser
    {
        internal static async Task<int> Main()
        {
            Dictionary<string, int> wordsFrequencyData = [];
            Dictionary<string, int> gameProcessedData = [];

            // WordFrequenciesLoader wfl = new();
            wordsFrequencyData = await WordFrequenciesLoader.LoadFrequencyDataAsync(".");
            Console.WriteLine($"{wordsFrequencyData.Count}");


            gameProcessedData = WordsAnalyser.NormaliseWordsList(wordsFrequencyData);
            Console.WriteLine($"{gameProcessedData.Count}");
            WordsAnalyser.StoreWordsList(gameProcessedData);

            WordsAnalyser.DisplayStatistics(gameProcessedData);

            // foreach ((string word, int frequency) in wordsFrequencyData)
            // {
            //     Console.WriteLine($"{word}: {frequency}");
            // }
            return 0;
        }        
    }

}
