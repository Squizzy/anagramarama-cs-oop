using Microsoft.Extensions.Logging;

namespace AgOop
{

    /// <summary> Constants used for creating and processing the anagrams </summary>
    internal static class AnagramsConstants
    {
        /// <summary>Represents the space character used in the program.</summary>
        internal const char SPACE_CHAR = '#';

        /// <summary>Represents the ASCII summary for the space character.</summary>
        internal const int ASCII_SPACE = 32;

        /// <summary>Represents a string filled with space characters.</summary> 
        internal const string SPACE_FILLED_CHARS = "#######";

        /// <summary> Represents an array of space characters. </summary>
        internal static readonly string SPACE_FILLED_STRING = new(SPACE_CHAR, 7);

        /// <summary>the size of the biggest word to be used in the game</summary>
        internal const int MAX_ANAGRAM_LENGTH = 7;
    }

    /// <summary> Manages the Anagrams activity, inc checking if found, moving letters... </summary>
    internal class AnagramsManager
    {
        private readonly ILogger<AnagramsManager> _logger;
        // internal LocaleManager? _localeManager;
        private readonly LocaleManager _localeManager;

        public AnagramsManager(ILogger<AnagramsManager> logger, LocaleManager localemanager)
        {
            _logger = logger;
            _localeManager = localemanager;
        }

        internal void AnagramsManagerExit(ref Sprite? letters, ref Anagrams.Node? headNode)
        {
            // Console.WriteLine("AnagramsManager Destructor");
            _logger.LogInformation("AnagramsManager Destructor");
            // DestroyLetters(ref letters);
            DestroyAnswers(ref headNode);
        }

        // private string _rootword;
        // private Anagrams.Node _anagramsList;
        // private int bigWordLength;


        /// <summary> returns the number of anagrams from the root word </summary>
        /// <param name="headNode">pointer to the first node</param>
        /// <returns> integer value of the number of anagrams in the list </returns>
        internal int Length(Anagrams.Node? headNode)
        {
            Anagrams.Node? current = headNode;
            int count = 0;

            while (current != null)
            {
                ++count;
                current = current.next;
            }
            return count;
        }


        /// <summary>swap the content from two linkedlist nodes without changing the position of the node 
        /// This is used when sorting the list alphabetically and by anagram's length</summary>
        /// <param name="fromNode">first node (in/out)</param>
        /// <param name="toNode">second node (in/out)</param>
        /// <returns>Nothing</returns>
        internal void Swap(ref Anagrams.Node fromNode, ref Anagrams.Node toNode)
        {
            string? word = fromNode.anagram;
            int len = fromNode.length;

            fromNode.anagram = toNode.anagram;
            fromNode.length = toNode.length;
            toNode.anagram = word;
            toNode.length = len;
        }


        /// <summary>Sort the anagrams list first alphabetically then by increasing word length</summary>
        /// <param name="headNode">the node head (in/out)</param>
        /// <returns>Nothing</returns>
        internal void Sort(ref Anagrams.Node headNode)
        {
            Anagrams.Node? left, right;
            bool completed = false;

            while (!completed)
            {
                left = headNode;
                right = left.next;
                completed = true;
                while ((left != null) && (right != null))
                {
                    if (string.Compare(left.anagram, right.anagram) > 0)
                    {
                        Swap(ref left, ref right);
                        completed = false;
                    }
                    left = left.next;
                    right = right.next;
                }
            }

            completed = false;
            while (!completed)
            {
                left = headNode;
                right = left.next;
                completed = true;
                while ((left != null) && (right != null))
                {
                    if (left.length > right.length)
                    {
                        Swap(ref left, ref right);
                        completed = false;
                    }
                    left = left.next;
                    right = right.next;
                }
            }
        }


        /// <summary>Resets the linkedlist of the anagrams from the root word
        /// unlike with C, the garbage collector of C# will take care of reclaiming the memory
        /// So no individual node clearing is needed </summary>
        /// <param name="headNode">the head node</param>
        /// <returns>Nothing</returns>
        internal void DestroyAnswers(ref Anagrams.Node? headNode)
        {
            headNode = null;
        }


