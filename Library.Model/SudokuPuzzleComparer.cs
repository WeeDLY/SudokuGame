using System.Collections.Generic;

namespace Library.Model
{
    /// <summary>
    /// Comparer for SudokuPuzzle class
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IComparer{Library.Model.SudokuPuzzle}" />
    public class SudokuPuzzleComparer :IComparer<SudokuPuzzle>
    {
        /// <summary>
        /// true: Sort ascending
        /// false: Sort Descending
        /// </summary>
        private bool Ascending;

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuPuzzleComparer"/> class.
        /// </summary>
        /// <param name="Ascending">if set to <c>true</c> [ascending].</param>
        public SudokuPuzzleComparer(bool Ascending)
        {
            this.Ascending = Ascending;
        }

        /// <summary>
        /// Compares two SudokuPuzzle objects.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns></returns>
        public int Compare(SudokuPuzzle p1, SudokuPuzzle p2)
        {
            if (p1 == null && p2 == null) return 0;
            if (p1 == null) return -1;
            if (p2 == null) return 1;

            return Ascending ? p1.GetTimer.CompareTo(p2.GetTimer) : p2.GetTimer.CompareTo(p1.GetTimer);
        }
    }
}
