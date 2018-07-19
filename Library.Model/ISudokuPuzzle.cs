namespace Library.Model
{
    /// <summary>
    /// Interface for SudokuPuzzle
    /// </summary>
    interface ISudokuPuzzle
    {
        /// <summary>
        /// Completeds the puzzle.
        /// </summary>
        /// <param name="puzzleCleared">if set to <c>true</c> [puzzle cleared].</param>
        void CompletedPuzzle(bool puzzleCleared);

        /// <summary>
        /// Starts the timer.
        /// </summary>
        void StartTimer();
    }
}