        /// <summary>Add a new word at the front of the linkedlist of anagrams 
        /// as long as it is not a duplicate </summary>
        /// <param name="headNode"></param>
        /// <param name="new_anagram">The word to add to the list</param>
        /// <returns>Nothing as the linkedlist itself is modified</returns>
        internal void Push(ref Anagrams.Node? headNode, string new_anagram)
        {
            Anagrams.Node? current = headNode;
            bool duplicate = false;

            // check if the word is already in the list
            while (current != null)
            {
                if (string.Equals(new_anagram, current.anagram))
                {
                    duplicate = true;
                    break;
                }
                current = current.next;
            }

            if (!duplicate)
            {
                Anagrams.Node newNode = new Anagrams.Node(new_anagram);
                newNode.next = headNode;
                headNode = newNode;
            }
        }


        // TODO: no need to use the +1
        /// <summary>Returns the first blank space in a string (end of a word).
        /// blanks are indicated by '#' (SPACE_CHAR) not ' ' (ASCII_SPACE)</summary>
        /// <param name="thisString">the string to analyse</param>
        /// <returns>returns position of next blank (1 is first character) or 0 if no blanks found</returns>
        internal int NextBlank(string thisString)
        {
            // +1 is needed to align with the 1-position of first character in original C application
            return thisString.IndexOf(AnagramsConstants.SPACE_CHAR) + 1;
        }


        /// <summary>Shift a string one character to the left, truncating the leftmost character</summary>
        /// <param name="thisString"></param>
        /// <returns>the string less its first character</returns>
        internal string ShitfLeftKill(string thisString)
        {
            return thisString[1..];
        }


        /// <summary>Move the first character to the end of the string</summary>
        /// <param name="thisString"></param>
        /// <returns></returns>
        internal string ShiftLeft(string thisString)
        {
            return thisString[1..] + thisString[..1];
        }


        /// <summary>Generate all possible valid anagrams using the root word letters
        /// the initial letter is fixed, so to work out all anagrams of a word, prefix with space.
        /// </summary>
        /// <param name="headNode">The head node of the anagrams list(in/out)</param>
        /// <param name="dlbHeadNode">The head node of the dictionary</param>
        /// <param name="guess">the current guess</param>
        /// <param name="remain">the remaining letters</param>
        /// <returns>Nothing, the anagram's linkedlist is updated by reference</returns>
        internal void Ag(ref Anagrams.Node? headNode, WordsList.Dlb_node? dlbHeadNode, string guess, string remain)
        {
            string newGuess = guess;
            string newRemain = remain;

            int guessLen = guess.Length;
            int remainLen = remain.Length;
            int totalLen = guessLen + remainLen;

            // add the element at last position of newRemain to newGuess
            newGuess += newRemain[^1..];

            // remove the last element of newRemain
            newRemain = newRemain[..^1];

            //  If the newGuess word is more than 3 char
            if (newGuess.Length > 3)
            {
                // Shift its letters left dropping the first one
                string shiftLeftKilledString = ShitfLeftKill(newGuess);

                // If this is a word in the dictionary, add it to the anagrams linkedlist
                if (WordsList.DlbLookup(dlbHeadNode, shiftLeftKilledString))
                {
                    Push(ref headNode, shiftLeftKilledString);
                }
            }

            if (newRemain.Length > 0)
            {
                // Recursively check other words
                Ag(ref headNode, dlbHeadNode, newGuess, newRemain);

                // Then for all the total letters
                for (int i = totalLen - 1; i > 0; i--)
                {
                    // recursively try all the combinations of newRemain letters with the guess
                    if (newRemain.Length > i)
                    {
                        newRemain = ShiftLeft(newRemain);
                        Ag(ref headNode, dlbHeadNode, newGuess, newRemain);
                    }
                }
            }
        }


        /// <summary> Get a random word in the dictionary txt file (not the dlb dictionary linked list)
        /// [note from original C file] spin the word file to a random location and then loop until a 7 or 8 letter word is found
        /// This is quite a weak way to get a random word considering we've got a nice dbl Dictionary to hand - 
        /// but it works for now.
        /// </summary>
        /// <param name="randomWord">the referenced random word variable (will contain the random word found)</param>
        /// <param name="randomWordMinLength">The min length desired for the random word (7 or 8 in the original game)</param>
        /// <returns>Nothing as the result is passed by reference</returns>
        internal void GetRandomWord(ref string randomWord, int randomWordMinLength)
        {
            string filename = _localeManager.language + "wordlist.txt";

            string[] lines = File.ReadAllLines(filename);

            // only use the words that are at randomWordMinLength length
            lines = lines.Where(line => line.Length == randomWordMinLength).ToArray();

            int lineCount = lines.Length;

            Random rnd = new Random();

            randomWord = lines[rnd.Next(lineCount)];

            // Extra space as needed by the Ag function to easily rotate chars
            randomWord += ' ';
        }


