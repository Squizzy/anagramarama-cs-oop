from PIL import Image, ImageDraw, ImageFont
import math
from pathlib import Path

# Dimensions of current letter banks (large letters):
# letter box: 80x90 pixels
# bank size: 2160x90 pixels
# characters spacing: 2 px
# character top: 6px
# character bottom: 84px 
# => character height = 84 - 6 = 78 px
# Characters total width use: 80x27 + 2x26 = 2160 px (A-Z + space)

debug = False

# BG_TRANSPARENT = "transparent"  # RGBA transparent background
BG_TRANSPARENT = (0, 0, 0, 0)  # RGBA transparent background
BG_WHITE = "white"

# These are the key variables

# This seems to be the originally used font (Blue Highway - althought more probably Blue Highway Deluxe)
# https://www.dafont.com/blue-highway.font
# licensed under GPL/OFL
font_name = "./Blue_Highway_Rg.otf"

# Some more open source fonts that this script was tested with:
# https://www.nerdfonts.com/font-downloads
# font_name = "./RobotoMonoNerdFont-Regular.ttf"
# font_name = "./GeistMonoNerdFontPropo-SemiBold.otf"
# font_name = "./CaskaydiaCoveNerdFontPropo-Bold.ttf"

letter_colour = "black"
image_background_colour: str | tuple[int, int, int , int]
if debug:
    image_background_colour = BG_WHITE
else:
    image_background_colour = BG_TRANSPARENT

class CharacterBank:
    """ Class describing the details of a 3 picture boxes in the original anagramara
    """
    # characters for this set
    characters_set: str
    
    # characters box dimension in the letterbank (letter sprite size) is hardwired in the game:
    box_width: int
    box_height: int
    box_spacing: int
    
    # distances from the top of the image file
    # these are the values observed in the original anagramarama letterbanks
    image_top_to_glyph_top: int
    image_top_to_glyph_bottom: int
    # The difference of the two above
    glyph_height: int
    glyph_shift_from_bottom:int
    
    # Bank image file dimensions
    bank_width: int
    bank_height: int

    def __init__(self, characters_set: str, box_width: int, box_height: int, box_spacing: int, image_top_to_glyph_top: int, image_top_to_glyph_bottom: int, bank_width: int, bank_height: int):
        self.characters_set = characters_set
        self.box_width = box_width
        self.box_height = box_height
        self.box_spacing = box_spacing
        self.image_top_to_glyph_top = image_top_to_glyph_top
        self.image_top_to_glyph_bottom = image_top_to_glyph_bottom
        self.glyph_height = self.image_top_to_glyph_bottom - self.image_top_to_glyph_top
        self.glyph_shift_from_bottom = self.box_height - self.image_top_to_glyph_bottom
        self.bank_width = bank_width
        self.bank_height = bank_height
        
# The information read from the original character banks
#region Large letters
# All lower case letters + a space
LARGE_LETTERS_SET = "abcdefghijklmnopqrstuvwxyz " 
# Characters box dimension in the letterbank (letter sprite size) is hardwired in the game:
LARGE_LETTER_BOX_WIDTH: int = 80  # in px
LARGE_LETTER_BOX_HEIGHT = 90  # in px
# spacing between characters in the letterbank, also hardwired
LARGE_LETTER_BOX_SPACING = 0  # in px

# distance from top of original letterbank bmp, to top of the topmost of all glyphs
LARGE_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP = 6  # in px
# glyph from bottom of character box:
LARGE_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM = 84  # px from top of the character box to the bottom of the glyph
# large_glyph_height = LARGE_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM - LARGE_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP  # height of the glyph in the character box

# Large letterbank dimensions in the game
LARGE_LETTERBANK_WIDTH = 2160
LARGE_LETTERBANK_HEIGHT = LARGE_LETTER_BOX_HEIGHT
#endregion

large_letters_bank: CharacterBank = CharacterBank(
    LARGE_LETTERS_SET, 
    LARGE_LETTER_BOX_WIDTH, 
    LARGE_LETTER_BOX_HEIGHT, 
    LARGE_LETTER_BOX_SPACING, 
    LARGE_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP, 
    LARGE_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM,
    LARGE_LETTERBANK_WIDTH,
    LARGE_LETTERBANK_HEIGHT)


#region Small letters
# All lower case letters, no space
SMALL_LETTERS_SET = "abcdefghijklmnopqrstuvwxyz" 
# Characters box dimension in the letterbank (letter sprite size) is hardwired in the game:
SMALL_LETTER_BOX_WIDTH: int = 10  # in px
SMALL_LETTER_BOX_HEIGHT = 16  # in px
# spacing between characters in the letterbank, also hardwired
SMALL_LETTER_BOX_SPACING = 0  # in px

# distance from top of original letterbank bmp, to top of the topmost of all glyphs
SMALL_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP = 2  # in px
# glyph from bottom of character box:
SMALL_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM = 14  # px from top of the character box to the bottom of the glyph
# small_glyph_height = SMALL_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM - SMALL_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP  # height of the glyph in the character box

# small letterbank dimensions in the game
SMALL_LETTERBANK_WIDTH = 260
SMALL_LETTERBANK_HEIGHT = SMALL_LETTER_BOX_HEIGHT
#endregion

small_letters_bank: CharacterBank = CharacterBank(
    SMALL_LETTERS_SET,
    SMALL_LETTER_BOX_WIDTH,
    SMALL_LETTER_BOX_HEIGHT,
    SMALL_LETTER_BOX_SPACING,
    SMALL_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP,
    SMALL_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM,
    SMALL_LETTERBANK_WIDTH,
    SMALL_LETTERBANK_HEIGHT
    )


