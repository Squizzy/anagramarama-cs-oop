using SDL2;

namespace AgOop
{
    /// <summary>
    /// Structure to identify a letter graphic in the "band" image containing 
    /// all the letters. 
    /// The band images are:
    /// - letterBank - for the big letters (Shuffle, Answer boxes)
    /// - smallLetterBank - for the small answers boxes
    /// - numberBank - for the numbers (time, score)
    /// </summary>
    internal struct Element
    {
        /// <summary>texture of the spriteband (the "band" image)</summary>
        internal IntPtr sprite_band_texture;

        /// <summary>SDL_rect dimentions of the spriteband (the "band" image)</summary>
        internal SDL.SDL_Rect sprite_band_dimensions;

        /// <summary>Sprite graphic offset from the top left position of the band image</summary>
        internal int sprite_x_offset, sprite_y_offset;
    }


    /// <summary>
    /// class Sprite is used to position the letters in the SHUFFLE and ANSWER boxes.
    ///     spr:        the graphical representation
    ///     numSpr:     ** ?? **
    ///     letter:     the actual letter value
    ///     x, y:       the offset position to use for in the band image
    ///     h, w:       height and width of the graphical representation of the character in the band image
    ///     toX, toY:   the position in the SHUFFLE or ANSWER box where the letter should be placed
    ///     next:       ** ?? **
    ///     box:        the box in which this letter should be placed
    ///     index:      the index of this letter (?)
    /// </summary>
    internal class Sprite
    {
        /// <summary>The spriteband (or spritebands) to be used (ie the character)
        /// contains the sprite, and the offset to the character in the sprite
        /// </summary>
        internal Element[] sprite;

        /// <summary>The number of spr elements in this Sprite</summary>
        internal int numSpr;

        /// <summary>The actual letter summary </summary>
        internal char letter;

        /// <summary>The offset position from top left to use for in the band image and width and height </summary>
        internal int x, y, w, h;

        /// <summary>The position in the SHUFFLE or ANSWER box where the letter should be placed </summary>
        internal int toX, toY;

        /// <summary>Pointer to the next sprite, when needed</summary>
        internal Sprite? next;

        /// <summary>the index (position) of this letter in the SHUFFLE box </summary>
        internal int index;

        /// <summary>The target box (SHUFFLE or ANSWER) in which this letter should be placed </summary>
        internal int box;

        /// <summary> constructor for Sprite </summary>
        /// <param name="numOfElements">The number of sprite elements</param>
        internal Sprite(int numOfElements)
        {
            sprite = new Element[numOfElements];
            for (int i = 0; i < numOfElements; i++)
            {
                sprite[i].sprite_band_texture = IntPtr.Zero; // or some valid texture
                sprite[i].sprite_band_dimensions = new SDL.SDL_Rect();
                sprite[i].sprite_x_offset = 0;
                sprite[i].sprite_y_offset = 0;
            }
            numSpr = 0;
            letter = '\0';
            x = y = w = h = 0;
            toX = toY = 0;
            next = null;
            index = 0;
            box = 0;
        }
    }
}