        /// <summary> Shuffles the characters in a given word array. </summary>
        /// <param name="word">The word array to be shuffled.</param>
        /// <remarks>
        /// This method generates a random number between 20 and 26, and then swaps two characters in the word array
        /// for the generated number of times. The characters to be swapped are randomly selected using the Random class.
        /// </remarks>
        /// <returns>Nothing, the word is passed by reference</returns>
        internal void ShuffleWord(ref char[] word)
        {
            int a, b;
            char tmp;
            Random random = new Random();

            // generate a random number between 20 and 26. The rand() function in C no longer exists in C#
            // This was done in the initial c app, not sure why. Probably to increase the randomness result?
            int count = random.Next(20, 27);

            for (int n = 0; n < count; n++)
            {
                a = random.Next(0, 7);
                b = random.Next(0, 7);
                tmp = word[a];
                word[a] = word[b];
                word[b] = tmp;
            }
        }


        /// <summary>Returns the index of first occurrence of a specific letter in a string.</summary>
        /// <param name="word">The word to check</param>
        /// <param name="letter">The char to find in the word</param>
        /// <returns>The position of the letter if it is found or -1 if not</returns>
        internal int WhereInString(string word, char letter)
        {
            int pos = word.IndexOf(letter);
            return pos == -1 ? 0 : pos;
        }


        /// <summary> shuffle the anagram letter nodes from the available letters in the LETTER box </summary>
        /// <param name="word">The word to shuffle</param>
        /// <param name="letters">The sprite letters to shuffle at the same time</param>
        /// <returns>Nothing as passed by reference</returns>
        internal void ShuffleAvailableLetters(ref string word, ref Sprite? letters)
        {
            Sprite? thisLetter = letters;
            Random random = new Random();
            int from, to;

            /// <summary> Switches two chars in a string </summary>
            void CharsSwap(ref string charsSeq, int from, int to)
            {
                // make sure from is the lowest value
                if (from > to) (from, to) = (to, from);
                if (to != from)
                {
                    string tempString = "";
                    tempString = charsSeq[0..from] + charsSeq[to] + charsSeq[(from + 1)..to] + charsSeq[from] + charsSeq[(to + 1)..];
                    charsSeq = tempString;
                }
            }

            char[] shuffleCharArray = word.ToCharArray();
            string shuffleChars = new string(shuffleCharArray);
            string shufflePos = "0123456";
            int numSwaps = random.Next(20, 30);

            for (int i = 0; i < numSwaps; i++)
            {
                from = random.Next(0, 6);
                to = random.Next(0, 6);

                CharsSwap(ref shuffleChars, from, to);
                CharsSwap(ref shufflePos, from, to);
            }

            while (thisLetter != null)
            {
                if (thisLetter.box == BoxConstants.SHUFFLE)
                {
                    int positionInString = WhereInString(new string(shufflePos), (char)(thisLetter.index + SpriteConstants.NUM_TO_CHAR));
                    thisLetter.toX = positionInString * (SpriteConstants.GAME_LETTER_WIDTH + SpriteConstants.GAME_LETTER_SPACE)
                                     + BoxConstants.BOX_START_X;
                    thisLetter.index = positionInString;
                }
                thisLetter = thisLetter.next;
            }
            word = new string(shuffleChars);
        }


        /// <summary> method to display the list of anagrams to be played, in the console
        /// Used for debug if needed</summary>
        /// <param name="headNode">the first node in the list of anagrams</param>
        /// <returns>Nothing</results>
        internal void ListHead(Anagrams.Node headNode)
        {
            Anagrams.Node? current = headNode;
            while (current != null)
            {
                current = current.next;
            }
        }

        /// <summary> Declare all the anagrams as found (but not necessarily guessed)</summary>
        /// <param name="headNode">The head node of the anagrams list</param>
        /// <returns>Nothing</returns>
        internal void SolveIt(Anagrams.Node? headNode)
        {
            Anagrams.Node? current = headNode;

            while (current != null)
            {
                current.found = true;
                current = current.next;
            }
        }

