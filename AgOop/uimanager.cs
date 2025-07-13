using Microsoft.Extensions.Logging;
using System.Dynamic;
using SDL2;

namespace AgOop
{

    /// <summary>Manages UI-related constants and functionality </summary>
    internal class UIManager
    {

        /// <summary>Flag to indicate need to make the window full screen or not</summary>
        internal bool fullscreen = false;

        private readonly ILogger<UIManager> _logger;

        internal GameManager? _gameManager { get; set; }
        internal SoundManager? _soundManager;

        // internal UIManager(SoundManager soundManager)
        public UIManager(ILogger<UIManager> logger)
        {
            _logger = logger;
            // _soundManager = soundManager;
            // _gameManager = gameManager;
        }


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
        internal void HandleKeyboardEvent(SDL.SDL_Event SDLevent, Anagrams.Node? headNode, Sprite? letters)
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
                // TODO: trigger a rescaling of all the sprites
            }

            // F2 key pressed: Start new game
            else if (keyedLetter == SDL.SDL_Keycode.SDLK_F2)
            {
                GameState.startNewGame = true;
                // _gameManager.startNewGame = true;
                // GameManager.startNewGame = true;
            }

            // F5 key pressed: quit the game
            else if (keyedLetter == SDL.SDL_Keycode.SDLK_F5)
            {
                GameState.quitGame = true;
                // _gameManager.quitGame = true;
                // GameManager.quitGame = true;
            }

            // else if (!GameManager.gamePaused)
            // else if (!_gameManager.gamePaused)
            else if (!GameState.gamePaused)
            {
                switch (keyedLetter)
                {
                    // ESC key pressed: clear the ANSWER box
                    case SDL.SDL_Keycode.SDLK_ESCAPE:

                        GameState.clearGuess = true;
                        // _gameManager.clearGuess = true;
                        // GameManager.clearGuess = true;
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
                                current.toX = _gameManager.NextBlankPosition(BoxConstants.SHUFFLE, ref current.index);
                                // current.toX = GameManager.NextBlankPosition(BoxConstants.SHUFFLE, ref current.index);
                                current.toY = BoxConstants.SHUFFLE_BOX_Y;
                                current.box = BoxConstants.SHUFFLE;
                                // SoundManager.PlaySound("click-answer");
                                // TODO: Fix using a proper queue system
                                // using (SoundManager sm = new SoundManager())
                                // {
                                //     sm.PlaySound("click-answer");
                                // }
                                _soundManager.PlaySound("click-answer");
                                break;
                            }
                            current = current.next;
                        }
                        break;

                    // ENTER key pressed: submit the ANSWER to be checked
                    case SDL.SDL_Keycode.SDLK_RETURN:
                        _gameManager.CheckGuess(GameState.Answer, headNode);
                        // _gameManager.CheckGuess(_gameManager.Answer, headNode);
                        // _gameManager.CheckGuess(GameManager.Answer, headNode);
                        // GameManager.CheckGuess(GameManager.Answer, headNode);
                        break;

                    // SPACE key pressed: Shuffle the SHUFFLE box
                    case SDL.SDL_Keycode.SDLK_SPACE:
                        GameState.shuffleRemaining = true;
                        // _gameManager.shuffleRemaining = true;
                        // GameManager.shuffleRemaining = true;
                        // SoundManager.PlaySound("shuffle");
                        // TODO: Fix using a proper queue system
                        // using (SoundManager sm = new SoundManager())
                        // {
                        //     sm.PlaySound("shuffle");
                        // }
                        _soundManager.PlaySound("shuffle");
                        break;

