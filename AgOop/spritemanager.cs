using System.Runtime.InteropServices;
using SDL2;

namespace AgOop
{
    internal class SpriteConstants
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

    /// <summary> Manages the sprites creation, modification, rendering </summary>
    internal class SpriteManager
    {
        private static readonly AgOopLogger logger = new AgOopLogger("SpriteManager");

        // SDL summarys

        // - SDL Textures    
        /// <summary>the SDL texture containing the background image</summary>
        internal static IntPtr backgroundTex = IntPtr.Zero;

        /// <summary>The SDL texture containing all the large letters</summary>
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


        /// <summary>The time for which the last tick sound of the clock was requested</summary>
        internal static int _lastClockTick;


        /// <summary> Scaling factors related to the new dimensions of a resized window from 600x800 </summary>
        static double scalew = 1;
        static double scaleh = 1;


        /// <summary> Constructor </summary>
        internal SpriteManager()
        {
            // TODO: 
            // initialise screen textures
            // load and initialise textures
            // Console.WriteLine("SpriteManager Constructor");
            logger.LogInformation("SpriteManager Constructor");

            if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_VIDEO | SDL.SDL_INIT_TIMER) < 0)
            {
                // Console.WriteLine($"Unable to unit SDL: {SDL.SDL_GetError()}");
                logger.LogError($"Unable to unit SDL: {SDL.SDL_GetError()}");
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
                logger.LogWarning($"Unable to set 800x600 video:  {SDL.SDL_GetError()}");
                Console.ReadLine();
            }

            GameManagerVariables.renderer = SDL.SDL_CreateRenderer(GameManagerVariables.window,
                                              -1,
                                              SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                                              SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            if (GameManagerVariables.renderer == IntPtr.Zero)
            {
                // Console.WriteLine($"Error creating the renderer: {SDL.SDL_GetError()}");
                logger.LogError($"Error creating the renderer: {SDL.SDL_GetError()}");
                Console.ReadLine();

            }
            SDL.SDL_SetRenderDrawColor(GameManagerVariables.renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(GameManagerVariables.renderer);
            SDL.SDL_RenderPresent(GameManagerVariables.renderer);

            /* cache in-game graphics */
            // string imagesPath = LocaleManager.basePath + LocaleManager.i18nPath + LocaleManager.localePath + LocaleManager.imagesSubPath;
            string imagesPath = LocaleManager.language + LocaleManager.imagesSubPath;

            if (!File.Exists(imagesPath + "background.png"))
            {
                // Console.WriteLine("problem with background file");
                logger.LogWarning($"Background picture file not found at {imagesPath + "background.png"}");
                Console.ReadLine();

            }
            IntPtr backgroundSurf = SDL_image.IMG_Load(imagesPath + "background.png");
            if (backgroundSurf == IntPtr.Zero)
            {
                // Console.WriteLine($"Error loading background image: {SDL.SDL_GetError()}");

                logger.LogError($"Error loading background image: {SDL.SDL_GetError()}");
            }
            // if (backgroundSurf == IntPtr.Zero)
            // {
            //     string error = SDL.SDL_GetError();
            //     Console.WriteLine("SDL Error: " + error);
            // }
            backgroundTex = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, backgroundSurf);
            SDL.SDL_FreeSurface(backgroundSurf);


            if (!File.Exists(imagesPath + "letterBank.png"))
            {
                // Console.WriteLine("problem with letterBank file");
                logger.LogError($"Letter bank picture file not found at {imagesPath + "letterBank.png"}");
                Console.ReadLine();
            }
            IntPtr letterSurf = SDL_image.IMG_Load(imagesPath + "letterBank.png");
            letterBank = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, letterSurf);
            SDL.SDL_FreeSurface(letterSurf);