#region Numbers
# All numbers, a column and a space
NUMBERS_SET = "0123456789: "
# Characters box dimension in the numbersbank (number sprite size) is hardwired in the game:
NUMBER_BOX_WIDTH: int = 18  # in px
NUMBER_BOX_HEIGHT = 32  # in px
# spacing between characters in the numbersbank, also hardwired
NUMBER_BOX_SPACING = 0  # in px

# distance from top of original numbersbank bmp, to top of the topmost of all glyphs
NUMBERBANK_IMAGE_TOP_TO_GLYPH_TOP = 5  # in px
# glyph from bottom of character box:
NUMBERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM = 29  # px from top of the character box to the bottom of the glyph
# number_glyph_height = NUMBERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM - NUMBERBANK_IMAGE_TOP_TO_GLYPH_TOP  # height of the glyph in the character box

# numbersbank dimensions in the game
NUMBERBANK_WIDTH = 216
NUMBERBANK_HEIGHT = NUMBER_BOX_HEIGHT
#endregion

numbers_bank: CharacterBank = CharacterBank(
    NUMBERS_SET,
    NUMBER_BOX_WIDTH,
    NUMBER_BOX_HEIGHT,
    NUMBER_BOX_SPACING,
    NUMBERBANK_IMAGE_TOP_TO_GLYPH_TOP,
    NUMBERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM,
    NUMBERBANK_WIDTH,
    NUMBERBANK_HEIGHT
)

# And a test region for debug
#region test
# All lower case letters + a space
# TEST_LETTERS_SET = "abcdefghijklmnopqrstuvwxyz " 
# TEST_LETTERS_SET = "bdlpq" 
TEST_LETTERS_SET = "p" 
# Characters box dimension in the letterbank (letter sprite size) is hardwired in the game:
TEST_LETTER_BOX_WIDTH: int = 80  # in px
TEST_LETTER_BOX_HEIGHT = 90  # in px
# spacing between characters in the letterbank, also hardwired
TEST_LETTER_BOX_SPACING = 0  # in px

# distance from top of original letterbank bmp, to top of the topmost of all glyphs
TEST_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP = 6  # in px
# glyph from bottom of character box:
TEST_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM = 84  # px from top of the character box to the bottom of the glyph
# large_glyph_height = TEST_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM - TEST_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP  # height of the glyph in the character box

# Large letterbank dimensions in the game
TEST_LETTERBANK_WIDTH = 2160
TEST_LETTERBANK_HEIGHT = TEST_LETTER_BOX_HEIGHT
#endregion

test_letters_bank: CharacterBank = CharacterBank(
    TEST_LETTERS_SET, 
    TEST_LETTER_BOX_WIDTH, 
    TEST_LETTER_BOX_HEIGHT, 
    TEST_LETTER_BOX_SPACING, 
    TEST_LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP, 
    TEST_LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM,
    TEST_LETTERBANK_WIDTH,
    TEST_LETTERBANK_HEIGHT)



#region old

# Not unused as unsuitable font metrics
def get_desired_font_size(font_name: str, target_height: int) -> int:
    """
    Get the font size for a given font name and target character height.
    This is used to calculate the required font size for a character to fit in the character box.
    """
    # Test size to get the metrics
    test_font_size = 120
    
    # With the test size, get the actual resulting metric
    test_font_height_actual_value = ImageFont.truetype(font_name, test_font_size).getmetrics()[0]
    
    # Calculate the font size needed to achieve the target height
    # if test_font_Size gives real_test_font_height
    # what is x when we want font_height (glyph height really)
    # test_font_size = > font_size
    # test_font_height_actual_value => target_height
    font_size = math.floor((test_font_size * target_height) / test_font_height_actual_value)
    
    test_font_height_actual_value_2 = ImageFont.truetype(font_name, font_size).getmetrics()[0]
    
    if debug:
        print("==== Starting new font processing - font size ====")
        print(f"{font_size} = {test_font_size} * {target_height} / {test_font_height_actual_value}")
        print(f"{target_height=}")
        print(f"{font_size=}")
        print(f"{test_font_height_actual_value_2=}")

    return int(font_size)

# Not used
def draw_font_ascent_and_descent(letterBank_drawing: ImageDraw.ImageDraw, font: ImageFont.FreeTypeFont, current_box_horizontal_position: int, next_box_horizontal_position, from_bank: CharacterBank):
    # Font ascent and descent are different from character ascender_height, descender_height in that
    # the font-level ascent and descent need to support all chars in that font
    font_ascent: int
    font_descent: int
    
    font_ascent, font_descent = font.getmetrics()
    print(f"{font_ascent=} = {font_descent=}")
    
        # in order to draw these below, we need to know where the baseline is positioned on the character box
    draw_font_ascent_and_descent: bool = False
    
    if draw_font_ascent_and_descent:
        
        baseline: int = 0
        
        font_ascent_box =  (
            current_box_horizontal_position + 1, 
            baseline - font_ascent, 
            (next_box_horizontal_position - 2) - (from_bank.box_width-2)/2, 
            baseline)
        
        letterBank_drawing.rectangle(font_ascent_box, fill = "red")
        
        font_descent_box = (
            current_box_horizontal_position + 1 + (from_bank.box_width - 2) / 2, 
            baseline, 
            (next_box_horizontal_position - 2), 
            baseline + font_descent)
        
        letterBank_drawing.rectangle(font_descent_box, fill = "blue")

# def test_new_positioning(font_name:str):
#     test_font = ImageFont.truetype(font_name, 40)  # Load the font with a specific size : 90 for example
#     print(test_font.getmetrics())
#     test_font = ImageFont.truetype(font_name, 90)  # Load the font with a specific size : 90 for example
#     print(test_font.getmetrics())
#     test_font = ImageFont.truetype(font_name, 120)  # Load the font with a specific size : 90 for example
#     print(test_font.getmetrics())
#     test_font = ImageFont.truetype(font_name, 90 * 90 / 87)  # to get 90 px in height: 90 * 90 / 87 = 103.44827586206896
#     print(test_font.getmetrics())

