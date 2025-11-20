using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SDL2;


// TODO: Create a usable installer
// TODO: Test under different platforms
// TODO: store the files in a decent location


namespace AgOop
{
    internal class GameConstants
    {
        /// <summary> Application version </summary>
        // Based on version 0.7 (dusli) of the game written in C
        internal const string VERSION = "0.7-CS1.0";

        /// <summary> License </summary>
        // following the license version of previous authors
        internal const string LICENCE = "GPL-v2";

        /// <summary> List of languages provided with the app </summary>
        // TODO: of course, could be better done by scanning through the i18n folder.
        internal static string[] AllowedLanguages = { "en-GB", "it", "fr-FR", "pt-BR" };

        /// <summary> Help message </summary>
        // following the license version of previous authors
        internal const string HELP =
            "AgOop [-h/--help] [-v/--version] [-l/--locale<language-code>]\n" +
            " -h / --help: (optional) this message\n" +
            " -v / --version: (optional) the version of this release\n" +
            " -l / --locale <language-code>: (optional) the language to use.\n" +
            "            Language-codes choices currently are\n" +
            "                UK English (default): en-GB\n" +
            "                Italian: it\n" +
            "                French: fr-FR\n" +
            "                Portugese (Brazilian): pt-BR\n"
        ;  
    }

    /// <summary> Game related constants </summary>
    internal class GameManagerVariables
    {
        /// <summary> Sets the time of the game (5 mins default = 300s). </summary>
        internal const int AVAILABLE_TIME = 300;

        /// <summary>The speed at which the letters move from one box to the other</summary>
        internal static int letterSpeed = SpriteConstants.LETTER_FAST;

        // - SDL window
        /// <summary>the SDL window</summary>
        internal static IntPtr window;

        /// <summary>the SDL renderer</summary
        internal static IntPtr renderer;

    }


    /// <summary> Manages the game handling the anagrams, solving the game, requesting the screen refreshes... </summary>
    internal class GameManager
    {

        // private static readonly AgOopLogger logger = new AgOopLogger("GameManager");
        private readonly ILogger<GameManager> _logger;
        private readonly SoundManager _soundManager;
        private readonly AnagramsManager _anagramsManager;
        private readonly UIManager _uiManager;
        private readonly SpriteManager _spriteManager;

        private readonly WordsList _wordsList;

        /// <summary> Constructor - Initialises the game
        public GameManager(ILogger<GameManager> logger, 
                        AnagramsManager anagramsManager,
                        SoundManager soundManager,
                        UIManager uiManager,
                        SpriteManager spriteManager,
                        WordsList wordsList)
        {
            _logger = logger;
            _soundManager = soundManager;
            _anagramsManager = anagramsManager;
            _uiManager = uiManager;
            _spriteManager = spriteManager;
            // TODO: should probably use this to clean up even more, but haven't done yet. And a pseudo destructor too

            _wordsList = wordsList;
        }


        /// <summary>The word from which the anagrams are created</summary>
        internal string rootword = "";

        // Game time inform
        /// <summary>time at which the game was started</summary>
        internal DateTime gameStart;

        /// <summary>The number of seconds elapsed since the begining of the game</summary>
        internal int gameTime;

        /// <summary>Flag to indicate to stop the clock</summary>
        internal bool stopTheClock = false;

        /// <summary>Flag to indicate to play the clock ticking sound</summary>
        internal bool TickClock = false;


        // Score info
        /// <summary>The information to the final score (if all words found, score x2</summary>
        internal int totalScore = 0;

        /// <summary>The current score, incremented by the number of letters of the words guessed correctly</summary>
        internal int score = 0;

        /// <summary>Flag to indicate need to update the score display</summary>
        internal bool updateTheScore = false;


        // Guesses related flags and info
        /// <summary>Flag to indicate need to shuffle the remaining letters in the shuffle box</summary>
        internal bool shuffleRemaining = false;

        /// <summary>Flag to indicate need to clear the Answers box</summary>
        internal bool clearGuess = false;

        /// <summary>The number of anagrams to find</summary>
        internal int answersSought = 0;

