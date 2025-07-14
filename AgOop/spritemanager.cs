using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using SDL2;

namespace AgOop
{
    internal static class SpriteConstants
    {
        // Large letters graphics dimensions and spacing and displacement speed
        /// <summary>Width of the graphic of a letter in the "band" image file</summary>
        internal const int GAME_LETTER_WIDTH = 80;
        
        /// <summary>Height of the graphic of a letter in the "band" image file</summary>
        internal const int GAME_LETTER_HEIGHT = 90;
        
        /// <summary>Separation between the graphics of two letters in the SHUFFLE box</summary>
        internal const int GAME_LETTER_SPACE = 2;

        /// <summary> letter sprite fast speed </summary>
        internal const int LETTER_FAST = 30;

        /// <summary> letter sprite slow speed </summary>
        internal const int LETTER_SLOW = 10;

        /// <summary> letter sprite slow speed </summary>
        internal const int SPACE_POS_IN_NUMBERSGRAPHICS = 11;

        // constant used to convert a letter value to a number ('a' = 0, 'b' = 1...)
        // used for selecting the correct letter on the spriteband
        /// <summary> ASCII OFFSET to convert a number to its character equivalent </summary>
        internal const int NUM_TO_CHAR = 48;

    }

    internal static class Sprites
    {
        // SDL summarys

        // - SDL Textures    
        /// <summary>the SDL texture containing the background image</summary>
        internal static IntPtr backgroundTex = IntPtr.Zero;

        /// <summary>The SDL texture containing all the large letters</summary>
        // internal IntPtr letterBank = IntPtr.Zero;
        internal static IntPtr letterBank = IntPtr.Zero;

        /// <summary>The SDL texture containing all the small letters</summary>
        internal static IntPtr smallLetterBank = IntPtr.Zero;

        /// <summary>The SDL texture containing all the numbers</summary>
        internal static IntPtr numberBank = IntPtr.Zero;

        // textures for the (small) answer boxes
        /// <summary>answerBoxUnknown</summary>
        internal static IntPtr answerBoxUnknown = IntPtr.Zero;

        /// <summary>answerBoxKnown</summary>
        internal static IntPtr answerBoxKnown = IntPtr.Zero;


        // The clock and score sprite representations
        /// <summary>The list of sprites containing the graphical time representation</summary>
        internal static Sprite clockSprite = new Sprite(5);

        /// <summary>The list of sprites containing the graphical score representation</summary>
        internal static Sprite scoreSprite = new Sprite(5);
    }

    internal abstract class ISpriteManager
    {
        //TODO: Define the interface for the SpriteManager
        
    }

    /// <summary> Manages the sprites creation, modification, rendering </summary>
    internal class SpriteManager
    {
        private readonly ILogger<SpriteManager> _logger;
        // private readonly LocaleManager _localeManager;
        private readonly LocaleSettings _localeSettings;

        // internal LocaleManager? _localeManager;
        // internal GameManager? _gameManager;
        // private static readonly AgOopLogger logger = new AgOopLogger("SpriteManager");

        /// <summary> Constructor </summary>
        public SpriteManager(ILogger<SpriteManager> logger, LocaleSettings localeSettings)
        // public SpriteManager(ILogger<SpriteManager> logger, LocaleManager localeManager)
        {
            // TODO: 
            // initialise screen textures
            // Console.WriteLine("SpriteManager Constructor");

            // Inject logger and localeManager 
            _logger = logger;
            _localeSettings = localeSettings;
            // _localeManager = localeManager;

            // _gameManager = gameManager;

            // load and initialise textures
            Initialise();
        }


        // // SDL summarys

        // // - SDL Textures    
        // /// <summary>the SDL texture containing the background image</summary>
        // internal IntPtr backgroundTex = IntPtr.Zero;

        // /// <summary>The SDL texture containing all the large letters</summary>
        // internal IntPtr letterBank = IntPtr.Zero;

        // /// <summary>The SDL texture containing all the small letters</summary>
        // internal IntPtr smallLetterBank = IntPtr.Zero;

        // /// <summary>The SDL texture containing all the numbers</summary>
        // internal IntPtr numberBank = IntPtr.Zero;

        // // textures for the (small) answer boxes
        // /// <summary>answerBoxUnknown</summary>
        // internal IntPtr answerBoxUnknown = IntPtr.Zero;

        // /// <summary>answerBoxKnown</summary>
        // internal IntPtr answerBoxKnown = IntPtr.Zero;


        // // The clock and score sprite representations
        // /// <summary>The list of sprites containing the graphical time representation</summary>
        // internal Sprite clockSprite = new Sprite(5);

        // /// <summary>The list of sprites containing the graphical score representation</summary>
        // internal Sprite scoreSprite = new Sprite(5);


        /// <summary>The time for which the last tick sound of the clock was requested</summary>
        internal int _lastClockTick;


        /// <summary> Scaling factors related to the new dimensions of a resized window from 600x800 </summary>
        static double scalew = 1;
        static double scaleh = 1;


        internal void Initialise()
        {

            _logger.LogInformation("SpriteManager Constructor");

            if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_TIMER) < 0)
            {
                // Console.WriteLine($"Unable to unit SDL: {SDL.SDL_GetError()}");
                _logger.LogError($"Unable to unit SDL: {SDL.SDL_GetError()}");
                Console.ReadLine();
            }