# def test_new_bbox(font_name:str, test_char: str):
#     test_font_size = 90
#     real_test_font_height = ImageFont.truetype(font_name, test_font_size).getmetrics()[0] 
#     font_size = test_font_size * CHARACTER_BOX_HEIGHT / real_test_font_height
#     print(font_size)

#     test_font = ImageFont.truetype(font_name, font_size)  # Load the font with a specific size : 90 for example

#     # Initialise, then create an image of the character box size
#     test_image = Image.new("RGBA", (CHARACTER_BOX_WIDTH, CHARACTER_BOX_HEIGHT), background_white)
#     test_drawing = ImageDraw.Draw(test_image)
    
#     # set the top left anchor to 0,0 for now, and define a char to use (e.g. "g")
#     test_top_left = (0,0)

    
#     # test the position of the bounding box when applying the test char to the test character image
#     left, top, right, bottom = test_drawing.textbbox(test_top_left, test_char, font=test_font)
#     print(f"Bounding box for character '{test_char}': {left=}, {top=}, {right=}, {bottom=}")
#     print(f"width: {right - left}, height: {bottom - top}")
#     print(f"{test_font.getlength(test_char)=}")
    
#         # Draw the character at the specified position
#     test_drawing.text(test_top_left, test_char, font=test_font, fill="black")
#     return test_image



    

# # Calculate the font and the top-left position for the character required to fit the glyph position
# def calculate_font_metrics(font_name: str, test_char: str) -> tuple[int, tuple[int, int]]:
#     # Load the font with a test size
#     test_font_size = 90
#     test_font = ImageFont.truetype(font_name, test_font_size)  # Load the font with a specific size : 90 for example

#     # Initialise, then create an image of the character box size
#     test_image = Image.new("RGBA", (CHARACTER_BOX_WIDTH, CHARACTER_BOX_HEIGHT), background_white)
#     test_drawing = ImageDraw.Draw(test_image)
    
#     # set the top left anchor to 0,0 for now, and define a char to use (e.g. "g")
#     test_top_left = (0,0)

    
#     # test the position of the bounding box when applying the test char to the test character image
#     left, top, right, bottom = test_drawing.textbbox(test_top_left, test_char, font=test_font)
#     print(test_font.getmetrics())
    
#     # print(textbox,  "size:", (textbox[2] - textbox[0], textbox[3] - textbox[1]))
#     # print(f"Bounding box for character '{test_char}': {left=}, {top=}, {right=}, {bottom=}")
#     # print(f"width: {right - left}, height: {bottom - top}")
#     # Calculate the anchor position, and the font size required 
#     # to fit the character in the box as expected
#     required_font_size = int(test_font_size * glyph_height / (bottom - top))
#     required_top_left =  int((CHARACTER_BOX_WIDTH - (right - left)) / 2 ), int(LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP - top)
    
#     return required_font_size, required_top_left


# # Calculate the font and the top-left position for the character required to fit the glyph position
# def calculate_font_metrics_for_string(font_name: str, test_char_string: str) -> tuple[int, tuple[int, int]]:
#     # Load the font with a test size
#     test_font_size = 90
#     test_font = ImageFont.truetype(font_name, test_font_size)  # Load the font with a specific size : 90 for example

#     string_box_width = CHARACTER_BOX_WIDTH * len(test_char_string) + CHARACTER_BOX_SPACING * (len(test_char_string) - 1)

#     # Initialise, then create an image of the character box size
#     test_image = Image.new("RGBA", (string_box_width, CHARACTER_BOX_HEIGHT), background_white)
#     test_drawing = ImageDraw.Draw(test_image)
    
#     # set the top left anchor to 0,0 for now, and define a char to use (e.g. "g")
#     test_top_left = (0,0)

    
#     # test the position of the bounding box when applying the test char to the test character image
#     left, top, right, bottom = test_drawing.textbbox(test_top_left, test_char_string, font=test_font)
    
#     # print(textbox,  "size:", (textbox[2] - textbox[0], textbox[3] - textbox[1]))
#     # print(f"Bounding box for character '{test_char}': {left=}, {top=}, {right=}, {bottom=}")
#     # print(f"width: {right - left}, height: {bottom - top}")
#     # Calculate the anchor position, and the font size required 
#     # to fit the character in the box as expected
#     required_font_size = int(test_font_size * glyph_height / (bottom - top))
#     required_top_left =  int((string_box_width - (right - left)) / 2 ), int(LETTERBANK_IMAGE_TOP_TO_GLYPH_TOP - top)
    
#     return required_font_size, required_top_left


# def draw_a_character(character: str, font_name: str, font_size: int, top_left: tuple[int, int]) :
    
#     # Load the font with the specified size
#     font = ImageFont.truetype(font_name, font_size)
    
#     # Initialise and Create a new image for the character
#     character_image = Image.new("RGBA", (CHARACTER_BOX_WIDTH, CHARACTER_BOX_HEIGHT), background_white)
#     character_drawing = ImageDraw.Draw(character_image)
    
#     # Draw the character at the specified position
#     character_drawing.text(top_left, character, font=font, fill="black")
    
#     return character_image

# def draw_a_string(string: str, font_name: str, font_size: int, top_left: tuple[int, int]) :
#     # Load the font with the specified size
#     font = ImageFont.truetype(font_name, font_size)
    
#     # Calculate the width of the string
#     string_box_width = CHARACTER_BOX_WIDTH * len(string) + CHARACTER_BOX_SPACING * (len(string) - 1)

#     # Initialise and Create a new image for the string
#     string_image = Image.new("RGBA", (string_box_width, CHARACTER_BOX_HEIGHT), background_white)
#     string_drawing = ImageDraw.Draw(string_image)
    
