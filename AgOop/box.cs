namespace AgOop
{

    internal class BoxConstants
    {
        /// <summary>Answer box identifier </summary>
        internal const int ANSWER = 1;

        /// <summary>Shuffle box identifier </summary>
        internal const int SHUFFLE = 2;

        /// <summary>Controls box identifier (Score + Time)</summary>
        internal const int CONTROLS = 3;


        // Positions and dimensions of the SHUFFLE and ANSWER boxes
        /// <summary>Shuffle and Answer Boxes X position </summary>
        internal const int BOX_START = 30;

        /// <summary>Shuffle Box Y position </summary>
        internal const int SHUFFLEBOX = 110;

        /// <summary> Answer Box Y position </summary>
        internal const int ANSWERBOX = 245;

        /// <summary> Shuffle and Answer Boxes length </summary>
        internal const int BOX_LENGTH = 644;

        // TODO: reconcile with above
        /// <summary>X position of the SHUFFLE and ANSWERS box</summary>
        internal const int BOX_START_X = 80;
        
        /// <summary>Y Position from the top of the window of the SHUFFLE box</summary>
        internal const int SHUFFLE_BOX_Y = 107;
        
        /// <summary>Y Position from the top of the window of the ANSWER box</summary>
        internal const int ANSWER_BOX_Y = 247;


        // Clock box position and dimension
        /// <summary> ClockBox X pixel position </summary>
        internal const int CLOCK_X = 690;

        /// <summary> ClockBox Y pixel position </summary>
        internal const int CLOCK_Y = 35;

        /// <summary> ClockCharacter pixel width </summary>
        internal const int CLOCK_WIDTH = 18;

        /// <summary> ClockCharacter pixel height </summary>
        internal const int CLOCK_HEIGHT = 32;


        // Score Box dimension and position
        /// <summary> ScoreBox X pixel position </summary>
        internal const int SCORE_X = 690;

        /// <summary> ScoreBox Y pixel position </summary>
        internal const int SCORE_Y = 67;

        /// <summary> ScoreCharacter pixel width </summary>
        internal const int SCORE_WIDTH = 18;

        /// <summary> ScoreCharacter pixel height </summary>
        internal const int SCORE_HEIGHT = 32;

        /// <summary>The positioning name of the hotboxes, used in that order in hotbox</summary>
        internal enum HotBoxes { boxSolve, boxNew, boxQuit, boxShuffle, boxEnter, boxClear };
        

    };

    internal class HotBoxes
    {
        /// <summary>The hotboxes (buttons) default positions and dimensions</summary>
        internal static Box[] hotbox = new Box[]
        {
            new Box(x: 612, y: 0, width: 66, height: 30),  /* boxSolve */
            new Box(x: 686, y: 0, width: 46, height: 30),  /* boxNew */
            new Box(x: 742, y: 0, width: 58, height: 30),  /* boxQuit */
            new Box(x: 618, y: 206, width: 66, height: 16),  /* boxShuffle */
            new Box(x: 690, y: 254, width: 40, height: 35),  /* boxEnter */
            new Box(x: 690, y: 304, width: 40, height: 40)   /* boxClear */
        };
    }
    

    /// <summary>  Represents a rectangular box with position and size. 
    //  Also sets the button (hotbox) boxes </summary>
    internal class Box
    {
        /// <summary>x-coordinate of the top left of the box.</summary>
        private int _x;
        internal int x { get { return _x; } set { _x = value; } }

        /// <summary>y-coordinate of the top left of the box.</summary>
        private int _y;
        internal int y { get { return _y; } set { _y = value; } }

        /// <summary>width of the box.</summary>
        private int _width;
        internal int width { get { return _width;} set { _width = value; } }

        /// <summary>height of the box.</summary>
        private int _height;
        internal int height { get { return _height;} set { _height = value; } }

        /// <summary> Constructor for the Box </summary>
        internal Box(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }
    }
}