            GameManagerVariables.window = SDL.SDL_CreateWindow("Anagramarama",
                                                                SDL.SDL_WINDOWPOS_UNDEFINED,
                                                                SDL.SDL_WINDOWPOS_UNDEFINED,
                                                                800,
                                                                600, SDL
                                                                .SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            if (GameManagerVariables.window == IntPtr.Zero)
            {
                // Console.WriteLine($"Unable to set 800x600 video:  {SDL.SDL_GetError()}");
                _logger.LogWarning($"Unable to set 800x600 video:  {SDL.SDL_GetError()}");
                Console.ReadLine();
            }

            GameManagerVariables.renderer = SDL.SDL_CreateRenderer(GameManagerVariables.window,
                                              -1,
                                              SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                                              SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            if (GameManagerVariables.renderer == IntPtr.Zero)
            {
                // Console.WriteLine($"Error creating the renderer: {SDL.SDL_GetError()}");
                _logger.LogError($"Error creating the renderer: {SDL.SDL_GetError()}");
                Console.ReadLine();

            }
            SDL.SDL_SetRenderDrawColor(GameManagerVariables.renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(GameManagerVariables.renderer);
            SDL.SDL_RenderPresent(GameManagerVariables.renderer);

            /* cache in-game graphics */
            // string imagesPath = LocaleManager.basePath + LocaleManager.i18nPath + _localeManager.localePath + _localeManager.imagesSubPath;
            // string imagesPath = _localeManager.language + _localeManager.imagesSubPath;
            // string imagesPath = _localeSettings.imagesPath;
            // string imagesPath = _localeManager.imagesSubPath;

            string backgroundFileLocation = Path.Join(_localeSettings.imagesPath, "background.png");
            if (!File.Exists(backgroundFileLocation))
            {
                // Console.WriteLine("problem with background file");
                _logger.LogError($"Background picture file not found at {backgroundFileLocation}");
                // Console.ReadLine("Please press a key");

            }
            IntPtr backgroundSurf = SDL_image.IMG_Load(Path.Join(backgroundFileLocation));
            if (backgroundSurf == IntPtr.Zero)
            {
                // Console.WriteLine($"Error loading background image: {SDL.SDL_GetError()}");

                _logger.LogError($"Error loading background image: {SDL.SDL_GetError()}");
            }
            // if (backgroundSurf == IntPtr.Zero)
            // {
            //     string error = SDL.SDL_GetError();
            //     Console.WriteLine("SDL Error: " + error);
            // }
            Sprites.backgroundTex = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, backgroundSurf);
            SDL.SDL_FreeSurface(backgroundSurf);

            string letterBankFileLocation = Path.Join(_localeSettings.imagesPath, "letterBank.png");
            if (!File.Exists(letterBankFileLocation))
            {
                // Console.WriteLine("problem with letterBank file");
                _logger.LogError($"Letter bank picture file not found at {letterBankFileLocation}");
                Console.ReadLine();
            }
            IntPtr letterSurf = SDL_image.IMG_Load(letterBankFileLocation);
            Sprites.letterBank = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, letterSurf);
            SDL.SDL_FreeSurface(letterSurf);

            string smallLetterBankFileLocation = Path.Join(_localeSettings.imagesPath, "smallLetterBank.png");
            if (!File.Exists(smallLetterBankFileLocation))
            {
                // Console.WriteLine("problem with smallLetterBank file");
                _logger.LogError($"Small letter bank picture file not found at {smallLetterBankFileLocation}");
                Console.ReadLine();
            }
            IntPtr smallLetterSurf = SDL_image.IMG_Load(smallLetterBankFileLocation);
            Sprites.smallLetterBank = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, smallLetterSurf);
            SDL.SDL_FreeSurface(smallLetterSurf);