#     # Draw the string at the specified position
#     string_drawing.text(top_left, string, font=font, fill="black")
    
#     return string_image


# def get_target_font_height(font_name: str, char_set: str) -> tuple[int, tuple[int, int]]:
#     """
#     Get the font height to select for all the chars needed in the font in the font.
#     This is used to calculate the required font size for a character to fit in the character box.
#     """
#     target_font_size = 0
#     max_font_size = 0
#     min_font_size = 0
#     for char in char_set:
#         # Load the font with a test size
#         char_font_size, top_left = calculate_font_metrics(font_name, char)
#         print(f"{char_font_size} - {top_left}")
#         target_font_size += char_font_size
#         if char_font_size > max_font_size:
#             max_font_size = char_font_size
#         if min_font_size == 0 or char_font_size < min_font_size:
#             min_font_size = char_font_size
     
#     print(f"{target_font_size=}, {max_font_size=}, {min_font_size=}")   
#     return min_font_size, top_left

   #region oldgt
    # # # do another test to check the changes
    # # test_font_size2: int = 150
    # # test_max_ascender_height2, test_max_descender_height2 = get_ascender_and_descender_heights(font_name, test_font_size2, from_bank, anchor, test)

    # # # calculate the max size needed for the glyphs at that test_font_size for the characters used
    # # max_test_glyph_size2:float = test_max_ascender_height2 - test_max_descender_height2
    # # print(f"{test_font_size2=}: {max_test_glyph_size2=}")

    # # # create the adjuster for the font and for the ascender and descender sizes:
    # # adjuster2 = test_font_size / max_test_glyph_size
    # # print(f"{adjuster2=}")
    
    # # font_size2 = from_bank.glyph_height * adjuster2
    
    # # print(f".  {from_bank.glyph_height} - {font_size2=}")

    # #region gdfs
    # # # With the test size, get the actual resulting metric
    # # char_font:ImageFont.FreeTypeFont = ImageFont.truetype(font_name, test_font_size)

    # # test_img:Image.Image = Image.new("RGBA",
    # #                         (test_font_size, test_font_size),
    # #                         image_background_colour)
    # # test_draw:ImageDraw.ImageDraw = ImageDraw.Draw(test_img)
    
    # # test_baseline:int = test_font_size * 3 // 4 # some value, cause something is needed
    # # # anchor:str = "md" #  Set anchor to the baseline, character to the horizontal centered (don't really care)
    
    # # max_ascender_height:int  = 0
    # # max_descender_height:int = 0
    
    # # for char in from_bank.characters_set:
        
    # #     # Get the bounding_box for this letter at the desired placement
    # #     bounding_box: tuple[float, float, float, float] = test_draw.textbbox((test_img.width/2, test_baseline), char, font=char_font, anchor=anchor)
    # #     # print(f"{bounding_box}")
        
    # #     # Calculate the ascending and descending heights
    # #     ascender_height:int = test_baseline - int(bounding_box[1])
    # #     descender_height:int = test_baseline - int(bounding_box[3])
    # #     print(f"{char=}: {bounding_box}: {ascender_height=} - {descender_height=}")
        
        
    # #     if test:
    # #         # Print the ascending and descending heights
    # #         test_draw.rectangle((0,test_baseline - ascender_height,test_img.width/2,test_baseline), fill="red")
    # #         test_draw.rectangle((0,test_baseline,test_img.width/2,test_baseline - descender_height), fill="green")
            
    # #         # draw the baseline on the image
    # #         test_draw.line((0, test_baseline, test_img.width, test_baseline), fill="yellow")
            
    # #         # draw the letter on the image
    # #         test_draw.text((test_img.width/2, test_baseline), char, font=char_font, anchor=anchor, fill="black")
            
    # #         # draw the bounding box for this letter
    # #         test_draw.rectangle(bounding_box, outline="green")
        
    # #     # ascender_heights should always be positive
    # #     if ascender_height > max_ascender_height:
    # #         max_ascender_height = ascender_height
            
    # #     # descender_heights should always be negative
    # #     if descender_height < max_descender_height:
    # #         max_descender_height = descender_height
    # #endregion
    
    
    # # create the adjuster for the font and for the ascender and descender sizes:
    # adjuster2 = test_font_size / max_test_glyph_size
    # # print(f"{adjuster=}")
    
    # # calculate the desired_font_size to fit the expectations
    # # if test_font_size = 120 means max_glyph_size = max_test_glyph_size
    # # then 
    # # desired_font_size = from_box.glyph_height means * test_font_size / max_glyph_size
    # # if 
    # #   test_font_size(known) means max_test_glyph_size(known)
    # # and
    # #   desired_font_size(unknown) means desired_glyph_size(known)
    # # then
    # #   desired_font_size = desired_glyph_size * test_font_size / max_test_glyph_size
    
    # # desired_font_size:int = from_Bank.glyph_height * test_font_size // max_glyph_size
    # desired_font_size:int = int(from_bank.glyph_height * adjuster2)
    # # print(f"{from_bank.glyph_height=}, {desired_font_size=}")
    
    
    
    
    # # do a second pass
    
    # test_max_ascender_height, test_max_descender_height = get_ascender_and_descender_heights(font_name, desired_font_size, from_bank, anchor, test)

    #     # calculate the max size needed for the glyphs at that test_font_size for the characters used
    # max_test_glyph_size = test_max_ascender_height - test_max_descender_height
    # # print(f"{test_font_size=}: {max_test_glyph_size=}")
    
    # # create the adjuster for the font and for the ascender and descender sizes:
    # adjuster = test_font_size / max_test_glyph_size
    # # print(f"2{adjuster=}")
    
    # desired_font_size = int(from_bank.glyph_height * adjuster)
    # # print(f"{from_bank.glyph_height=}, {desired_font_size=}")
    
    # # with the new font size, recalculate the max ascender and descender
    
    # # #adjust the max ascender and descender values that will result from the changed font
    # # max_ascender_height = int(max_ascender_height / adjuster)
    # # max_descender_height = int(max_descender_height / adjuster)

    # # char_font = ImageFont.truetype(font_name, desired_font_size)

    # # max_ascender_height  = 0
    # # max_descender_height = 0
    
    # # for char in from_bank.characters_set:
        
    # #     # Get the bounding_box for this letter at the desired placement
    # #     bounding_box = test_draw.textbbox((test_img.width/2, test_baseline), char, font=char_font, anchor=anchor)
    # #     # print(f"{bounding_box}")
        
    # #     # Calculate the ascending and descending heights
    # #     ascender_height = test_baseline - int(bounding_box[1])
    # #     descender_height = test_baseline - int(bounding_box[3])
    # #     print(f"{char=}: {bounding_box}: {ascender_height=} - {descender_height=}")
        
        
    # #     if test:
    # #         # Print the ascending and descending heights
    # #         test_draw.rectangle((0,test_baseline - ascender_height,test_img.width/2,test_baseline), fill="red")
    # #         test_draw.rectangle((0,test_baseline,test_img.width/2,test_baseline - descender_height), fill="green")
            
    # #         # draw the baseline on the image
    # #         test_draw.line((0, test_baseline, test_img.width, test_baseline), fill="yellow")
            
    # #         # draw the letter on the image
    # #         test_draw.text((test_img.width/2, test_baseline), char, font=char_font, anchor=anchor, fill="black")
            
    # #         # draw the bounding box for this letter
    # #         test_draw.rectangle(bounding_box, outline="green")
        
    # #     # ascender_heights should always be positive
    # #     if ascender_height > max_ascender_height:
    # #         max_ascender_height = ascender_height
            
    # #     # descender_heights should always be negative
    # #     if descender_height < max_descender_height:
    # #         max_descender_height = descender_height
    
    
    # # Recalculate the max_max_ascender_height and max_descender_height for the chars we want, now we know the font size
    # # test_max_ascender_height, test_max_descender_height = get_ascender_and_descender_heights(font_name, test_font_size, from_bank, anchor, test)
    # # print(f"{max_ascender_height=}, {max_descender_height=}")
    
    # # if test:
    # #     return test_img
    # # else:
    # return desired_font_size
    # # , max_ascender_height, max_descender_height
    #endregion


    #region font_metrics
    # Get the value of the ascent and descent of this font
    # Ascent: the distance from the baseline to the top of the highest glyph
    # Descent: the distance from the baseline to the bottom of the lowest glyph
    # Baseline is the line on which most letters sit 
    #   (eg the bottom of the lowercase 'a' or bottom of the circle of lower 'q')
    # The letters align to the baseline, 
    #   ascent will absorb the up vertical bar of the lower case 'd'
    #   descent will absorb the down vertical bar of the lower case 'p'    
    # actually since a tiny subset of the font is used, the font metrics are not useful...
    # font_ascent, font_descent = char_font.getmetrics()
    # if debug:
    #     print(f"{font_ascent=}, {font_descent=}")
    #endregion



    #region old1
    # character_box1_position = (
    #         0,                                         
    #         0, 
    #         CHARACTER_BOX_WIDTH - 1,   
    #         CHARACTER_BOX_HEIGHT - 1)
    
    # # Draw the first box
    # letterBank_drawing.rectangle(character_box1_position , outline="magenta")
    
    # # Set the position of the horizontal start of the next box in case there are more
    # next_box_horizontal_pos: int = CHARACTER_BOX_WIDTH - 1
    
    # # The first char box has been drawn already, we do this if we need more
    # for _ in (range (num_of_chars - 1)):
        
    #     # define the position of the spacer
    #     box_separator_position = (
    #         next_box_horizontal_pos, 
    #         0, 
    #         next_box_horizontal_pos + CHARACTER_BOX_SPACING - 1, 
    #         CHARACTER_BOX_HEIGHT - 1)
         
    #     # Draw the spacer   
    #     letterBank_drawing.rectangle(box_separator_position, outline="cyan")
        
    #     # Shift the horizontal starting point for the next box by a spacer
    #     next_box_horizontal_pos += CHARACTER_BOX_SPACING
    
    #     # define the position of the next character
    #     character_box_next_position = (
    #             next_box_horizontal_pos, 
    #             0, 
    #             next_box_horizontal_pos + CHARACTER_BOX_WIDTH - 1,   
    #             CHARACTER_BOX_HEIGHT - 1)
        
    #     # Draw the next character
    #     letterBank_drawing.rectangle(character_box_next_position, outline="magenta")
        
    #     # Shift the horizontal starting point for the next box by a character box width
    #     next_box_horizontal_pos += CHARACTER_BOX_SPACING
        
    # if num_of_chars == 2:
    #     box2_position = (
    #             CHARACTER_BOX_WIDTH, 
    #             0, 
    #             CHARACTER_BOX_WIDTH + CHARACTER_BOX_SPACING - 1, 
    #             CHARACTER_BOX_HEIGHT - 1)
    #     test_drawing.rectangle(box2_position, outline="cyan")
        
    #     box3_position = (
    #             CHARACTER_BOX_WIDTH + CHARACTER_BOX_SPACING, 
    #             0, 
    #             CHARACTER_BOX_WIDTH + CHARACTER_BOX_SPACING + CHARACTER_BOX_WIDTH - 1,   
    #             CHARACTER_BOX_HEIGHT - 1)
    #     test_drawing.rectangle(box3_position, outline="magenta")
            # NOTE: scaling to generate the tiny resolutions expected by the letterbank images
            # does not work so great, so descent here is halved as a kludge to display okay still
            # anchor_position_y = from_bank.image_top_to_glyph_bottom - descent // 2
            # print(f"{i=}: {chars_set[i]=}")
            # print(f"{i=}: {from_bank.characters_set[i]=}")
            # draw_bounding_box(letterBank_drawing, (anchor_position_x, anchor_position_y), chars_set[0], char_font, anchor )
            # draw_bounding_box(letterBank_drawing, (anchor_position_x, anchor_position_y), from_bank.characters_set[i], char_font, anchor)
        # bounding_box1 = letterBank_drawing.textbbox(position_of_anchor_on_pic, chars_set[0], font=char_font, anchor=anchor)
        # letterBank_drawing.rectangle(bounding_box1, outline="green")
    #region old2
    # if debug:
    #     draw_bounding_box(letterBank_drawing, (anchor_position_x, anchor_position_y), chars_set[i], char_font, anchor )

    # # Draw the next character at the specified position
    # letterBank_drawing.text(
    #     (anchor_position_x, anchor_position_y), 
    #     chars_set[i], 
    #     font=char_font, 
    #     fill="black", 
    #     anchor=anchor)


    # if num_of_chars == 2:
    #     # Adjust the horizontal position of the anchor for the second character
    #     #   Basically, it is offset by one character box width + the spacing between the boxes
    #     position_of_anchor_on_pic = (CHARACTER_BOX_WIDTH + CHARACTER_BOX_SPACING + CHARACTER_BOX_WIDTH // 2, LETTERBANK_IMAGE_TOP_TOP_GLYPH_BOTTOM - descent)
        
    #     # For debug, draw the bounding box (the space the glyph takes) for the second character
    #     bounding_box2 = test_drawing.textbbox(position_of_anchor_on_pic, char[1], font=char_font, anchor=anchor)
    #     test_drawing.rectangle(bounding_box2, outline="red")
        
    #     # Draw the second character at the specified position
    #     test_drawing.text(position_of_anchor_on_pic, char[1], font=char_font, fill="black", anchor=anchor)
    #endregion
    
    #endregion

    #region oldmain1
    # test_character = "ai"  # Character to draw for testing
    # test_characters = "abcdefghijklmnopqrstuvwxyz "  # Characters to test

    # if False:  # Set to True to test the new positioning
    #     required_font_size, required_top_left = calculate_font_metrics(font_name, test_character)
    #     print(f"{required_font_size=}, {required_top_left=}")
    #     # earlier test got:
    #     # test_font = ImageFont.truetype("./Blue_Highway_Rg.otf", size = 97) # makes font dimensions 52x78
    #     # font test size set to 40, target size is 78, test_height was 32 -> (40 *78)/32 = 97
    #     #   font_size = 97  # This size makes the character dimensions fit within the box
    #     # width = 80 bounding box = 52 space remaining = (80 - 52) / 2 = 14
    #     # height starts at 6 - 39 = - 33
    #     #   top_left = (14,-33)  # x, y coordinates for the top-left corner for a char to be positioned correctly

    #     # Get the required font size and top-left position for the test character
    #     required_font_size, required_top_left = get_target_font_height(font_name, test_characters)
    #     print(f"{required_font_size=}, {required_top_left=}")
    
    # if False: # Set to True to test the character drawing
    #     for char in test_characters:
    #         character_image = draw_a_character(char, font_name, required_font_size, required_top_left)

    #     # Save the character image to a file
        
    #         character_image.save(f"character_result_{char}.png")
    
    # if False:
    #     test_new_positioning(font_name)
    
    # if False:
    #     test_new_bbox(font_name, test_character).save("test_character_image.png")
    #endregion
    
        
    #region oldmain2    
    # character_image = Image.new("RGBA", (character_box_width, character_box_height), background_white)

    # # draw_a_character = ImageDraw.Draw(character_image)
    # # character_image.save("character_result.png")
    #     if False:
    #         required_font_size, required_top_left = calculate_font_metrics_for_string(font_name, test_characters)
    #         image = draw_a_string(test_characters, font_name, required_font_size, required_top_left)

    #         image.save("string_result.png")
    # # letterbank_num_of_chars = 27
    # # bank_width = character_box_width * letterbank_num_of_chars + character_box_spacing * (letterbank_num_of_chars - 1)



    # text = "g"
    # draw.text(top_left, text, font=font, fill="black")
    # # draw.textbbox((10, 6, 70, 84), "g", font=font, fill="black")
    #endregion


    # if False:
    #     # debug testing the calcuation of ascender and descender heights
    #     font_size:int = 200
    #     get_max_ascender_and_descender_heights(font_name, font_size, test_letters_bank, "ms", True).save("string_max_ascender_and_descender_heights.png")

