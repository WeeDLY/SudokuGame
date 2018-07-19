using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SudokuPuzzle : ISudokuPuzzle
    {
        #region Properties/Fields
        /// <summary>
        /// The sudoku puzzle identifier
        /// </summary>
        [Key]
        private int _SudokuPuzzleId;
        /// <summary>
        /// Gets or sets the sudoku puzzle identifier.
        /// </summary>
        /// <value>
        /// The sudoku puzzle identifier.
        /// </value>
        public int SudokuPuzzleId { get => _SudokuPuzzleId; set => _SudokuPuzzleId = value; }

        /// <summary>
        /// The start date
        /// </summary>
        private DateTime _StartDate;
        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime StartDate { get => _StartDate; set => _StartDate = value; }

        /// <summary>
        /// The end date
        /// </summary>
        private DateTime _EndDate;
        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>
        /// The end date.
        /// </value>
        public DateTime EndDate { get => _EndDate; set => _EndDate = value; }

        /// <summary>
        /// The puzzle cleared
        /// </summary>
        private bool _PuzzleCleared;
        /// <summary>
        /// Gets or sets a value indicating whether [puzzle cleared].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [puzzle cleared]; otherwise, <c>false</c>.
        /// </value>
        public bool PuzzleCleared { get => _PuzzleCleared; set => _PuzzleCleared = value; }

        /// <summary>
        /// Gets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        public string Summary
        {
            get => $"PuzzleId: {SudokuPuzzleId}, Time: {GetTimer} | Cleared: {PuzzleCleared}";
        }

        /// <summary>
        /// Gets the get time used
        /// </summary>
        /// <value>
        /// The get timer.
        /// </value>
        public TimeSpan GetTimer
        {
            get => GetTimeUsed();
        }

        /// <summary>
        /// Gets or sets the register users.
        /// </summary>
        /// <value>
        /// The register users.
        /// </value>
        public virtual List<RegisterUser> RegisterUsers { get; set; } = new List<RegisterUser>();
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuPuzzle"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="puzzleCleared">if set to <c>true</c> [puzzle cleared].</param>
        public SudokuPuzzle(bool puzzleCleared = false)
        {
            StartDate = DateTime.Now;
            PuzzleCleared = puzzleCleared;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuPuzzle"/> class.
        /// Empty Constructor for Newtonsoft.Json, used when deserealizing
        /// </summary>
        public SudokuPuzzle() { }

        /// <summary>
        /// Completeds the puzzle.
        /// </summary>
        /// <param name="puzzleCleared">if set to <c>true</c> [puzzle cleared].</param>
        public void CompletedPuzzle(bool puzzleCleared)
        {
            if(EndDate == default(DateTime))
            {
                PuzzleCleared = puzzleCleared;
                EndDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {
            StartDate = DateTime.Now;
        }

        /// <summary>
        /// Gets the current time, the user used on the SudokuPuzzle
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetTimeUsed()
        {
            if(StartDate == default(DateTime))
            {
                return TimeSpan.FromSeconds(0);
            }

            if(EndDate == default(DateTime))
            {
                return DateTime.Now - StartDate;
            }
            else
            {
                return EndDate - StartDate;
            }
        }
    }
}