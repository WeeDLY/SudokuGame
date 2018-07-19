namespace Library.Model
{
    /// <summary>
    /// GameSession object. Includes Session and SudokuGame
    /// </summary>
    /// <seealso cref="Library.Model.Session" />
    public class GameSession : Session
    {
        /// <summary>
        /// The current sudoku
        /// </summary>
        private SudokuGame _CurrentSudoku;
        /// <summary>
        /// Gets or sets the current sudoku.
        /// </summary>
        /// <value>
        /// The current sudoku.
        /// </value>
        public SudokuGame CurrentSudoku { get => _CurrentSudoku; set => _CurrentSudoku = value; }


        /// <summary>
        /// The preloaded sudoku
        /// </summary>
        private SudokuGame _PreloadedSudoku;
        /// <summary>
        /// Gets or sets the preloaded sudoku.
        /// </summary>
        /// <value>
        /// The preloaded sudoku.
        /// </value>
        public SudokuGame PreloadedSudoku { get => _PreloadedSudoku; set => _PreloadedSudoku = value; }

        /// <summary>
        /// The preloaded ready
        /// </summary>
        private bool _PreloadedReady = false;
        /// <summary>
        /// Gets or sets a value indicating whether [preloaded ready].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [preloaded ready]; otherwise, <c>false</c>.
        /// </value>
        public bool PreloadedReady { get => _PreloadedReady; set => _PreloadedReady = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSession"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public GameSession(Session session) : base(session.UserId, session.Username, session.Authenticated)
        {
            if (session == null)
                return;
            CurrentSudoku = new SudokuGame();
        }
    }
}