        /// <summary>The number of anagrams found so far</summary>
        internal int answersGot = 0;

        /// <summary>Flag repesenting that the rootword was found</summary>
        internal bool gotBigWord = false;

        /// <summary>The length of the rootword</summary>
        internal int bigWordLen = 0;

        /// <summary>Flag to indicate that a word guessed had already been discovered</summary>
        internal bool foundDuplicate = false;

        /// <summary>Flag to indicate need to update the small answer boxes</summary>
        internal bool updateAnswers = false;


        // Game status flags
        /// <summary>Flag to indicate need to start a new game</summary>
        internal bool startNewGame = false;

        /// <summary>Flag to indicate that game was paused</summary>
        internal bool gamePaused = false;

        /// <summary>Flag to indicate need to quit the game </summary>
        internal bool quitGame = false;

        /// <summary>Flag to indicate need to solve the anagrams (game timed out)</summary>
        internal bool solvePuzzle = false;

        /// <summary>Flag to indicate the game was won</summary>
        internal bool winGame = false;

        // shuffle is an array that can be modified so it needs to have a field that is set and get
        /// <summary>Represents a shuffled array of characters.</summary>
        internal string Shuffle = "";

        // answer is an array that can be modified so it needs to have a field that is set and get
        /// <summary>Represents an answer array of characters.</summary>
        internal string Answer = "";


        /// <summary> Declare all all the anagrams as found (but not necessarily guessed)</summary>
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


        /// <summary> Check if the guess is a word that is to be found </summary>
        /// <param name="answer">the word proposed</param>
        /// <param name="headNode">The head node of anagrams</param>
        /// <returns>Nothing</returns>
        internal void CheckGuess(string answer, Anagrams.Node? headNode)
        {
            Anagrams.Node? current = headNode;
            bool foundWord = false;
            // bool foundAllLength = true; // used for Gamerzilla - ignored here

            string test;
            // int len = AnagramsManager.NextBlank(answer) - 1;
            int len = _anagramsManager.NextBlank(answer) - 1;
            if (len == -1) len = answer.Length;
            test = answer[0..len];

            while (current != null)
            {
                // if (current.anagram == new string(test))
                if (current.anagram == test)
                {
                    foundWord = true;
                    if (!current.found)
                    {
                        score += current.length;
                        totalScore += current.length;
                        answersGot++;
                        if (len == bigWordLen)
                        {
                            gotBigWord = true;
                            // SoundManager.PlaySound("foundbig");
                            // TODO: Fix using a proper queue system
                            // using (SoundManager sm = new SoundManager())
                            // {
                            //     sm.PlaySound("foundbig");
                            // }
                            _soundManager.PlaySound("foundbig");
                        }
                        else
                        {
                            // just a normal word
                            // SoundManager.PlaySound("found");
                            // TODO: Fix using a proper queue system
                            // using (SoundManager sm = new SoundManager())
                            // {
                            //     sm.PlaySound("found");
                            // }
                            _soundManager.PlaySound("found");
                        }


                        if (answersSought == answersGot)
                        {
                            // getting all answers gives us the game score again!!
                            totalScore += score;
                            winGame = true;
                        }
                        current.found = true;
                        current.guessed = true;
                        updateTheScore = true;
                    }
                    else
                    {
                        foundDuplicate = true;
                        // SoundManager.PlaySound("duplicate");
                        // TODO: Fix using a proper queue system
                        // using (SoundManager sm = new SoundManager())
                        // {
                        //     sm.PlaySound("duplicate");
                        // }            
                        _soundManager.PlaySound("duplicate");            

                    }
                    updateAnswers = true;
                    break;
                }
                current = current.next;
            }

            // used for gamerzilla - ignored here
            // current = headNode;
            // while (current != null)
            // {
            //     if ((!current.found) && (len == current.anagram.Length))
            //     {
            //         foundAllLength = false; 
            //     }
            //     current = current.next;
            // }

            if (!foundWord)
            {
                // SoundManager.PlaySound("badword");
                // TODO: Fix using a proper queue system
                // using (SoundManager sm = new SoundManager())
                // {
                //     sm.PlaySound("badword");
                // }
                _soundManager.PlaySound("badword");
            }
        }


