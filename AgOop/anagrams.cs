namespace AgOop
{

    /// <summary> The anagrams linkedlist node</summary>
    internal class Anagrams
    {
        

        /// <summary>Node containing an anagram from the list of anagrams 
        /// that can be made from the root word </summary>
        // TODO: fix the names to naming rules
        internal class Node
        {

            /// <summary>The anagram word </summary>
            internal string anagram { get; set; } = "";

            /// <summary>This is set if the user guessed, or if the game timed out and the game found it </summary>
            internal bool found { get; set; } = false;

            /// <summary>This is set if the user guessed</summary>
            internal bool guessed { get; set; } = false;

            /// <summary>Length of the anagram word, used for counting points</summary>
            internal int length { get; set; } = 0;

            /// <summary>Pointer to the next node </summary>
            internal Node? next { get; set; } = null;

            /// <summary>Node constructor </summary>
            /// <param name="anagram">The word to store in the node</param>
            internal Node(string anagram)
            {
                this.anagram = anagram;
                found = false;
                guessed = false;
                length = anagram.Length;
            }
        }
    }
}