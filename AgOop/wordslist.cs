using Microsoft.Extensions.Logging;

namespace AgOop
{

    internal class WordsList
    {
        // private static readonly AgOopLogger logger = new("WordsList");
        private readonly ILogger<WordsList> _logger;
        private readonly LocaleSettings _localeSettings;

        /// <summary>Constructor for the wordlist</summary
        // internal WordsList(ILogger<WordsList> logger, ref Dlb_node? dlbHeadNode, string wordlist)
        public WordsList(ILogger<WordsList> logger, LocaleSettings localeSettings)
        {
            _logger = logger;
            _localeSettings = localeSettings;
        }

        // private Dlb_node _dictionary;

        // private void LoadDictionary()
        // {
        //     string wordsListPath = _localeSettings.wordslistPath;
        //     _dictionary = new Dlb_node('\0');

        // }

        internal void GenerateWordsList(ref Dlb_node? dlbHeadNode, string wordlist)
        {
            // Console.WriteLine("loading dictionary: " + wordlist);
            _logger.LogInformation($"loading dictionary: {wordlist}");
            if (!DlbCreate(ref dlbHeadNode, wordlist))
            {
                // Console.WriteLine("error leading the dictionary");
                _logger.LogError($"error leading the dictionary: {wordlist}");
                Console.ReadLine();
            }
        }

        internal void WordsListExit(ref Dlb_node? dlbHeadNode)
        {
            // Console.WriteLine("WordsList Destructor");
            _logger.LogInformation("WordsList Destructor called");
            DlbFree(ref dlbHeadNode);
        }

        #region overview of Dlb (de la Briandais Trie) information
        /*
        dbl_node contains the key building blocks ("node") for the dictionary that is loaded from the text file provided. 
        
        each node contains:
        - letter:  a letter that has been read from the dictionary file
        - valid:   the indication that this letter is the end of a word
        - child:   the link to the next "node" which, together with the previous nodes since the last valid, 
                and with all other children until the next valid, makes up a word
        - sibling: the link to the next "node" for a new word that uses the same letters that have been used until now, 
                with its letter replacing the current node's for that word. ie "branches out" into a new word

        e.g.
        "head node" (nothing) 
            -> child -
            -> sibling 'a'
                -> child 'b'
                    -> child 'b'
                        -> child 'a'
                            -> child 'c'
                                -> child 'i' 
                                    -> child -, valid -> word "abaci"
                                    -> sibling 'k', valid -> word "aback"
                                        -> child -
                                        -> sibling 'u'
                                            -> child s
                                                -> child -, sibling -, valid -> word "abacus"
                                            -> sibling -
                            -> sibling 'f'
                                -> child 't'
                                    -> child -, valid -> word "abaft"
                                
                -> sibling 'b' -> start of words starting with 'b'         
        */
        #endregion

        /// <summary> Node for the linkedlist containing all the possible words of the dictionary loaded for the game </summary>
        internal class Dlb_node
        {
            /// <summary> The letter in the word </summary>
            internal char letter { get; set; } = '\0';

            /// <summary> end of a valid word composed from the previous nodes </summary>
            internal bool valid { get; set; } = false;

            /// <summary> pointer a letter that belongs to a new word which shares the same initial letters until this point. </summary>
            internal Dlb_node? sibling;

            /// <summary> pointer to the next letter of the same word </summary>
            internal Dlb_node? child;

            /// <summary>Constructor</summary>
            internal Dlb_node(char c)
            {
                letter = c;
                valid = false;
                sibling = null;
                child = null;
            }
        }

       
        /// <summary> Delegate for operations on a Dlb_node. (for DLBWalk)
        /// Modified to return nothing as the original app did not make use of the return value anyway
        /// </summary>
        /// <param name="node">The Dlb_node to operate on.</param>
        /// <returns>Nothing</returns>
        public delegate void Dlb_node_operation(Dlb_node node);


        /// <summary>Constructor for Dlb_node node (as per C file)
        /// Creates a new Dlb_node with the specified character and initializes its properties. 
        /// Not really needed in c# but kept for sake of keeping...
        /// </summary>
        /// <param name="c">The character for the new node.</param>
        /// <returns>The newly created Dlb_node</returns>
        internal static Dlb_node DlbNodeCreateNode(char c)
        {
            Dlb_node newNode = new(c)
            {
                letter = c,
                valid = false,
                sibling = null,
                child = null
            };
            return newNode;
        }


        /// <summary> Destructor for a Dlb_node node. 
        /// Not really needed in c# but kept for sake of keeping...
        /// </summary>
        /// <param name="dlbHeadNode">The node to be cleared</param>
        /// <returns>Nothing</returns>
        internal static void DlbFreeNode(Dlb_node? dlbHeadNode)
        {
            dlbHeadNode = null;
        }