        /// <summary> determine the next blank space in a the Answers or Shuffle box 
        /// blanks are indicated by pound not space.
        /// When a blank is found, move the chosen letter from one box to the other.
        /// i.e. If we're using the ANSWER box, 
        ///   - move the chosen letter from the SHUFFLE box to the ANSWER box 
        ///   - and move a SPACE back to the SHUFFLE box. 
        /// and if we're using the SHUFFLE box 
        ///  - move the chosen letter from ANSWER to SHUFFLE 
        ///  - and move a SPACE into ANSWER.
        /// </summary>
        /// <param name="box">the ANSWER or SHUFFLE box</param>
        /// <param name="index">pointer to the letter we're interested in</param>
        /// <returns>the coords of the next blank position</returns>
        internal int NextBlankPosition(int box, ref int index)
        {
            int i = 0;

            switch (box)  // destination box for the letter
            {
                case BoxConstants.ANSWER:

                    // TODO: Handle this better?
                    for (i = 0; i < 7; i++)
                    {
                        if (Answer.ToCharArray()[i] == AnagramsConstants.SPACE_CHAR)
                        {
                            break;
                        }
                    }
                    // New: if no SPACE_CHAR found in Answer, 
                    // i becomes 7 creating out of bound exception
                    // not an issue is C due to the null char to end a string
                    if (i < 7)
                    {
                        // Answer.ToCharArray()[i] = Shuffle[index];
                        Answer = Answer[..i] + Shuffle[index] + Answer[(i + 1)..];
                        // Shuffle.ToCharArray()[index] = AnagramsConstants.SPACE_CHAR;
                        Shuffle = Shuffle[..index] + AnagramsConstants.SPACE_CHAR + Shuffle[(index + 1)..];
                    }
                    break;

                case BoxConstants.SHUFFLE:
                    for (i = 0; i < 7; i++)
                    {
                        if (Shuffle.ToCharArray()[i] == AnagramsConstants.SPACE_CHAR)
                        {
                            break;
                        }
                    }
                    // if no SPACE_CHAR found in Answer, 
                    // i becomes 7 creating out of bound exception
                    // not an issue is C due to the null char to end a string
                    if (i < 7)
                    {
                        // Shuffle.ToCharArray()[i] = Answer[index];
                        Shuffle = Shuffle[..i] + Answer[index] + Shuffle[(i + 1)..];

                        // Answer.ToCharArray()[index] = AnagramsConstants.SPACE_CHAR;
                        Answer = Answer[..index] + AnagramsConstants.SPACE_CHAR + Answer[(index + 1)..];
                    }
                    break;

                default:
                    break;
            }

            index = i;

            // return the toX position in the target box of the letter returned
            return i * (SpriteConstants.GAME_LETTER_WIDTH + SpriteConstants.GAME_LETTER_SPACE) + BoxConstants.BOX_START_X;
        }


        /// <summary> Returns a boolean indicating whether the click was inside a box </summary>
        /// <param name="box"> The box </param>
        /// <param name="x"> the x position clicked </param>
        /// <param name="y">The y position clicked</param>
        /// <returns>true if clicked inside the box, false otherwise</returns>
        internal bool IsInside(Box box, int x, int y)
        {
            return (x > box.x) && (x < (box.x + box.width)) &&
                    (y > box.y) && (y < (box.y + box.height));
        }


        // TODO: Clarify what the return is - just for playing a sound should be a bool
        /// <summary> move all letters from answer to shuffle </summary>
        /// <param name="letters">the letter sprites</param>
        /// <returns>the count of??? letters cleared, used to play a sound if not null??</returns>
        internal int ClearWord(Sprite? letters)
        {
            Sprite? current = letters;
            Sprite[] orderedLetters = new Sprite[7];
            int count = 0;

            while (current != null)
            {
                if (current.box == BoxConstants.ANSWER)
                {
                    count++;
                    orderedLetters[current.index] = new Sprite(1);
                    orderedLetters[current.index] = current;
                    current.toY = BoxConstants.SHUFFLE_BOX_Y;
                    current.box = BoxConstants.SHUFFLE;
                }
                current = current.next;
            }

            for (int i = 0; i < 7; i++)
            {
                if (orderedLetters[i] != null)
                {
                    orderedLetters[i].toX = NextBlankPosition(BoxConstants.SHUFFLE, ref orderedLetters[i].index);
                }
            }

            return count;
        }