#endregion



def get_max_ascender_and_descender_heights(font_name: str, font_size:int, from_Bank: CharacterBank, anchor: str, test:bool = False) -> tuple[float, float]:
    """ Calculates the max_ascender_height and max_descender_height for a given set of characters of a font
    The character banks need a specific height of characters per bank.
    As only lower case are used, the font ascent and descent can't be used as characters outside the need 
        might have higher ascender or descenders which messes up the output.
    
    Args:
        font_name (str): The font to be used
        font_size (int): The font height for which the ascender and descender are to be calculated
        from_Bank (CharacterBank): The bank including the characters to be used for the calculation
        anchor (str): The anchor position on the character for calculation purposes
        test (bool, optional): used for debuging the function only. Defaults to False.

    Returns:
        tuple[float, float]: max_ascender_height max_descender_height
    """
    # With the test size, get the actual resulting metric
    char_font:ImageFont.FreeTypeFont = ImageFont.truetype(font_name, font_size)

    test_img:Image.Image = Image.new("RGBA",
                            (font_size, font_size),
                            image_background_colour)
    test_draw:ImageDraw.ImageDraw = ImageDraw.Draw(test_img)
    
    test_baseline:int = font_size * 3 // 4 # some value, cause something is needed
    
    # initialise the max values
    max_ascender_height:float = 0
    max_descender_height:float = 0
    
    for char in from_Bank.characters_set:
        
        # Get the bounding_box for this letter at the desired placement
        bounding_box: tuple[float, float, float, float] = test_draw.textbbox((test_img.width/2, test_baseline), char, font=char_font, anchor=anchor)
        
        # Calculate the ascending and descending heights for this character
        ascender_height:float = test_baseline - bounding_box[1]
        descender_height:float = test_baseline - bounding_box[3]
        # print(f"{char=}: {bounding_box}: {ascender_height=} - {descender_height=}")
        
        
        if test:
            # Print the ascending and descending heights
            test_draw.rectangle((0,test_baseline - ascender_height,test_img.width/2,test_baseline), fill="red")
            test_draw.rectangle((0,test_baseline,test_img.width/2,test_baseline - descender_height), fill="green")
            
            # draw the baseline on the image
            test_draw.line((0, test_baseline, test_img.width, test_baseline), fill="yellow")
            
            # draw the letter on the image
            test_draw.text((test_img.width/2, test_baseline), char, font=char_font, anchor=anchor, fill="black")
            
            # draw the bounding box for this letter
            test_draw.rectangle(bounding_box, outline="blue")
        
        # ascender_heights should always be positive
        if ascender_height > max_ascender_height:
            max_ascender_height = ascender_height
            
        # descender_heights should always be negative
        if descender_height < max_descender_height:
            max_descender_height = descender_height

    # print(f"{font_size=}, {max_ascender_height=}, {max_descender_height=}")
    
    # if test:
    #     return test_img
    # else:
    return max_ascender_height, max_descender_height