        /// <summary> Walk through the whole of the dictionary linkedlist and perform the op delegate on the node
        /// In this particular application, op is called to free the node
        /// This method just walks the whole linkedlist and clears all the nodes.
        /// Not really needed in c# but kept for sake of keeping...
        /// </summary>
        /// <param name="dlbHeadNode">The node to be freed</param>
        /// <param name="op">The method to be called to be applied to the node (in this app: free the memory)</param>
        /// <returns>Nothing</returns>
        internal static void DlbWalk(ref Dlb_node? dlbHeadNode, Dlb_node_operation op)
        {
            while (dlbHeadNode != null)
            {
                Dlb_node tempNode = dlbHeadNode;
                if (dlbHeadNode.child != null)
                {
                    DlbWalk(ref dlbHeadNode.child, op);
                }
                dlbHeadNode = dlbHeadNode.sibling;
                op(tempNode);
            }
        }


        /// <summary> Frees memory of all Dlb_node node of the words dictionary linked list
        /// Not really needed in c# but kept for sake of keeping...
        /// </summary>
        /// <param name="headNode">The headnode of the dictionary to clear</param>
        /// <returns>Nothing</returns>
        internal static void DlbFree(ref Dlb_node? headNode)
        {
            DlbWalk(ref headNode, DlbFreeNode);
        }

        
        /// <summary>add a new word to words dicionary linked list
        /// load a new word into the dictionary link list, taking into account children and siblings possibilities.
        /// </summary>
        /// <param name="dlbHeadNode">The head node of the dictionary</param>
        /// <param name="word">The word to insert in the linked list</param>
        /// <returns>Nothing</returns>
        internal static void DlbPush(ref Dlb_node? dlbHeadNode, string word)
        {
            Dlb_node? current = dlbHeadNode;
            Dlb_node previous = new Dlb_node('\0');
            string p = word;
            bool child = false;
            bool sibling = false;
            bool newHead = dlbHeadNode == null;

            while (p.Length > 0)
            {
                char letter = p[0];
                // char letter = word[0];

                if (current == null)
                // This position can be reached when starting a new head (new dictionary linked list), 
                // or current had been set to previous - which means child or sibling will have been set
                {
                    // current = DlbNodeCreateNode(letter);
                    current = new Dlb_node(letter);
                    if (newHead)
                    {
                        dlbHeadNode = current;
                        newHead = false;
                    }
                    if (child)
                    {
                        previous.child = current;
                    }
                    if (sibling)
                    {
                        previous.sibling = current;
                    }

                }

                // Reset the detection of child or sibling
                child = false;
                sibling = false;
                // and set the previous node to the current one.
                previous = current;

                // if the current letter already exists in the tree, we are on a child, then move to the next letter
                if (letter == previous.letter)
                {
                    // Move to the next letter in the word (remove the first letter of the word)
                    p = p[1..];
                    // Declare that we are working with a child
                    child = true;
                    // set the current node to the child of the previous node (node will be null but the "child" will be set)
                    current = previous.child;
                }
                // Otherwise we are on a sibling
                else
                {
                    // declare that we are working with a sibling
                    sibling = true;
                    // set the current not to the sibling of the previous node (node will be null but the "sibling" will be set)
                    current = previous.sibling;
                }
            }
            previous.valid = true;
        }


        /// <summary>create a dictionary linked list from a file. 
        /// It reads each line of the file, adds the words to the dictionary, and sets the necessary links between nodes. 
        /// </summary>
        /// <param name="dlbHeadNode">The head node of the dictionary.</param>
        /// <param name="filename">The name of the file containing the dictionary words.</param>
        /// <returns>Nothing</returns>
        internal bool DlbCreate(ref Dlb_node? dlbHeadNode, string filename)
        {
            _logger.LogDebug(filename);
            
            int lineCount = File.ReadLines(filename).Count();
            string? currentWord;
            using StreamReader sr = new(filename);

            try
            {
                for (int i = 0; i < lineCount; i++)
                {
                    currentWord = sr.ReadLine();
                    // currentWord.ic();
                    if (currentWord != null)
                    {
                        DlbPush(ref dlbHeadNode, currentWord);
                    }
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine("DlbCreate Exception while creating dlbHeadNode: " + e.Message);
                _logger.LogError($"DlbCreate Exception while creating dlbHeadNode: {e.Message}");
                return false;
            }
            finally
            {
                // Console.WriteLine("dlbHead Dictionary linked list created");
                _logger.LogInformation("dlbHead Dictionary linked list created");
            }
            return true;

        }


        /// <summary>Determine if a given word is in the dictionary 
        /// essentially the same as a push, but doesn't add any of the new letters
        /// </summary>
        /// <param name="dlbHeadNode">the dictionary linked list</param>
        /// <param name="word">the word to find</param>
        /// <returns> return true if the word is in the dictionary else return false </returns>
        internal static bool DlbLookup(Dlb_node? dlbHeadNode, string word)
        {
            Dlb_node? current = dlbHeadNode;
            Dlb_node previous = new Dlb_node('\0');
            string p = word;
            char letter;
            // bool retval = false;
            bool wordInDictionary = false;

            while (p.Length > 0)
            {
                letter = p[0];

                if (current == null)
                {
                    wordInDictionary = false;
                    break;
                }

                previous = current;

                if (letter == previous.letter)
                {
                    p = p[1..];
                    current = previous.child;
                    wordInDictionary = previous.valid;
                }
                else
                {
                    current = previous.sibling;
                    wordInDictionary = false;
                }
            }

            return wordInDictionary;
        }
    }
}