        /// <summary> Check if the guess is in the list of anagrams and if it has been found before</summary>
        /// <param name="answer">the word proposed</param>
        /// <param name="headNode">The head node of anagrams</param>
        /// <returns >
        /// isInList: The anagram is in the list to be found
        /// wasAlreadyFound: The anagram was already found
        /// lengthFound: The length of the anagram found
        /// </returns>
        internal (bool isInList, bool wasAlreadyFound, int lengthFound) IsInAnagramsList(string answer, Anagrams.Node? headNode)
        {
            Anagrams.Node? current = headNode;
            bool isInList = false;
            bool wasAlreadyFound = false;
            int lengthFound = 0;

            string test;
            int len = NextBlank(answer) - 1;
            if (len == -1) len = answer.Length;
            test = answer[0..len];

            while (current != null)
            {
                if (current.anagram == test)
                {
                    isInList = true;
                    wasAlreadyFound = current.found;
                    lengthFound = current.length;
                    current.found = true;
                    break;
                }
                current = current.next;
            }
            return (isInList, wasAlreadyFound, lengthFound);
        }

        /// <summary> Do all of the initialisation for a new game:
        /// build the screen
        /// get a random word and generate anagrams
        /// (must get less than 66 anagrams to display on screen)
        /// initialise all the game control flags
        /// </summary>
        /// <param name="headNode">first node in the answers list (in/out)</param>
        /// <param name="dlbHeadNode">first node in the dictionary list</param>
        /// <param name="screen">SDL_Surface to display the image</param>
        /// <param name="letters">first node in the letter sprites (in/out)</param>
        /// <returns>Nothing</returns>
        internal void GetNewRootWordAndAnagramsList(ref Anagrams.Node? headNode, WordsList.Dlb_node? dlbHeadNode)
        {
            string guess;
            string remain = "";

            // happy is true if we have < 67 anagrams and => 6
            bool happy = false;

            // DestroyLetters(ref letters);
            // AnagramsManager.DestroyLetters(ref letters);

            while (!happy)
            {
                // changed this max size from original game
                GetRandomWord(ref GameState.rootword, AnagramsConstants.MAX_ANAGRAM_LENGTH);
                // GetRandomWord adds an extra space at the end (legacy from C) so we remove it
                GameState.bigWordLen = GameState.rootword.Length - 1;
                guess = "";
                remain = GameState.rootword;

                DestroyAnswers(ref headNode);

                // Call the recursive Ag method that will identify all the 
                Ag(ref headNode, dlbHeadNode, guess, remain);

                GameState.answersSought = this.Length(headNode);

                happy = (GameState.answersSought <= 77) && (GameState.answersSought >= 6);
            }

            // now we have a good set of words - sort them alphabetically and by size
            this.Sort(ref headNode!);
        }

        //     // if the big word is less than 7 chars, fill in the rest with spaces
        //     for (int i = GameState.bigWordLen; i < 7; i++)
        //     {
        //         remain = remain[0..(i - 1)] + AnagramsConstants.SPACE_CHAR;
        //     }

        //     // remain should still contain the root word... not sure why the original game didn't pass that
        //     // making sure the remain string is exactly 7 chars
        //     remain = remain[0..7]; 
        //     // Original game remnant - convert to char array 
        //     char[] remainToShuffle = remain.ToCharArray();
        //     ShuffleWord(ref remainToShuffle);
        //     GameState.Shuffle = new string(remainToShuffle);

        //     GameState.Answer = AnagramsConstants.SPACE_FILLED_CHARS;
        // }

        internal string GetInitialShuffle()
        {
            string remain = GameState.rootword;

            for (int i = GameState.bigWordLen; i < 7; i++)
            // for (int i = bigWordLen; i < 7; i++)
            {
                remain = remain[0..(i - 1)] + AnagramsConstants.SPACE_CHAR;
            }
            remain = remain[0..7]; // making sure we don't have extra chars
            char[] remainToShuffle = remain.ToCharArray();
            // AnagramsManager.ShuffleWord(ref remainToShuffle);
            ShuffleWord(ref remainToShuffle);
            return new string(remainToShuffle);
        }


    }
}