                    // Other key pressed: if it is a letter from the SHUFFLE box, move it to the ANSWER box
                    default:
                        while (current != null && current.box != BoxConstants.CONTROLS)
                        {
                            if (current.box == BoxConstants.SHUFFLE)
                            {
                                if (current.letter == (char)keyedLetter)
                                {
                                    current.toX = _gameManager.NextBlankPosition(BoxConstants.ANSWER, ref current.index);
                                    // current.toX = GameManager.NextBlankPosition(BoxConstants.ANSWER, ref current.index);
                                    current.toY = BoxConstants.ANSWER_BOX_Y;
                                    current.box = BoxConstants.ANSWER;
                                    // SoundManager.PlaySound("click-shuffle");
                                    // TODO: Fix using a proper queue system
                                    // using (SoundManager sm = new SoundManager())
                                    // {
                                    //     sm.PlaySound("click-shuffle");
                                    // }
                                    _soundManager.PlaySound("click-shuffle");
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
        internal void ClickDetect(int button, int x, int y, IntPtr screen, Anagrams.Node? headNode, Sprite? letters)
        {
            Sprite? current = letters;

            // if (!GameManager.gamePaused)
            // if (!_gameManager.gamePaused)
            if (!GameState.gamePaused)
            {
                // ANSWER or SHUFFLE box area clicked
                while (current != null && current.box != BoxConstants.CONTROLS)
                {
                    // Have we clicked on this current letter (from letters), be it in answer or shuffle box?
                    if (x >= current.x && x <= (current.x + current.w) && y >= current.y && y <= (current.y + current.h))
                    {
                        if (current.box == BoxConstants.SHUFFLE)
                        {
                            // current.toX = GameManager.NextBlankPosition(BoxConstants.ANSWER, ref current.index);
                            current.toX = _gameManager.NextBlankPosition(BoxConstants.ANSWER, ref current.index);
                            current.toY = BoxConstants.ANSWER_BOX_Y;
                            current.box = BoxConstants.ANSWER;
                            // SoundManager.PlaySound("click-shuffle");
                            // TODO: Fix using a proper queue system
                            // using (SoundManager sm = new SoundManager())
                            // {
                            //     sm.PlaySound("click-shuffle");
                            // }
                            _soundManager.PlaySound("click-shuffle");
                        }
                        else
                        {
                            // current.toX = GameManager.NextBlankPosition(BoxConstants.SHUFFLE, ref current.index);
                            current.toX = _gameManager.NextBlankPosition(BoxConstants.SHUFFLE, ref current.index);
                            current.toY = BoxConstants.SHUFFLE_BOX_Y;
                            current.box = BoxConstants.SHUFFLE;
                            // SoundManager.PlaySound("click-answer");
                            // TODO: Fix using a proper queue system
                            // using (SoundManager sm = new SoundManager())
                            // {
                            //     sm.PlaySound("click-answer");
                            // }
                            _soundManager.PlaySound("click-answer");
                        }
                        break;
                    }
                    current = current.next;
                }

                // clear ANSWER box (red cross) clicked
                // if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxClear], x, y))
                if (IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxClear], x, y))
                // if (_gameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxClear], x, y))
                {
                    // GameManager.clearGuess = true;
                    GameState.clearGuess = true;
                    // _gameManager.clearGuess = true;
                }

                // check ANSWER box (green tick) clicked
                // if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxEnter], x, y))
                if (IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxEnter], x, y))
                // if (_gameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxEnter], x, y))
                {
                    _gameManager.CheckGuess(GameState.Answer, headNode);
                    // _gameManager.CheckGuess(_gameManager.Answer, headNode);
                    // GameManager.CheckGuess(GameManager.Answer, headNode);
                }

                // Solve box area clicked
                // if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxSolve], x, y))
                if (IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxSolve], x, y))
                // if (_gameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxSolve], x, y))
                {
                    GameState.solvePuzzle = true;
                    // _gameManager.solvePuzzle = true;
                }

                // Shuffle SHUFFLE area clicked
                // if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxShuffle], x, y))
                if (IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxShuffle], x, y))
                // if (_gameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxShuffle], x, y))
                {
                    GameState.shuffleRemaining = true;
                    // _gameManager.shuffleRemaining = true;
                    // SoundManager.PlaySound("shuffle");
                    // TODO: Fix using a proper queue system
                    // using (SoundManager sm = new SoundManager())
                    // {
                    //     sm.PlaySound("click-answer");
                    // }
                    _soundManager.PlaySound("click-answer");
                }
            }

            // start new game button clicked
            // if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxNew], x, y))
            // if (_gameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxNew], x, y))
            if (IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxNew], x, y))
            {
                // GameManager.startNewGame = true;
                // _gameManager.startNewGame = true;
                GameState.startNewGame = true;
            }

            // quit game button clicked
            // if (GameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxQuit], x, y))
            if (IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxQuit], x, y))
            // if (_gameManager.IsInside(HotBoxes.hotbox[(int)BoxConstants.HotBoxes.boxQuit], x, y))
            {
                // GameManager.quitGame = true;
                // _gameManager.quitGame = true;
                GameState.quitGame = true;
            }
        }

        /// <summary> Returns a boolean indicating whether the click was inside a box </summary>
        /// <param name="box"> The box </param>
        /// <param name="x"> the x position clicked </param>
        /// <param name="y">The y position clicked</param>
        /// <returns>true if clicked inside the box, false otherwise</returns>
        internal static bool IsInside(Box box, int x, int y)
        {
            return (x > box.x) && (x < (box.x + box.width)) &&
                    (y > box.y) && (y < (box.y + box.height));
        }

    }
}