def get_desired_font_size_for_string(font_name: str, from_bank: CharacterBank, anchor: str, test:bool = False) -> int:
    """
    Get the maximum ascender and descender heights of a set of characters of a specific font.
    This is used to calculate the required font size for a character to fit in the character box.
    """
    test_max_ascender_height: float
    test_max_descender_height: float
    
    # Test size to get the metrics
    test_font_size: int = 200

    # Calculate the ascender and descender heights for this font size
    test_max_ascender_height, test_max_descender_height = get_max_ascender_and_descender_heights(font_name, test_font_size, from_bank, anchor)

    # calculate the max size needed for the glyphs at that test_font_size for the characters used
    max_test_glyph_size:float = test_max_ascender_height - test_max_descender_height
    # print(f"{test_font_size=}: {max_test_glyph_size=}")
    
    # create the adjuster for the font and for the ascender and descender sizes:
    adjuster: float = test_font_size / max_test_glyph_size
    # print(f"{adjuster=}")
    
    font_size: float = from_bank.glyph_height * adjuster
    
    # print(f"{from_bank.glyph_height} - {font_size1=}")
    return int(font_size)

# for debug
def draw_ascender_and_descender_height(letterBank_drawing: ImageDraw.ImageDraw, position_of_anchor_on_pic: tuple[int, int], max_ascender_height: int, max_descender_height: int, width: int) -> None:
    
    # print(f"draw: {position_of_anchor_on_pic=}, {max_ascender_height=}, {max_descender_height=}, {width=}")
    
    if debug:
        # draw ascender_height
        letterBank_drawing.rectangle((
            position_of_anchor_on_pic[0] - width // 2,
            position_of_anchor_on_pic[1] - max_ascender_height,
            position_of_anchor_on_pic[0],
            position_of_anchor_on_pic[1]),
            fill="red")
        
        # draw decender_height
        letterBank_drawing.rectangle((
            position_of_anchor_on_pic[0],
            position_of_anchor_on_pic[1],
            position_of_anchor_on_pic[0] + width // 2,
            position_of_anchor_on_pic[1] - max_descender_height),
            fill="green")

