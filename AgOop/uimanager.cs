using System.Dynamic;
using SDL2;

namespace AgOop
{

    /// <summary>Manages UI-related constants and functionality </summary>
    internal static class UIManager
    {

        /// <summary>Flag to indicate need to make the window full screen or not</summary>
        internal static bool fullscreen = false;


        /// <summary> handle the keyboard events:
        ///  - BACKSPACE and ESCAPE - clear letters
        ///  - RETURN - check guess
        ///  - SPACE - shuffle
        /// - a-z - select the first instance of that letter in the shuffle box and move to the answer box
        /// </summary>
        /// <param name="SDLevent">The event from the keyboard</param>
        /// <param name="headNode">the head node of the anagrams</param>
        /// <param name="letters">The letters</param>
        /// <returns>Nothing</returns>
        internal static void HandleKeyboardEvent(SDL.SDL_Event SDLevent, Anagrams.Node? headNode, Sprite? letters)
        {
            Sprite? current = letters;
            var keyedLetter = SDLevent.key.keysym.sym;
            int maxIndex = 0;

            // F1 key pressed: toggle between windowed and full screen
            if (keyedLetter == SDL.SDL_Keycode.SDLK_F1)
            {
                if (!fullscreen)
                {
                    SDL.SDL_SetWindowFullscreen(GameManagerVariables.window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
                }
                else
                {
                    SDL.SDL_SetWindowFullscreen(GameManagerVariables.window, 0);
                }
                fullscreen = !fullscreen;
            }

            // F2 key pressed: Start new game
            else if (keyedLetter == SDL.SDL_Keycode.SDLK_F2)
            {
                GameManager.startNewGame = true;
            }

            else if (!GameManager.gamePaused)
            {
                switch (keyedLetter)
                {
                    // ESC key pressed: clear the ANSWER box
                    case SDL.SDL_Keycode.SDLK_ESCAPE:
                        
                        GameManager.clearGuess = true;
                        break;

                    // BACKSPACE key pressed: remove the last letter from the answer box if present
                    case SDL.SDL_Keycode.SDLK_BACKSPACE:
                        // Find the max index value of the letter node from the ANSWER box
                        while (current != null)
                        {
                            if (current.box == BoxConstants.ANSWER && current.index > maxIndex)
                            {
                                maxIndex = current.index;
                            }
                            current = current.next;
                        }

                        // Then send this node letter back to the SHUFFLE box
                        current = letters;
                        while (current != null)
                        {
                            if (current.box == BoxConstants.ANSWER && current.index == maxIndex)
                            {
                                current.toX = GameManager.NextBlankPosition(BoxConstants.SHUFFLE, ref current.index);
                                current.toY = BoxConstants.SHUFFLE_BOX_Y;
                                current.box = BoxConstants.SHUFFLE;
                                SoundManager.PlaySound("click-answer");
                                break;
                            }
                            current = current.next;
                        }
                        break;

                    // ENTER key pressed: submit the ANSWER to be checked
                    case SDL.SDL_Keycode.SDLK_RETURN:
                        GameManager.CheckGuess(GameManager.Answer, headNode);
                        break;

                    // SPACE key pressed: Shuffle the SHUFFLE box
                    case SDL.SDL_Keycode.SDLK_SPACE:
                        GameManager.shuffleRemaining = true;
                        SoundManager.PlaySound("shuffle");
                        break;

                    // Other key pressed: if it is a letter from the SHUFFLE box, move it to the ANSWER box
                    default:
                        while (current != null && current.box != BoxConstants.CONTROLS)
                        {
                            if (current.box == BoxConstants.SHUFFLE)
                            {
                                if (current.letter == (char)keyedLetter)
                                {
                                    current.toX = GameManager.NextBlankPosition(BoxConstants.ANSWER, ref current.index);
                                    current.toY = BoxConstants.ANSWER_BOX_Y;
                                    current.box = BoxConstants.ANSWER;
                                    SoundManager.PlaySound("click-shuffle");
                                    break;
                                }
                            }
                            current = current.next;
                        }
                        break;
                }
            }
        }


        /// <summary> Reacts to mouse clicks
        ///  - if it's in a defined hotspot then perform the appropriate action
	    /// Hotspot	        Action
	    /// -----------------------------------------------------
	    /// A letter		set the new x,y of the letter and play the appropriate sound
        /// 
	    /// ClearGuess		set the clearGuess flag
        /// 
	    /// checkGuess		pass the current answer to the checkGuess routine
        /// 
	    /// solvePuzzle		set the solvePuzzle flag
        /// 
	    /// shuffle		    set the shuffle flag and play the appropriate sound
        /// 
	    /// newGame		    set the newGame flag
        /// 
	    /// quitGame		set the quitGame flag
        /// </summary>
        /// <param name="button">mouse button that has been clicked</param>
        /// <param name="x">the x coords of the mouse</param>
        /// <param name="y">the y coords of the mouse</param>
        /// <param name="screen">the SDL_Surface to display the image</param>
        /// <param name="headNode">pointer to the top of the answers list</param>
        /// <param name="letters">pointer to the letters sprites</param>
        /// <returns>Nothing</returns>
        internal static void ClickDetect(int button, int x, int y, IntPtr screen, Anagrams.Node? headNode, Sprite? letters)
        {
            Sprite? current = letters;

            if (!GameManager.gamePaused)
            {
                // ANSWER or SHUFFLE box area clicked
                while (current != null && current.box != BoxConstants.CONTROLS)
                {
                    // Have we clicked on this current letter (from letters), be it in answer or shuffle box?
                    if (x >= current.x && x <= (current.x + current.w) && y >= current.y && y <= (current.y + current.h))
                    {
                        if (current.box == BoxConstants.SHUFFLE)
                        {
                            current.toX = GameManager.NextBlankPosition(BoxConstants.ANSWER, ref current.index);
                            current.toY = BoxConstants.ANSWER_BOX_Y;
                            current.box = BoxConstants.ANSWER;
                            SoundManager.PlaySound("click-shuffle");
                        }
                        else
                        {
                            current.toX = GameManager.NextBlankPosition(BoxConstants.SHUFFLE, ref current.index);
                            current.toY = BoxConstants.SHUFFLE_BOX_Y;
                            current.box = BoxConstants.SHUFFLE;
                            SoundManager.PlaySound("click-answer");
                        }
                        break;
                    }
                    current = current.next;
                }

                // clear ANSWER box (red cross) clicked
                if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxClear], x, y))
                {
                    GameManager.clearGuess = true;
                }

                // check ANSWER box (green tick) clicked
                if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxEnter], x, y))
                {
                    GameManager.CheckGuess(GameManager.Answer, headNode);
                }

                // Solve box area clicked
                if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxSolve], x, y))
                {
                    GameManager.solvePuzzle = true;
                }

                // Shuffle SHUFFLE area clicked
                if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxShuffle], x, y))
                {
                    GameManager.shuffleRemaining = true;
                    SoundManager.PlaySound("shuffle");
                }
            }

            // start new game button clicked
            if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxNew], x, y))
            {
                GameManager.startNewGame = true;
            }

            // quit game button clicked
            if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxQuit], x, y))
            {
                GameManager.quitGame = true;
            }
        }

    }
}