            if (!File.Exists(imagesPath + "smallLetterBank.png"))
            {
                // Console.WriteLine("problem with smallLetterBank file");
                logger.LogError($"Small letter bank picture file not found at {imagesPath + "smallLetterBank.png"}");
                Console.ReadLine();
            }
            IntPtr smallLetterSurf = SDL_image.IMG_Load(imagesPath + "smallLetterBank.png");
            smallLetterBank = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, smallLetterSurf);
            SDL.SDL_FreeSurface(smallLetterSurf);


            if (!File.Exists(imagesPath + "numberBank.png"))
            {
                // Console.WriteLine("problem with numberBank file");
                logger.LogError($"Number bank picture file not found at {imagesPath + "numberBank.png"}");
                Console.ReadLine();
            }
            IntPtr numberSurf = SDL_image.IMG_Load(imagesPath + "numberBank.png");
            numberBank = SDL.SDL_CreateTextureFromSurface(GameManagerVariables.renderer, numberSurf);
            SDL.SDL_FreeSurface(numberSurf);

            _lastClockTick = 11;

            // Console.WriteLine("SpriteManager Constructor DONE");
            logger.LogInformation("SpriteManager Constructor DONE");
        }



        internal static void SpriteManagerExit()
        {
            // Console.WriteLine("SpriteManager destructor");
            logger.LogInformation("SpriteManager Destructor");

            SDL.SDL_DestroyTexture(letterBank);
            SDL.SDL_DestroyTexture(smallLetterBank);
            SDL.SDL_DestroyTexture(numberBank);
            SDL.SDL_DestroyTexture(backgroundTex);
            SDL.SDL_DestroyRenderer(GameManagerVariables.renderer);
            SDL.SDL_DestroyWindow(GameManagerVariables.window);

        }

        /// <summary> Displays a sprite on the screen </summary>
        /// <param name="screen">The renderer to display on</param>
        /// <param name="movie">The sprite to use</param>
        /// <returns>Nothing</returns>
        internal static void ShowSprite(IntPtr screen, Sprite movie)
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
        internal static bool IsSpriteMoving(Sprite sprite)
        {
            return (sprite.y != sprite.toY) || (sprite.x != sprite.toX);
        }


        /// <summary> checks if any sprite needs to move </summary>
        /// <param name="letters">The sprite tested</param>
        /// <returns>false if a sprite needs to move</returns>
        internal static bool AnySpriteMoving(Sprite letters)
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
        internal static void MoveSprite(IntPtr screen, Sprite movie, int letterSpeed)
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
        internal static void MoveSprites(IntPtr screen, Sprite? letters, int letterSpeed)
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
        internal static void SDLScale_MouseEvent(ref SDL.SDL_Event mouseEvent)
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
        internal static void SDLScale_RenderCopy(IntPtr renderer, IntPtr texture, SDL.SDL_Rect? srcRect, SDL.SDL_Rect? dstRect)
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
                        logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 1");
                        Console.ReadLine();
                    }
                }
                else
                {
                    int sdlRtn = SDL.SDL_RenderCopy(renderer, texture, (nint)null, ref dstReal);
                    if (sdlRtn != 0)
                    {
                        // Console.WriteLine("Problem with RenderCopy in SDLScale_RenderCopy 2 ");
                        logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 2");
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
                        logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 3");
                        Console.ReadLine();
                    }
                }
                else
                {
                    int sdlRtn = SDL.SDL_RenderCopy(renderer, texture, (nint)null, (nint)null);
                    if (sdlRtn != 0)
                    {
                        // Console.WriteLine("Problem with RenderCopy in SDLScale_RenderCopy 4 ");
                        logger.LogError("Problem with RenderCopy in SDLScale_RenderCopy 4");
                        Console.ReadLine();
                    }
                }
            }
        }


        /// <summary> applies the scaling factor in run-time changes </summary>
        /// <param name="w">width factor</param>
        /// <param name="h">height factor</param>
        /// <returns>Nothing</returns>
        internal static void SDLScaleSet(double w, double h)
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
        internal static void ShowBMP(string file, IntPtr screen)
        {
            IntPtr imageSurf;
            IntPtr image;
            SDL.SDL_Rect dest;

            // load the BMP file into a surface
            imageSurf = SDL.SDL_LoadBMP(file);
            if (imageSurf == IntPtr.Zero)
            {
                // Console.WriteLine("Couldn't load %s: %s\n", file, SDL.SDL_GetError());
                logger.LogError($"Couldn't load {file}: {SDL.SDL_GetError()}");
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
        internal static void CreateAnswerBoxesSurfaces(IntPtr screen)
        {
            //answerBoxUnknown: empty answer box as it has not been found
            //amswerBoxKnow: contains the known letter on a white background

            // create the unknown and known (small) answerbox texture
            if (answerBoxUnknown == IntPtr.Zero)
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
                answerBoxUnknown = SDL.SDL_CreateTextureFromSurface(screen, box);

                // fill in the texture with the inner rectangle, 
                // applying the surface format of the texture with white colour
                SDL.SDL_FillRect(box, ref innerRect, SDL.SDL_MapRGB(surface.format, 255, 255, 255));
                // set this to the known answerbox as a texture
                answerBoxKnown = SDL.SDL_CreateTextureFromSurface(screen, box);

                // free the memory used by the temporary surface
                SDL.SDL_FreeSurface(box);
            }
        }


        /// <summary> renders the answer boxes (small boxes at the bottom) onto the surface </summary>
        /// <param name="headNode">The head node of anagrams (with the info on if they have been found or guessed)</param>
        /// <param name="screen">The renderer</param>
        /// <returns>Nothing</returns>
        internal static void DisplayAnswerBoxes(Anagrams.Node? headNode, IntPtr screen)
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
                    SDLScale_RenderCopy(screen, current.guessed ? answerBoxKnown : answerBoxUnknown, null, outerRect);
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
                        SDLScale_RenderCopy(screen, smallLetterBank, letterBankRect, innerRect);
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
        internal static void BuildLetters(ref Sprite? letters, IntPtr screen)
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

            int len = GameManager.Shuffle.Length;
            thisLetter = new Sprite(1);
            previousLetter = new Sprite(1);

            for (int i = 0; i < len; i++)
            {
                thisLetter.numSpr = 0;

                if (GameManager.Shuffle[i] != AnagramsConstants.ASCII_SPACE && GameManager.Shuffle[i] != AnagramsConstants.SPACE_CHAR)
                {
                    int chr = (int)(GameManager.Shuffle[i] - 'a');
                    rect.x = chr * SpriteConstants.GAME_LETTER_WIDTH;
                    thisLetter.numSpr = 1;

                    thisLetter.sprite[0].sprite_band_texture = letterBank;
                    thisLetter.sprite[0].sprite_band_dimensions = rect;
                    thisLetter.sprite[0].sprite_x_offset = 0;
                    thisLetter.sprite[0].sprite_y_offset = 0;

                    thisLetter.letter = GameManager.Shuffle[i];
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
                    GameManager.Shuffle.ToCharArray()[i] = AnagramsConstants.SPACE_CHAR;
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
        internal static void AddScore(ref Sprite? letters, IntPtr screen)
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
                logger.LogError("AddScore: Error navigating to the end of the letters list, no letters yet");
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
                fromRect.x = i==0 ? 0 : BoxConstants.SCORE_WIDTH * SpriteConstants.SPACE_POS_IN_NUMBERSGRAPHICS;
                // The position in the Score box
                toRect.x = BoxConstants.SCORE_WIDTH * i;

                thisLetter.sprite[i].sprite_band_texture = numberBank;
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
            scoreSprite = thisLetter;
        }


        // TODO: Probably remove the signature argument.
        /// <summary>Renders the score graphically on the SDL surface</summary>
        /// <param name="screen">the SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal static void UpdateScore(IntPtr screen)
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

            string buffer = GameManager.totalScore.ToString();

            for (int i = 0; i < buffer.Length; i++)
            {
                // The position in the number band for the number chraracter
                fromRect.x = BoxConstants.SCORE_WIDTH * (buffer[i] - SpriteConstants.NUM_TO_CHAR); 
                // The position in the scorebopx
                toRect.x = BoxConstants.SCORE_WIDTH * i; // The location in the score box
                scoreSprite.sprite[i].sprite_band_dimensions = fromRect;
                scoreSprite.sprite[i].sprite_x_offset = toRect.x;
            }
        }


        /// <summary>Creates the inital clock structure as a single node in letters, containing 5 graphics
        /// this sets the clock to a fixed 5:00 start 
        /// </summary>
        /// <param name="letters">letter sprites head node (in/out)</param>
        /// <param name="screen">SDL_Surface to display the image</param>
        /// <returns>Nothing</returns>
        internal static void AddClock(ref Sprite? letters, IntPtr screen)
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
                logger.LogError("AddClock: Error navigating to the end of the letters list, no letters yet");
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

                thisLetter.sprite[i].sprite_band_texture = numberBank;
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
            clockSprite = thisLetter;

        }


        /// <summary>Renders the time graphically on the SDL surface</summary>
        /// <param name="screen">the SDL_Surface on which to display the image</param>
        /// <returns>Nothing</returns>
        internal static void UpdateTime(IntPtr screen)
        {
            SDL.SDL_Rect fromRect = new SDL.SDL_Rect
            {
                y = 0, // position on the numberbank band (x CLOCKHEIGHT) - but as only one line, always 0
                w = BoxConstants.CLOCK_WIDTH, // width of the character in the timebox in px
                h = BoxConstants.CLOCK_HEIGHT // height of the character in px
            };

            int thisTime = GameManagerVariables.AVAILABLE_TIME - GameManager.gameTime;
            TimeSpan remainingTime = TimeSpan.FromSeconds(thisTime);

            int minutes = remainingTime.Minutes;
            int minutesTens = minutes / 10;
            int minutesUnits = minutes % 10;
            int seconds = remainingTime.Seconds;
            int secondsTens = seconds / 10;
            int secondsUnits = seconds % 10;

            // clockSprite = new Sprite(5);
            fromRect.x = BoxConstants.CLOCK_WIDTH * minutesTens;
            clockSprite.sprite[0].sprite_band_dimensions = fromRect;
            fromRect.x = BoxConstants.CLOCK_WIDTH * minutesUnits;
            clockSprite.sprite[1].sprite_band_dimensions = fromRect;
            fromRect.x = BoxConstants.CLOCK_WIDTH * secondsTens;
            clockSprite.sprite[3].sprite_band_dimensions = fromRect;
            fromRect.x = BoxConstants.CLOCK_WIDTH * secondsUnits;
            clockSprite.sprite[4].sprite_band_dimensions = fromRect;

            // tick out the last 10 seconds
            if (thisTime <= 10 && thisTime > 0)
            {
                if (thisTime < _lastClockTick)
                {
                    GameManager.TickClock = true;
                    _lastClockTick = thisTime;
                }
                // SoundManager.PlaySound("clock-tick");
            }

        }

    }
}