# for debug
def draw_character_box_position(letterBank_drawing: ImageDraw.ImageDraw, num_of_chars: int, font:ImageFont.FreeTypeFont, from_bank: CharacterBank) -> None:
    """ For testing visualisation, draw the characters space reserved on the image
    (this is not the bounding box)
    """

    # x-position of the about to be drawn box
    current_box_horizontal_position:int = 0
    
    # x-position of the next box after this one
    # basically width of the x-position of the box about to be drawn + 1    
    next_box_horizontal_position:int = from_bank.box_width
    
    # For each character in the desired string (bank)
    for _ in range (num_of_chars):
        
        # Position the character box
        character_box_position: tuple[int, int, int, int] = (
            current_box_horizontal_position,                                         
            0, 
            next_box_horizontal_position - 1,   
            from_bank.box_height - 1)
        
        # Draw the character box
        letterBank_drawing.rectangle(character_box_position , outline="magenta")

        # If there is more than one character, then there might be a spacer
        if num_of_chars > 0:
            
            # Shift the box start corner (x direction) of the next box
            # by the width of the box just drawn
            current_box_horizontal_position += from_bank.box_width    

            # If there is a box spacer, add it
            if from_bank.box_spacing != 0:
                
                # define the "width" of the spacer
                # this is basicaly the starting position of the next box
                next_box_horizontal_position += from_bank.box_spacing
                
                # position the spacer box
                box_separator_position = (
                    current_box_horizontal_position, 
                    0, 
                    next_box_horizontal_position - 1, 
                    from_bank.box_height - 1)
                
                # Draw the spacer   
                letterBank_drawing.rectangle(box_separator_position, outline="cyan")
                
                # Shift the box start corner (x direction) of the next box
                # by the width of the box just drawn
                current_box_horizontal_position += from_bank.box_spacing    
        
            # define the position of the box for the next character
            next_box_horizontal_position += from_bank.box_width