            string numberBankFileLocation = Path.Join(_localeSettings.imagesPath, "numberBank.png");
            if (!File.Exists(numberBankFileLocation))
            {
                // Console.WriteLine("problem with numberBank file");
                _logger.LogError($"Number bank picture file not found at {numberBankFileLocation}");
                Console.ReadLine();
            }
            IntPtr numberSurf = SDL_image.IMG_Load(numberBankFileLocation);
            Sprites.numberBank = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, numberSurf);
            SDL.SDL_FreeSurface(numberSurf);

            _lastClockTick = 11;

            // Console.WriteLine("SpriteManager Constructor DONE");
            _logger.LogInformation("SpriteManager Constructor DONE");
        }



        internal void SpriteManagerExit()
        {
            // Console.WriteLine("SpriteManager destructor");
            _logger.LogInformation("SpriteManager Destructor");

            //TODO: make letters a a spritemanager field 
            // DestroyLetters(_letters); 
            SDL.SDL_DestroyTexture(Sprites.letterBank);
            SDL.SDL_DestroyTexture(Sprites.smallLetterBank);
            SDL.SDL_DestroyTexture(Sprites.numberBank);
            SDL.SDL_DestroyTexture(Sprites.backgroundTex);
            SDL.SDL_DestroyRenderer(GameManagerVariables.renderer);
            SDL.SDL_DestroyWindow(GameManagerVariables.window);

        }

        /// <summary> Displays a sprite on the screen </summary>
        /// <param name="screen">The renderer to display on</param>
        /// <param name="movie">The sprite to use</param>
        /// <returns>Nothing</returns>
        internal void ShowSprite(IntPtr screen, Sprite movie)
        {
            SDL.SDL_Rect rect = new SDL.SDL_Rect()
            {
                x = movie.x,
                y = movie.y,
                w = movie.w,
                h = movie.h
            };

            for (int i = 0; i < movie.numSpr; i++)
            {
                rect.x = movie.x + movie.sprite[i].sprite_x_offset;
                rect.y = movie.y + movie.sprite[i].sprite_y_offset;
                rect.w = movie.sprite[i].sprite_band_dimensions.w;
                rect.h = movie.sprite[i].sprite_band_dimensions.h;
                SDLScale_RenderCopy(screen, movie.sprite[i].sprite_band_texture, movie.sprite[i].sprite_band_dimensions, rect);
            }
        }


        /// <summary> checks if a sprite needs to move </summary>
        /// <param name="sprite">The sprite tested</param>
        /// <returns>true if this sprite needs to move</returns>
        internal bool IsSpriteMoving(Sprite sprite)
        {
            return (sprite.y != sprite.toY) || (sprite.x != sprite.toX);
        }


        /// <summary> checks if any sprite needs to move </summary>
        /// <param name="letters">The sprite tested</param>
        /// <returns>false if a sprite needs to move</returns>
        internal bool AnySpriteMoving(Sprite letters)
        {
            Sprite? current = letters;

            while (current != null)
            {
                if (IsSpriteMoving(current))
                {
                    return false;
                }
                current = current.next;
            }


            return true;
        }


        /// <summary> Moves a sprite </summary>
        /// <param name="screen">The renderer</param>
        /// <param name="movie">The sprite to move</param>
        /// <param name="letterSpeed">The speed to move the sprite at</param>
        /// <returns>Nothing</returns>
        // TODO: Optimise
        internal void MoveSprite(IntPtr screen, Sprite movie, int letterSpeed)
        {
            int Xsteps;

            // new, for efficiency
            if (movie.x == movie.toX && movie.y == movie.toY) return;

            // move a sprite from its curent location to the new location
            if ((movie.y != movie.toY) || (movie.x != movie.toX))
            {
                int x = movie.toX - movie.x;
                int y = movie.toY - movie.y;

                if (y != 0)
                {
                    if (x < 0)
                    {
                        x *= -1;
                    }
                    if (y < 0)
                    {
                        y *= -1;
                    }
                    Xsteps = (x / y) * letterSpeed;
                }
                else
                {
                    Xsteps = letterSpeed;
                }

                for (int i = 0; i < Xsteps; i++)
                {
                    if (movie.x < movie.toX)
                    {
                        movie.x++;
                    }
                    if (movie.x > movie.toX)
                    {
                        movie.x--;
                    }
                }

                for (int i = 0; i < letterSpeed; i++)
                {
                    if (movie.y < movie.toY)
                    {
                        movie.y++;
                    }
                    if (movie.y > movie.toY)
                    {
                        movie.y--;
                    }
                }
            }
        }


        /// <summary>Animate the moving of the sprites </summary>
        /// <param name="screen">the renderer the sprites move on</param>
        /// <param name="letters">the sprites to move</param>
        /// <param name="letterSpeed">the speed of the move</param>
        /// <returns>Nothing</returns>
        internal void MoveSprites(IntPtr screen, Sprite? letters, int letterSpeed)
        {
            Sprite? current = letters;

            while (current != null)
            {
                MoveSprite(screen, current, letterSpeed);
                current = current.next;
            }

            current = letters;
            while (current != null)
            {
                ShowSprite(screen, current);
                current = current.next;
            }
            // SDL.SDL_RenderPresent(screen);

        }


        /// <summary> identify the location of the mouse event if the window was scaled </summary>
        /// <param name="mouseEvent">the mouse event</param>
        /// <returns>Nothing</returns>
        internal void SDLScale_MouseEvent(ref SDL.SDL_Event mouseEvent)
        {
            mouseEvent.button.x = (int)(mouseEvent.button.x / scalew);
            mouseEvent.button.y = (int)(mouseEvent.button.y / scaleh);
        }


        /// <summary> scales a texture according to the requirements </summary>
        /// <param name="renderer">The renderer on which the scaling happens</param>
        /// <param name="texture">The texture to scale</param>
        /// <param name="srcRect">The original size, if any</param>
        /// <param name="dstRect">The scaled rectangle</param>
        /// <returns>Nothing</returns>
        internal void SDLScale_RenderCopy(IntPtr renderer, IntPtr texture, SDL.SDL_Rect? srcRect, SDL.SDL_Rect? dstRect)
        {

            if (dstRect.HasValue)
            {
                SDL.SDL_Rect dstReal;
                SDL.SDL_Rect dstRectToSend = (SDL.SDL_Rect)dstRect;

                dstReal.x = (int)(dstRectToSend.x * scalew);
                dstReal.y = (int)(dstRectToSend.y * scaleh);
                dstReal.h = (int)(dstRectToSend.h * scaleh);
                dstReal.w = (int)(dstRectToSend.w * scalew);

                if (srcRect.HasValue)
                {
                    SDL.SDL_Rect srcRectToSend;
                    srcRectToSend = (SDL.SDL_Rect)srcRect;
                    int sdlRtn = SDL.SDL_RenderCopy(renderer, texture, ref srcRectToSend, ref dstReal);
                    if (sdlRtn != 0)
                    {
                        // Console.WriteLine("Problem with RenderCopy in SDLScale_RenderCopy 1");
                        _logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 1");
                        Console.ReadLine();
                    }
                }
                else
                {
                    int sdlRtn = SDL.SDL_RenderCopy(renderer, texture, (nint)null, ref dstReal);
                    if (sdlRtn != 0)
                    {
                        // Console.WriteLine("Problem with RenderCopy in SDLScale_RenderCopy 2 ");
                        _logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 2");
                        Console.ReadLine();
                    }
                }
            }

            else
            {

                if (srcRect.HasValue)
                {
                    SDL.SDL_Rect srcRectToSend;
                    srcRectToSend = (SDL.SDL_Rect)srcRect;
                    int sdlRtn = SDL.SDL_RenderCopy(renderer, texture, ref srcRectToSend, (nint)null);
                    if (sdlRtn != 0)
                    {
                        // Console.WriteLine("Problem with RenderCopy in SDLScale_RenderCopy 3 ");
                        _logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 3");
                        Console.ReadLine();
                    }
                }
                else
                {
                    int sdlRtn = SDL.SDL_RenderCopy(renderer, texture, (nint)null, (nint)null);
                    if (sdlRtn != 0)
                    {
                        // Console.WriteLine("Problem with RenderCopy in SDLScale_RenderCopy 4 ");
                        _logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 4");
                        Console.ReadLine();
                    }
                }
            }
        }


        /// <summary> applies the scaling factor in run-time changes </summary>
        /// <param name="w">width factor</param>
        /// <param name="h">height factor</param>
        /// <returns>Nothing</returns>
        internal void SDLScaleSet(double w, double h)
        {
            scalew = w;
            scaleh = h;
        }



        /// <summary> load the named image to position x,y onto the required surface 
        /// NOTE: Unused functionality
        /// </summary>
        /// <param name="file">the filename to load (.BMP)</param>
        /// <param name="screen">the SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal void ShowBMP(string file, IntPtr screen)
        {
            IntPtr imageSurf;
            IntPtr image;
            SDL.SDL_Rect dest;

            // load the BMP file into a surface
            imageSurf = SDL.SDL_LoadBMP(file);
            if (imageSurf == IntPtr.Zero)
            {
                // Console.WriteLine("Couldn't load %s: %s\n", file, SDL.SDL_GetError());
                _logger.LogError($"Couldn't load {file}: {SDL.SDL_GetError()}");
                return;
            }
            dest.x = 0;
            dest.y = 0;
            dest.w = 800;
            dest.h = 600;
            image = SDL.SDL_CreateTextureFromSurface(screen, imageSurf);
            SDLScale_RenderCopy(screen, image, null, dest);

            // Update the changed portion of the screen
            SDL.SDL_FreeSurface(imageSurf);
            SDL.SDL_DestroyTexture(image);
        }


        /// <summary> Creates the SDL surfaces of the small answer boxes if they do not exist </summary>
        /// <param name="screen">The renderer</param>
        internal void CreateAnswerBoxesSurfaces(IntPtr screen)
        {
            //answerBoxUnknown: empty answer box as it has not been found
            //amswerBoxKnow: contains the known letter on a white background

            // create the unknown and known (small) answerbox texture
            if (Sprites.answerBoxUnknown == IntPtr.Zero)
            {
                // coordinates of the outer rectangle (border rectangle -/+ boder thickness)
                SDL.SDL_Rect outerRect = new SDL.SDL_Rect
                {
                    w = 16,
                    h = 16,
                    x = 0,
                    y = 0
                };

                // coordinate of the inner rectangle (border rectangle -/+ boder thickness)
                SDL.SDL_Rect innerRect = new SDL.SDL_Rect
                {
                    w = outerRect.w - 1,
                    h = outerRect.h - 1,
                    x = outerRect.x + 1,
                    y = outerRect.y + 1
                };

                // create an empty texture
                IntPtr box = SDL.SDL_CreateRGBSurface(0, 16, 16, 32, 0, 0, 0, 0);

                // draw the border rectangle on the texture
                SDL.SDL_FillRect(box, ref outerRect, 0);

                // Get a pointer to the surface of the box
                SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(box);

                // fill in the texture with the inner rectangle, 
                // applying the surface format of the texture with light blue colour
                SDL.SDL_FillRect(box, ref innerRect, SDL.SDL_MapRGB(surface.format, 217, 220, 255));
                // set this to the unknown answerbox as a texture
                Sprites.answerBoxUnknown = SDL.SDL_CreateTextureFromSurface(screen, box);

                // fill in the texture with the inner rectangle, 
                // applying the surface format of the texture with white colour
                SDL.SDL_FillRect(box, ref innerRect, SDL.SDL_MapRGB(surface.format, 255, 255, 255));
                // set this to the known answerbox as a texture
                Sprites.answerBoxKnown = SDL.SDL_CreateTextureFromSurface(screen, box);

                // free the memory used by the temporary surface
                SDL.SDL_FreeSurface(box);
            }
        }


        /// <summary> renders the answer boxes (small boxes at the bottom) onto the surface </summary>
        /// <param name="headNode">The head node of anagrams (with the info on if they have been found or guessed)</param>
        /// <param name="screen">The renderer</param>
        /// <returns>Nothing</returns>
        internal void DisplayAnswerBoxes(Anagrams.Node? headNode, IntPtr screen)
        {
            Anagrams.Node? current = headNode;
            int numWords = 0;
            int acrossOffset = 70;
            int numLetters = 0;
            int listLetters = 0;

            SDL.SDL_Rect outerRect = new SDL.SDL_Rect
            {
                w = 16,
                h = 16,
                x = 0,
                y = 0
            };

            // coordinate of the inner rectangle (border rectangle -/+ boder thickness)
            SDL.SDL_Rect innerRect = new SDL.SDL_Rect
            {
                w = outerRect.w - 1,
                h = outerRect.h - 1,
                x = outerRect.x + 1,
                y = outerRect.y + 1
            };

            SDL.SDL_Rect letterBankRect = new SDL.SDL_Rect
            {
                w = 10,
                h = 16,
                x = 0,
                y = 0 // letter is chosen by 10x letter where a is 0
            };

            CreateAnswerBoxesSurfaces(screen);

            // TODO: Cleanup below
            // //answerBoxUnknown: empty answer box as it has not been found
            // //amswerBoxKnow: contains the known letter on a white background

            // // create the unknown and known (small) answerbox texture
            // if (answerBoxUnknown == IntPtr.Zero)
            // {
            //     // create an empty texture
            //     IntPtr box = SDL.SDL_CreateRGBSurface(0, 16, 16, 32, 0, 0, 0, 0);

            //     // draw the border rectangle on the texture
            //     SDL.SDL_FillRect(box, ref outerRect, 0);

            //     // Get a pointer to the surface of the box
            //     SDL.SDL_Surface surface = Marshal.PtrToStructure<SDL.SDL_Surface>(box);

            //     // fill in the texture with the inner rectangle, 
            //     // applying the surface format of the texture with light blue colour
            //     SDL.SDL_FillRect(box, ref innerRect, SDL.SDL_MapRGB(surface.format, 217, 220, 255));
            //     // set this to the unknown answerbox as a texture
            //     answerBoxUnknown = SDL.SDL_CreateTextureFromSurface(screen, box);

            //     // fill in the texture with the inner rectangle, 
            //     // applying the surface format of the texture with white colour
            //     SDL.SDL_FillRect(box, ref innerRect, SDL.SDL_MapRGB(surface.format, 255, 255, 255));
            //     // set this to the known answerbox as a texture
            //     answerBoxKnown = SDL.SDL_CreateTextureFromSurface(screen, box);

            //     // free the memory used by the temporary surface
            //     SDL.SDL_FreeSurface(box);
            // }

            // Width and height are always the same
            outerRect.x = acrossOffset;
            outerRect.y = 380;
            innerRect.x = outerRect.x + 1;

            while (current != null)
            {
                // new word, start at letter 0
                numWords++;
                numLetters = 0;

                // update the x for each letter
                for (int i = 0; i < current.length; i++)
                {
                    numLetters++;

                    // render the type of box for that letter
                    SDLScale_RenderCopy(screen, current.guessed ? Sprites.answerBoxKnown : Sprites.answerBoxUnknown, null, outerRect);
                    // if (current.guessed)
                    // {
                    //     SDLScale_RenderCopy(screen, answerBoxKnown, null, outerRect);
                    // }
                    // else
                    // {
                    //     SDLScale_RenderCopy(screen, answerBoxUnknown, null, outerRect);
                    // }

                    // realign the inner and outer boxes?!?
                    innerRect.w = outerRect.w - 1;
                    innerRect.h = outerRect.h - 1;
                    innerRect.x = outerRect.x + 1;
                    innerRect.y = outerRect.y + 1;

                    // if the word was found, display the letter inside that box
                    if (current.found)
                    {
                        int c = (int)(current.anagram[i] - 'a');

                        letterBankRect.x = 10 * c;

                        innerRect.x += 2;
                        innerRect.w = letterBankRect.w;
                        innerRect.h = letterBankRect.h;
                        SDLScale_RenderCopy(screen, Sprites.smallLetterBank, letterBankRect, innerRect);
                    }

                    outerRect.x += 18;
                }

                if (numLetters > listLetters)
                {
                    listLetters = numLetters;
                }

                if (numWords == 11)
                {
                    numWords = 0;
                    acrossOffset += (listLetters * 18) + 9;
                    outerRect.y = 380;
                    outerRect.x = acrossOffset;
                }
                else
                {
                    outerRect.x = acrossOffset;
                    outerRect.y += 19;
                }

                current = current.next;
            }
        }


        /// <summary>Build the SHUFFLE box shuffled word into linked list of letter graphics </summary>
        /// <param name="letters">letter sprites head node (in/out)</param>
        /// <param name="screen">SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal void BuildLetters(ref Sprite? letters, IntPtr screen)
        {
            Sprite thisLetter;
            Sprite previousLetter;

            int index = 0;

            Random random = new Random();

            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                y = 0,
                w = SpriteConstants.GAME_LETTER_WIDTH,
                h = SpriteConstants.GAME_LETTER_HEIGHT
            };

            int len = GameState.Shuffle.Length;
            // int len = _gameManager.Shuffle.Length;
            thisLetter = new Sprite(1);
            previousLetter = new Sprite(1);

            for (int i = 0; i < len; i++)
            {
                thisLetter.numSpr = 0;

                if (GameState.Shuffle[i] != AnagramsConstants.ASCII_SPACE && GameState.Shuffle[i] != AnagramsConstants.SPACE_CHAR)
                // if (_gameManager.Shuffle[i] != AnagramsConstants.ASCII_SPACE && _gameManager.Shuffle[i] != AnagramsConstants.SPACE_CHAR)
                {
                    int chr = (int)(GameState.Shuffle[i] - 'a');
                    // int chr = (int)(_gameManager.Shuffle[i] - 'a');
                    rect.x = chr * SpriteConstants.GAME_LETTER_WIDTH;
                    thisLetter.numSpr = 1;

                    thisLetter.sprite[0].sprite_band_texture = Sprites.letterBank;
                    thisLetter.sprite[0].sprite_band_dimensions = rect;
                    thisLetter.sprite[0].sprite_x_offset = 0;
                    thisLetter.sprite[0].sprite_y_offset = 0;

                    thisLetter.letter = GameState.Shuffle[i];
                    // thisLetter.letter = _gameManager.Shuffle[i];
                    // appear at random on the screen in x, y 
                    // To make the letter fly in from wherever on the screen to toX, toY location
                    thisLetter.x = random.Next(800);
                    thisLetter.y = random.Next(600);
                    thisLetter.h = SpriteConstants.GAME_LETTER_HEIGHT;
                    thisLetter.w = SpriteConstants.GAME_LETTER_WIDTH;
                    thisLetter.toX = i * (SpriteConstants.GAME_LETTER_WIDTH + SpriteConstants.GAME_LETTER_SPACE)
                                     + BoxConstants.BOX_START_X;
                    thisLetter.toY = BoxConstants.SHUFFLE_BOX_Y;
                    thisLetter.next = previousLetter;
                    thisLetter.box = BoxConstants.SHUFFLE;
                    thisLetter.index = index++;

                    // Add the letter sprite at the end of the letters linkedlised passed by ref
                    previousLetter = thisLetter;

                    // move down the linkedlist of letters
                    letters = thisLetter;

                    thisLetter = new Sprite(len);
                }
                else
                {
                    GameState.Shuffle.ToCharArray()[i] = AnagramsConstants.SPACE_CHAR;
                    // _gameManager.Shuffle.ToCharArray()[i] = AnagramsConstants.SPACE_CHAR;
                    // rect.x = 26 * GAME_LETTER_WIDTH;
                }

            }
        }


        /// <summary> Creates the inital score structure
        /// as a single node in the same linkedlist as anagrams,
        /// containing 5 graphics ("0    ")
        /// </summary>
        /// <param name="letters">letter sprites head node (in/out)</param>
        /// <param name="screen">SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal void AddScore(ref Sprite? letters, IntPtr screen)
        {
            Sprite thisLetter;
            Sprite? previousLetter = null;
            Sprite? current = letters;

            int index = 0;

            SDL.SDL_Rect fromRect = new SDL.SDL_Rect  // dimensions of the numbers band image in px
            {
                y = 0,
                w = BoxConstants.SCORE_WIDTH,
                h = BoxConstants.SCORE_HEIGHT
            };

            SDL.SDL_Rect toRect = new SDL.SDL_Rect     // position of the letter in the score box
            {
                y = 0,
                w = BoxConstants.SCORE_WIDTH,
                h = BoxConstants.SCORE_HEIGHT,
            };

            if (current == null)
            {
                // Console.WriteLine("AddScore: Error navigating to the end of the letters list, no letters yet");
                _logger.LogError("AddScore: Error navigating to the end of the letters list, no letters yet");
                return;
            }

            // Traverse to the end of the letters linkedlist
            while (current != null)
            {
                previousLetter = current;
                current = current.next;
            }

            // Create a new node containing 5 sprites for the score
            thisLetter = new Sprite(5);
            thisLetter.numSpr = 5;

            // pre-loading: "0    "
            for (int i = 0; i < 5; i++)
            {
                // The character in the numband corresponding to 0 or ' ' (character 0x11)
                fromRect.x = i == 0 ? 0 : BoxConstants.SCORE_WIDTH * SpriteConstants.SPACE_POS_IN_NUMBERSGRAPHICS;
                // The position in the Score box
                toRect.x = BoxConstants.SCORE_WIDTH * i;

                thisLetter.sprite[i].sprite_band_texture = Sprites.numberBank;
                thisLetter.sprite[i].sprite_band_dimensions = fromRect;
                thisLetter.sprite[i].sprite_x_offset = toRect.x;
                thisLetter.sprite[i].sprite_y_offset = 0;
            }

            thisLetter.x = BoxConstants.SCORE_X;
            thisLetter.y = BoxConstants.SCORE_Y;
            thisLetter.h = BoxConstants.SCORE_HEIGHT;
            thisLetter.w = BoxConstants.SCORE_WIDTH * 5; // initialise the first score on the RHS of the box
            thisLetter.toX = thisLetter.x;
            thisLetter.toY = thisLetter.y;
            thisLetter.next = null;
            thisLetter.box = BoxConstants.CONTROLS;
            thisLetter.index = index++;

            previousLetter!.next = thisLetter;
            Sprites.scoreSprite = thisLetter;
        }


        // TODO: Probably remove the signature argument.
        /// <summary>Renders the score graphically on the SDL surface</summary>
        /// <param name="screen">the SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal void UpdateScore(IntPtr screen)
        {

            SDL.SDL_Rect fromRect = new SDL.SDL_Rect
            {
                y = 0,
                w = BoxConstants.SCORE_WIDTH,
                h = BoxConstants.SCORE_HEIGHT
            };

            SDL.SDL_Rect toRect = new SDL.SDL_Rect
            {
                y = 0,
                w = BoxConstants.SCORE_WIDTH,
                h = BoxConstants.SCORE_HEIGHT
            };

            // string buffer = _gameManager.totalScore.ToString();
            string buffer = GameState.totalScore.ToString();

            for (int i = 0; i < buffer.Length; i++)
            {
                // The position in the number band for the number chraracter
                fromRect.x = BoxConstants.SCORE_WIDTH * (buffer[i] - SpriteConstants.NUM_TO_CHAR);
                // The position in the scorebopx
                toRect.x = BoxConstants.SCORE_WIDTH * i; // The location in the score box
                Sprites.scoreSprite.sprite[i].sprite_band_dimensions = fromRect;
                Sprites.scoreSprite.sprite[i].sprite_x_offset = toRect.x;
            }
        }


        /// <summary>Creates the inital clock structure as a single node in letters, containing 5 graphics
        /// this sets the clock to a fixed 5:00 start 
        /// </summary>
        /// <param name="letters">letter sprites head node (in/out)</param>
        /// <param name="screen">SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal void AddClock(ref Sprite? letters, IntPtr screen)
        {
            Sprite thisLetter;
            Sprite? previousLetter = null;
            Sprite? current = letters;
            int index = 0;

            SDL.SDL_Rect fromRect = new SDL.SDL_Rect
            {
                y = 0,
                w = BoxConstants.CLOCK_WIDTH,
                h = BoxConstants.CLOCK_HEIGHT
            };

            if (current == null)
            {
                // Console.WriteLine("AddClock: Error navigating to the end of the letters list, no letters yet");
                _logger.LogError("AddClock: Error navigating to the end of the letters list, no letters yet");
                return;
            }

            // go to the end of the anagrams linked list
            while (current.next != null)
            {
                previousLetter = current;
                current = current.next;
            }

            thisLetter = new Sprite(5); // 5 characters in the clock
            thisLetter.numSpr = 5;

            // initialise with 05:00
            // TODO: Probably could be done better - as in using the "AvailableTime" value
            for (int i = 0; i < 5; i++)
            {

                switch (i)
                {
                    case 0: // tens of mins ("0") = number 0 on the NumberBank graphic
                        fromRect.x = 0;
                        break;
                    case 1:
                        fromRect.x = 5 * BoxConstants.CLOCK_WIDTH; // units of minutes ("5")
                        break;
                    case 2:
                        fromRect.x = 10 * BoxConstants.CLOCK_WIDTH; // colon (":")
                        break;
                    case 3: // tens of secs ("0")
                        fromRect.x = 0;
                        break;
                    case 4: // units of secs ("0")
                        fromRect.x = 0;
                        break;
                    default:
                        break;
                }

                thisLetter.sprite[i].sprite_band_texture = Sprites.numberBank;
                thisLetter.sprite[i].sprite_band_dimensions = fromRect;
                thisLetter.sprite[i].sprite_x_offset = BoxConstants.CLOCK_WIDTH * i;
                thisLetter.sprite[i].sprite_y_offset = 0;
            }

            thisLetter.x = BoxConstants.CLOCK_X;
            thisLetter.y = BoxConstants.CLOCK_Y;
            thisLetter.h = BoxConstants.CLOCK_HEIGHT;
            thisLetter.w = BoxConstants.CLOCK_WIDTH * thisLetter.numSpr;
            thisLetter.toX = thisLetter.x;
            thisLetter.toY = thisLetter.y;
            thisLetter.next = null;
            thisLetter.box = BoxConstants.CONTROLS;
            thisLetter.index = index++;

            previousLetter!.next = thisLetter;
            Sprites.clockSprite = thisLetter;

        }


        /// <summary>Renders the time graphically on the SDL surface</summary>
        /// <param name="screen">the SDL_Surface on which to display the image</param>
        /// <returns>Nothing</returns>
        internal void UpdateTime(IntPtr screen)
        {
            SDL.SDL_Rect fromRect = new SDL.SDL_Rect
            {
                y = 0, // position on the numberbank band (x CLOCKHEIGHT) - but as only one line, always 0
                w = BoxConstants.CLOCK_WIDTH, // width of the character in the timebox in px
                h = BoxConstants.CLOCK_HEIGHT // height of the character in px
            };

            int thisTime = GameManagerVariables.AVAILABLE_TIME - GameState.gameTime;
            // int thisTime = GameManagerVariables.AVAILABLE_TIME - _gameManager.gameTime;
            TimeSpan remainingTime = TimeSpan.FromSeconds(thisTime);

            int minutes = remainingTime.Minutes;
            int minutesTens = minutes / 10;
            int minutesUnits = minutes % 10;
            int seconds = remainingTime.Seconds;
            int secondsTens = seconds / 10;
            int secondsUnits = seconds % 10;

            // clockSprite = new Sprite(5);
            fromRect.x = BoxConstants.CLOCK_WIDTH * minutesTens;
            Sprites.clockSprite.sprite[0].sprite_band_dimensions = fromRect;
            fromRect.x = BoxConstants.CLOCK_WIDTH * minutesUnits;
            Sprites.clockSprite.sprite[1].sprite_band_dimensions = fromRect;
            fromRect.x = BoxConstants.CLOCK_WIDTH * secondsTens;
            Sprites.clockSprite.sprite[3].sprite_band_dimensions = fromRect;
            fromRect.x = BoxConstants.CLOCK_WIDTH * secondsUnits;
            Sprites.clockSprite.sprite[4].sprite_band_dimensions = fromRect;

            // tick out the last 10 seconds
            if (thisTime <= 10 && thisTime > 0)
            {
                if (thisTime < _lastClockTick)
                {
                    GameState.TickClock = true;
                    // _gameManager.TickClock = true;
                    _lastClockTick = thisTime;
                }
                // SoundManager.PlaySound("clock-tick");
            }

        }

        internal void ClearScreen()
        {
            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = 800,
                h = 600
            };
            SDLScale_RenderCopy(GameManagerVariables.renderer, Sprites.backgroundTex, null, dest);
        }


        /// <summary>Frees the sprite letters memory
        /// was needed in C but no longer in c# as handled by garbage collector
        /// Kept for the sake of keepting
        /// </summary>
        /// <param name="letters">the set of letters to remove from memory</param>
        /// <returns>Nothing</returns>
        internal void DestroyLetters(ref Sprite? letters)
        {
            letters = null;
        }



        // /// <summary> Returns a boolean indicating whether the click was inside a box </summary>
        // /// <param name="box"> The box </param>
        // /// <param name="x"> the x position clicked </param>
        // /// <param name="y">The y position clicked</param>
        // /// <returns>true if clicked inside the box, false otherwise</returns>
        // internal bool IsInside(Box box, int x, int y)
        // {
        //     return (x > box.x) && (x < (box.x + box.width)) &&
        //             (y > box.y) && (y < (box.y + box.height));
        // }



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
                        // if (Answer.ToCharArray()[i] == AnagramsConstants.SPACE_CHAR)
                        if (GameState.Answer.ToCharArray()[i] == AnagramsConstants.SPACE_CHAR)
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
                        GameState.Answer = GameState.Answer[..i] + GameState.Shuffle[index] + GameState.Answer[(i + 1)..];
                        // Answer = Answer[..i] + Shuffle[index] + Answer[(i + 1)..];
                        // Shuffle.ToCharArray()[index] = AnagramsConstants.SPACE_CHAR;
                        GameState.Shuffle = GameState.Shuffle[..index] + AnagramsConstants.SPACE_CHAR + GameState.Shuffle[(index + 1)..];
                        // Shuffle = Shuffle[..index] + AnagramsConstants.SPACE_CHAR + Shuffle[(index + 1)..];
                    }
                    break;

                case BoxConstants.SHUFFLE:
                    for (i = 0; i < 7; i++)
                    {
                        if (GameState.Shuffle.ToCharArray()[i] == AnagramsConstants.SPACE_CHAR)
                        // if (Shuffle.ToCharArray()[i] == AnagramsConstants.SPACE_CHAR)
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
                        GameState.Shuffle = GameState.Shuffle[..i] + GameState.Answer[index] + GameState.Shuffle[(i + 1)..];
                        // Shuffle = Shuffle[..i] + Answer[index] + Shuffle[(i + 1)..];

                        // Answer.ToCharArray()[index] = AnagramsConstants.SPACE_CHAR;
                        GameState.Answer = GameState.Answer[..index] + AnagramsConstants.SPACE_CHAR + GameState.Answer[(index + 1)..];
                        // Answer = Answer[..index] + AnagramsConstants.SPACE_CHAR + Answer[(index + 1)..];
                    }
                    break;

                default:
                    break;
            }

            index = i;

            // return the toX position in the target box of the letter returned
            return i * (SpriteConstants.GAME_LETTER_WIDTH + SpriteConstants.GAME_LETTER_SPACE) + BoxConstants.BOX_START_X;
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
    }
}