        //[ ]: as the anagrams list is now maintained by WordsList, there is no need to pass it as argument

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
        // internal void NewGame(ref Anagrams.Node? headNode, WordsList.Dlb_node? dlbHeadNode, IntPtr screen, ref Sprite? letters)
        internal void NewGame(ref Anagrams.Node? headNode, IntPtr screen, ref Sprite? letters)
        {
            string guess;
            string remain = "";

            // happy is true if we have < 67 anagrams and => 6
            bool happy = false;

            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = 800,
                h = 600
            };
            _spriteManager.SDLScale_RenderCopy(screen, _spriteManager.backgroundTex, null, dest);



            _anagramsManager.DestroyLetters(ref letters);
            // AnagramsManager.DestroyLetters(ref letters);

            // TODO: move the creation of the wordslist into wordslist...

            while (!happy)
            {
                // changed this max size from original game
                // AnagramsManager.GetRandomWord(ref rootword, AnagramsConstants.MAX_ANAGRAM_LENGTH);
                _anagramsManager.GetRandomWord(ref rootword, AnagramsConstants.MAX_ANAGRAM_LENGTH);
                bigWordLen = rootword.Length - 1; // GetRandomWord adds an extra space at the end
                guess = "";
                remain = rootword;

                // AnagramsManager.DestroyAnswers(ref headNode);
                _anagramsManager.DestroyAnswers(ref headNode);

                // AnagramsManager.Ag(ref headNode, dlbHeadNode, guess, remain);
                _anagramsManager.Ag(ref headNode, _wordsList.dlbHeadNode, guess, remain);

                // answersSought = AnagramsManager.Length(headNode);
                answersSought = _anagramsManager.Length(headNode);

                happy = (answersSought <= 77) && (answersSought >= 6);
            }

            // now we have a good set of words - sort them alphabetically and by size
            // AnagramsManager.Sort(ref headNode!);
            _anagramsManager.Sort(ref headNode!);

            for (int i = bigWordLen; i < 7; i++)
            {
                remain = remain[0..(i - 1)] + AnagramsConstants.SPACE_CHAR;
            }
            remain = remain[0..7]; // making sure we don't have extra chars
            char[] remainToShuffle = remain.ToCharArray();
            // AnagramsManager.ShuffleWord(ref remainToShuffle);
            _anagramsManager.ShuffleWord(ref remainToShuffle);
            Shuffle = new string(remainToShuffle);

            Answer = AnagramsConstants.SPACE_FILLED_CHARS;

            /* build up the letter sprites */

            _spriteManager.BuildLetters(ref letters, screen);
            _spriteManager.AddClock(ref letters, screen);
            _spriteManager.AddScore(ref letters, screen);

            /* display all answer boxes */
            _spriteManager.DisplayAnswerBoxes(headNode, screen);

            gotBigWord = false;
            score = 0;
            updateTheScore = true;
            gamePaused = false;
            winGame = false;
            answersGot = 0;

            gameStart = DateTime.Now;
            gameTime = 0;
            stopTheClock = false;
        }


        /// <summary> Callback method for SDL timer events
        /// Attempt at rewrite of the timer callback from the original C
        /// </summary>
        /// <param name="interval">The interval in milliseconds for the timer.</param>
        /// <param name="param">Additional parameters for the callback (unused).</param>
        /// <returns>The interval for the next timer event.</returns>        
        public uint TimerCallBack(uint interval, IntPtr param)
        {
            SDL.SDL_UserEvent userEvent = new SDL.SDL_UserEvent()
            {
                type = (uint)SDL.SDL_EventType.SDL_USEREVENT,
                code = 0,
                data1 = IntPtr.Zero,
                data2 = IntPtr.Zero,
            };

            SDL.SDL_Event sdlEvent = new SDL.SDL_Event()
            {
                type = SDL.SDL_EventType.SDL_USEREVENT,
                user = userEvent,
            };

            SDL.SDL_PushEvent(ref sdlEvent);

            return interval;
        }