# for debug
def draw_reduced_bounding_box(letterBank_drawing, position_of_anchor_on_pic: tuple[int, int], char: str, font, anchor: str) -> None:
    """ Draw a bounding box around the letter box
    reduced as the bounding box will cover the first pixel but be outside of the last pixel
    in h and w
    this modified version focussing on ensuring the bounding box can be positioned only 
    where the char pixel are positioned.
    """
    # Read the bounding box coordinates for the character
    bounding_box = letterBank_drawing.textbbox(position_of_anchor_on_pic, char, font=font, anchor=anchor)
    print(f"{char=} - {bounding_box=}")
    
    
    # Space character does not have a bounding box height...
    if char != " ":
        # Bring the bounding_box lines bottom and right to over the character
        bounding_box = (bounding_box[0], bounding_box[1], bounding_box[2] - 1, bounding_box[3] - 1)
    # Draw this bounding box
    letterBank_drawing.rectangle(bounding_box, outline="blue")

def generate_letterBank_image(font: str, from_bank: CharacterBank) -> Image.Image:
    """Generates the image for a letterbank image based on the provided font and character bank.

    Args:
        font (str): The font to use
        from_bank (CharacterBank): The characterBank to generate for

    Returns:
        Image.Image: The image generated
    """
    
    # The font size used to generate the ImageFont item for the desired characterBank
    font_size:int
    
    # The max ascender and descender for the characters used (not the whole font)
    # used to position the glyph on the image
    max_ascender_height:float
    max_descender_height:float
    
    # define the position of the anchor on the glyph.
    # The anchor is the spot on the character that we attach the glyph on the image with
    # horizontally:
    #       letterbank contains evenly spaced sprites, so aligning centre (m) is logical 
    #       as glyphs won't be the same width themselves (truetype issue...)
    # vertically:
    #       glyphs should all align horizontally along the baseline
    #       The position of the baseline will be calculated: from the bottom of theimage, up the number of pixels identified,
    #        then up the max_descent_height
    # defined early as it will be used to calculate the font size that needs to be used, and for debug graphics purpose
    anchor:str = "ms"

    # Updated calculation of font size
    # not relying on font metrics but on the metrics of the characters to be used
    font_size  = get_desired_font_size_for_string(font, from_bank, anchor)
    
    # Load the font with a needed size
    char_font:ImageFont.FreeTypeFont = ImageFont.truetype(font_name, font_size)  
    
    # get the max ascender and descenders resulting for the charaterst to be used
    max_ascender_height, max_descender_height = get_max_ascender_and_descender_heights(font_name, font_size, from_bank, anchor)
    

    # Get the umber of characters in the string
    num_of_chars:int = len(from_bank.characters_set)  

    # Initialise, then create an image of the correct size for the number of characters
    image_width:int = from_bank.box_width + (num_of_chars - 1) * (from_bank.box_spacing + from_bank.box_width) 
    image_height:int  = from_bank.box_height
    background_colour: str | tuple[int, int, int, int] = image_background_colour
    
    letterBank_image:Image.Image = Image.new("RGBA", 
                            (image_width, image_height), 
                            background_colour)

    # Create a drawing context for the image
    letterBank_drawing:ImageDraw.ImageDraw = ImageDraw.Draw(letterBank_image)
        
    # # For testing visualisation, draw the characters bounding boxes
    if debug:
        draw_character_box_position(letterBank_drawing, num_of_chars, char_font, from_bank)

    # Define the position of the glyph anchor on the image
    # On the image, the anchor should be positioned:
    #   horizontally: in the middle of the character box where the glyph will go
    anchor_position_x:int = from_bank.box_width // 2
    
    #   vertically: at the baseline level
    #       The baseline is the line on which most letters sit
    #       the position is: 
    #           1) from the bottom 
    #           2) then up by the number of pixels identified
    #           3) then up the characterset (not font) max_descender_height
    anchor_position_y:int = from_bank.box_height - from_bank.glyph_shift_from_bottom + int(max_descender_height)
    
    # process charaters of the string one at a time    
    for i in range(num_of_chars):
    
        # For debug, draw the bounding box (the space the glyph takes) for the first character
        if debug:
            draw_reduced_bounding_box(letterBank_drawing, (anchor_position_x, anchor_position_y), from_bank.characters_set[i], char_font, anchor )
            draw_ascender_and_descender_height(
                        letterBank_drawing, 
                        (anchor_position_x, anchor_position_y), 
                        int(max_ascender_height), 
                        int(max_descender_height), 
                        from_bank.box_width)
    
        # Draw the character character at the specified position
        letterBank_drawing.text(
                (anchor_position_x, anchor_position_y), 
                from_bank.characters_set[i], 
                font=char_font, 
                fill="black", 
                anchor=anchor)
    
        # The horizontal position for the next character of the anchor shifts by 
        # one character box width + the spacing between the boxes
        anchor_position_x += from_bank.box_width + from_bank.box_spacing
    
    # output a confirmation of what image was created
    print(f"Created letterbank with characters '{from_bank.characters_set}': {letterBank_image.width=}, {letterBank_image.height=}")
    
    # if debug - though not really necessary:
    if debug:
        assert letterBank_image.width == from_bank.bank_width
        assert letterBank_image.height == from_bank.bank_height
        if font_size != from_bank.glyph_height:
            print(f"font_size: {font_size} != from_bank.glyph_height: {from_bank.glyph_height}")
    
    # Return the drawn image for printing
    return letterBank_image




if __name__ == "__main__":
    outputPath: str = Path("characterBank") / font_name[:-4]
    outputPath.mkdir(parents=True, exist_ok=True)

    generate_letterBank_image(font_name, large_letters_bank).save(Path(outputPath) / "letterBank.png")
    generate_letterBank_image(font_name, small_letters_bank).save(Path(outputPath) / "smallLetterBank.png")
    generate_letterBank_image(font_name, numbers_bank).save(Path(outputPath) / "numberBank.png")