        //[ ]: as the anagrams list is now maintained by WordsList, there is no need to pass it as argument

        /// <summary>
        /// a big while loop that runs the full length of the game, 
        /// checks the game events and responds accordingly
        ///
        /// event		    action
        /// -------------------------------------------------
        /// winGame	        stop the clock and solve puzzle
        /// timeRemaining   update the clock tick
        /// timeUp	        stop the clock and solve puzzle
        /// solvePuzzle	    trigger solve puzzle and stop clock
        /// updateAnswers   trigger update answers
        /// startNew        trigger start new
        /// updateScore	    trigger update score
        /// shuffle	        trigger shuffle
        /// clear		    trigger clear answer
        /// quit		    end loop
        /// poll events     check for keyboard/mouse and quit
        ///
        /// finally, move the sprites - this is always called so the sprites 
        /// are always considered to be moving no "move sprites" event exists 
        /// - sprites x and y just needs to be updated and they will always be moved
        /// </summary>
        /// <param name="headNode">first node in the answers list (in/out)</param>
        /// <param name="dldHeadNode">first node in the dictionary list</param>
        /// <param name="screen">SDL_Surface to display the image</param>
        /// <param name="letters">first node in the letter sprites (in/out)</param>
        /// <returns>Nothing</returns>
        public void GameLoop(ref Anagrams.Node? headNode, IntPtr screen, ref Sprite? letters)
        {
            _logger.LogWarning("GameLoop started");
            bool done = false;
            SDL.SDL_Event sdlEvent;
            TimeSpan timeNow;

            SDL.SDL_Init(SDL.SDL_INIT_TIMER);
            uint timer_delay = 20;
            int timer = SDL.SDL_AddTimer(timer_delay, TimerCallBack, IntPtr.Zero);

            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = 800,
                h = 600
            };

            while (!done)
            {
                DateTime frameStart = DateTime.Now;

                while (SDL.SDL_PollEvent(out sdlEvent) != 0)  // need to use int as return is not a bool
                {
                    switch (sdlEvent.type)
                    {
                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            _spriteManager.SDLScale_MouseEvent(ref sdlEvent);
                            _uiManager.ClickDetect(sdlEvent.button.button, sdlEvent.button.x, sdlEvent.button.y, screen, headNode, letters);
                            // UIManager.ClickDetect(sdlEvent.button.button, sdlEvent.button.x, sdlEvent.button.y, screen, headNode, letters);
                            break;

                        case SDL.SDL_EventType.SDL_KEYUP:
                            _uiManager.HandleKeyboardEvent(sdlEvent, headNode, letters);
                            // UIManager.HandleKeyboardEvent(sdlEvent, headNode, letters);
                            break;

                        case SDL.SDL_EventType.SDL_QUIT:
                            quitGame = true;
                            break;

                        case SDL.SDL_EventType.SDL_WINDOWEVENT:
                            if ((sdlEvent.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED) ||
                                    (sdlEvent.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED))
                            {
                                double scalew = sdlEvent.window.data1 / 800.0;
                                double scaleh = sdlEvent.window.data2 / 600.0;
                                _spriteManager.SDLScaleSet(scalew, scaleh);
                            }
                            break;
                    }
                }

                if (winGame)
                {
                    stopTheClock = true;
                    solvePuzzle = true;
                }

                if ((gameTime < GameManagerVariables.AVAILABLE_TIME) && !stopTheClock)
                {
                    timeNow = DateTime.Now - gameStart;

                    int timeNowSeconds = timeNow.Minutes * 60 + timeNow.Seconds;
                    if (timeNowSeconds != gameTime)
                    {
                        gameTime = timeNowSeconds;
                        _spriteManager.UpdateTime(screen);
                    }
                }
                else
                {
                    if (!stopTheClock)
                    {
                        stopTheClock = true;
                        solvePuzzle = true;
                    }
                }

                // Check messages
                if (solvePuzzle)
                {
                    // Walk the list, setting everything to found
                    SolveIt(headNode);
                    ClearWord(letters);
                    Shuffle = AnagramsConstants.SPACE_FILLED_STRING;
                    Answer = rootword;
                    gamePaused = true;
                    if (!stopTheClock) // not sure why there is this if statement
                    {
                        stopTheClock = true;
                    }
                    solvePuzzle = false;
                }

                // display all the little answers
                if (updateAnswers)
                {
                    // move letters back down again
                    ClearWord(letters);
                    updateAnswers = false;
                }
                _spriteManager.DisplayAnswerBoxes(headNode, screen);

                // start a new game
                if (startNewGame)
                {
                    if (!gotBigWord)  // not sure why this is here - probably for gamerzilla?
                    {
                        totalScore = 0;
                    }

                    //[ ]: dlbHeadNode is now handled by WordsList, no need to pass it as reference
                    // NewGame(ref headNode, dldHeadNode, screen, ref letters);
                    NewGame(ref headNode,  screen, ref letters);

                    startNewGame = false;
                }

                if (shuffleRemaining)
                {
                    // AnagramsManager.ShuffleAvailableLetters(ref Shuffle, ref letters);
                    _anagramsManager.ShuffleAvailableLetters(ref Shuffle, ref letters);
                    shuffleRemaining = false;
                }

                if (updateTheScore)
                {
                    updateTheScore = false;
                    _spriteManager.UpdateScore(screen);
                }

                if (TickClock)
                {
                    // TODO: Fix using a proper queue system
                    // using (SoundManager sm = new SoundManager())
                    // {
                    //     sm.PlaySound("clock-tick");
                    // }
                    _soundManager.PlaySound("clock-tick");
                    TickClock = false;
                }

                if (clearGuess)
                {
                    // clear the guess
                    if (ClearWord(letters) > 0)
                    {
                        clearGuess = false;
                        // TODO: Fix using a proper queue system
                        // SoundManager.PlaySound("clear");
                        // using (SoundManager sm = new SoundManager())
                        // {
                        //     sm.PlaySound("clear");
                        // }
                        _soundManager.PlaySound("clear");
                    }
                }

                if (quitGame)
                {
                    done = true;
                }

                // clear the renderer
                SDL.SDL_SetRenderDrawColor(screen, 0, 0, 0, 255);
                SDL.SDL_RenderClear(screen);
                // update the renderer
                _spriteManager.SDLScale_RenderCopy(screen, _spriteManager.backgroundTex, null, dest);
                _spriteManager.DisplayAnswerBoxes(headNode, screen);
                _spriteManager.MoveSprites(screen, letters, GameManagerVariables.letterSpeed);
                // present the renderer
                SDL.SDL_RenderPresent(screen);

                // Add a small delay to prevent CPU overuse
                double frameTime = (DateTime.Now - frameStart).TotalMilliseconds;
                double delay = Math.Max(0, 16 - frameTime);
                SDL.SDL_Delay((uint)delay); // roughly 60 FPS

            }
        }
    }

    internal static class AgOop
    {
        /// <summary>The application entry point </summary>
        /// <param name="args">The command line arguments, uf ysed</param>
        /// <returns>0 if exited normally, 1 if exited with an error</returns>
        // internal static int Main(string[] args)
        // {
        //     var di = new DependencyInjection();
        //     var serviceProvider = di.InitialiseServices(args);
        //     var gameManager = serviceProvider.GetRequiredService<GameManager>();

        //     // var anagramsManager = serviceProvider.GetRequiredService<AnagramsManager>();
        //     Console.WriteLine("Resolved");
        //     return 0;
        // }


        internal static int Main(string[] args)
        {
            // logger.LogInformation("Starting AgOop GameManager...");

            // var ServiceProvider = InitialiseLogging.InitialiseLoggingService<GameManager>();
            // ILogger<GameManager> _logger = LoggerFactory.CreateLogger<GameManager>();
            // GameManager = new GameManager()
            // var loggingservice = InitialiseLogging.InitialiseLoggingService<GameManager>();
            // // var gameManagerLogger = loggingservice.GetRequiredService<GameManager>();
            // // var logger = loggingservice.GetRequiredService<ILogger<GameManager>>();
            // GameManager gameManager = new GameManager(
            //         loggingservice.GetRequiredService<ILogger<GameManager>>());

            // // logger = loggingservice.GetRequiredService<ILogger<AnagramsManager>>();
            // AnagramsManager AnagramsManager = new AnagramsManager(
            //         loggingservice.GetRequiredService<ILogger<AnagramsManager>>());

            // WordsList WordsList = new WordsList(
            //     loggingservice.GetRequiredService<ILogger<WordsList>>());

            var di = new DependencyInjection();
            var serviceProvider = di.InitialiseServices(args);

            // var gameManagerFactory = serviceProvider.GetRequiredService<Func<GameManager>>();
            // var gameManager = gameManagerFactory();

            var localeManager = serviceProvider.GetRequiredService<LocaleManager>();
            var anagramsManager = serviceProvider.GetRequiredService<AnagramsManager>();
            var gameManager = serviceProvider.GetRequiredService<GameManager>();
            var soundManager = serviceProvider.GetRequiredService<SoundManager>();
            var spriteManager = serviceProvider.GetRequiredService<SpriteManager>();
            var uiManager = serviceProvider.GetRequiredService<UIManager>();
            var wordsList = serviceProvider.GetRequiredService<WordsList>();
            soundManager._gameManager = gameManager;
            soundManager._localeManager = localeManager;
            anagramsManager._localeManager = localeManager;
            spriteManager._localeManager = localeManager;
            spriteManager._gameManager = gameManager;
            uiManager._gameManager = gameManager;
            uiManager._soundManager = soundManager;

            // now initialised in WordsList
            // WordsList.Dlb_node? dlbHeadNode = null;
            
            Anagrams.Node? headNode = null;
            spriteManager.Initialise();
            Sprite? letters = null;
            // {

            // }
            // Configure the locale (language/path)
            // new LocaleManager(args);
            // var localeManager(args) = ServiceProvider.GetRequiredService<LocaleManager>;
            localeManager.GetBasePathAndInitLocale(args);

            // create the word list from the dictionary file in the locale language
            string wordlist = localeManager.language + "wordlist.txt";
            // new WordsList(ref dlbHeadNode, wordlist);

            // as the headnode is now maintained by wordslist, no need to pass it as argument
            // wordsList.GenerateWordsList(ref dlbHeadNode, wordlist);
            wordsList.GenerateWordsList(wordlist);

            // load the hotboxes positions and sizes from the Config.ini if available
            new ConfigurationManager(localeManager);

            // Initialise the SDL window, renderer and the textures
            // var SpriteManager = new SpriteManager(loggingservice.GetRequiredService<ILogger<SpriteManager>>());

            // Initiate the SDL sounds system if possible
            // new SoundManager();


            // [ ]: as the anagrams list is now maintained by wordslist, we do not need to pass it to NewGame and GameLoop
            // Initialise the game
            // gameManager.NewGame(ref headNode, dlbHeadNode, GameManagerVariables.renderer, ref letters);
            gameManager.NewGame(ref headNode, GameManagerVariables.renderer, ref letters);

            // Run the main loop until the game is quit.
            // gameManager.GameLoop(ref headNode, dlbHeadNode, GameManagerVariables.renderer, ref letters);
            gameManager.GameLoop(ref headNode, GameManagerVariables.renderer, ref letters);

            /* tidy up and exit */

            // As we don't know if the garbace collector will run the destructors, do this manually.
            // not really needed but done for the sake of keeping with the original code
            // SoundManager.SoundManagerExit();
    
            // [ ]: as the anagrams list is now maintained by wordslist, we do not need to pass it to NewGame and GameLoop
            // wordsList.WordsListExit(ref dlbHeadNode);
            wordsList.WordsListExit();

            anagramsManager.AnagramsManagerExit(ref letters, ref headNode);
            spriteManager.SpriteManagerExit();

            return 0;
        }
    }
}