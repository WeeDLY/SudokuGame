using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.Model
{
    /// <summary>
    /// SudokuGame class
    /// </summary>
    /// <seealso cref="Library.Model.SudokuPuzzle" />
    public class SudokuGame : SudokuPuzzle
    {
        #region Properties/Fields
        private Random random = new Random();

        /// <summary>
        /// The sudoku board
        /// </summary>
        private int[][] _SudokuBoard;
        /// <summary>
        /// Gets or sets the sudoku board.
        /// </summary>
        /// <value>
        /// The sudoku board.
        /// </value>
        public int[][] SudokuBoard { get => _SudokuBoard; set => _SudokuBoard = value; }

        /// <summary>
        /// The sudoku tiles
        /// </summary>
        private List<SudokuTile> _SudokuStartPosition;
        /// <summary>
        /// Gets or sets the sudoku tiles.
        /// </summary>
        /// <value>
        /// The sudoku tiles.
        /// </value>
        public List<SudokuTile> SudokuStartPosition { get => _SudokuStartPosition; set => _SudokuStartPosition = value; }

        /// <summary>
        /// The solved board
        /// </summary>
        private int[][] _SolvedBoard;
        public int[][] SolvedBoard { get => _SolvedBoard; set => _SolvedBoard = value; }
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuGame" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        public SudokuGame() : base()
        {
            SudokuStartPosition = new List<SudokuTile>();
        }

        public SudokuGame(SudokuGame game)
        {
            this.SudokuBoard = game.SudokuBoard;
            this.SudokuStartPosition = game.SudokuStartPosition;
            this.SolvedBoard = game.SolvedBoard;
        }

        #region Generate SudokuPuzzle
        /// <summary>
        /// Populates the board asynchronous.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public async void PopulateBoardAsync(int amount, CancellationToken cts)
        {
            // Clear previous BoardState, if any
            if(SudokuStartPosition.Count > 0)
                SudokuStartPosition.Clear();

            int[][] board = GenerateEmptyBoard();

            int added = 0;
            while (added < amount)
            {
                SudokuTile sTile = GenerateSudokuTile();
                if(board[sTile.Row][sTile.Column] == 0)
                {
                    board[sTile.Row][sTile.Column] = sTile.Value;
                    if (ValidBoard(board, false).Count == 0)
                    {
                        SudokuStartPosition.Add(sTile);
                        added++;
                    }
                    else
                        board[sTile.Row][sTile.Column] = 0;
                }
            }

            SudokuBoard = CloneBoard(board);
            SolvedBoard = await SolveAsync(CloneBoard(SudokuBoard), cts);
        }

        /// <summary>
        /// Generates the board.
        /// </summary>
        /// <returns></returns>
        private int[][] GenerateEmptyBoard()
        {
            int[][] board = new int[9][];
            for(int i = 0; i < board.Length; i++)
            {
                board[i] = new int[9];
            }
            return board;
        }

        /// <summary>
        /// Clones the board.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private int[][] CloneBoard(int[][] board)
        {
            return board.Select(s => s.ToArray()).ToArray();
        }

        /// <summary>
        /// Generates the sudoku tile.
        /// </summary>
        /// <returns></returns>
        private SudokuTile GenerateSudokuTile()
        {
            return new SudokuTile(RandomNumber(0, 8), RandomNumber(0, 8), RandomNumber(1, 9));
        }

        #endregion

        #region Solves SudokuPuzzle
        /// <summary>
        /// Solves the SudokuBoard asynchronous.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="cts">The CTS.</param>
        /// <returns></returns>
        public async Task<int[][]> SolveAsync(int[][] board, CancellationToken cts)
        {
            bool backtrack = false;
            int row = 0, col = 0;
            while (row < 9 && col < 9)
            {
                // Stops trying to solve the board
                if (UnsolveableBoard(row, col) || cts.IsCancellationRequested)
                {
                    return null;
                }

                int currNum = board[row][col];
                if((currNum == 0 || backtrack) && MoveableTile(row, col))
                {
                    currNum = (currNum == 0) ? 1 : ++currNum;
                    bool added = AddTile(board, row, col, currNum);
                    // added == false, vil si at vi må backtracke. Da det er ingen valid tall for SudokuTile.
                    backtrack = !added;
                }
                col = IncrementCol(!backtrack, col, row);
                row = IncrementRow(!backtrack, col, row);
            }

            return await Task.FromResult(board);
        }
        
        /// <summary>
        /// Checks if the board is unsolveable
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private bool UnsolveableBoard(int row, int col)
        {
            if (row < 0 || row > 9)
            {
                return true;
            }
            if (col < 0 || col > 9)
            {
                return true;
            }
            return false;
        }

        private int IncrementCol(bool up, int col, int row)
        {
            if (up)
            {
                if (row == 8)
                    return ++col;
            }
            else
            {
                if (row == 0)
                    return --col;
            }
            return col;
        }
        private int IncrementRow(bool up, int col, int row)
        {
            if (up)
            {
                if (row == 8)
                    return 0;
                return ++row;
            }
            else
            {
                if (row == 0)
                    return 8;
                return --row;
            }
        }

        private bool AddTile(int[][] board, int row, int col, int value)
        {
            for(int i = value; i <= 9; i++)
            {
                board[row][col] = i;
                List<SudokuTile> invalid = ValidBoard(board, false);
                if (invalid.Count == 0)
                    return true;
            }
            board[row][col] = 0;
            return false;
        }

        private bool MoveableTile(int row, int col)
        {
            try
            {
                foreach (SudokuTile tile in SudokuStartPosition)
                    if (tile.Row == row && tile.Column == col)
                        return false;
                return true;
            }
            catch (InvalidOperationException e)
            {
                Debug.WriteLine($"{nameof(e)}: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(e)}: {e.Message}");
            }
            return false;
        }

        #endregion

        #region Valid SudokuBoard

        /// <summary>
        /// Valids the board.
        /// </summary>
        /// <param name="boardState">State of the board.</param>
        /// <param name="getAll">if set to <c>true</c> [get all].</param>
        /// <returns></returns>
        public List<SudokuTile> ValidBoard(int[][] boardState, bool getAll)
        {
            List<SudokuTile> invalidPos = new List<SudokuTile>();

            invalidPos.AddRange(CheckRow(boardState, getAll));
            if(invalidPos.Count > 0 && !getAll)
            {
                return invalidPos;
            }

            invalidPos.AddRange(CheckCol(boardState, getAll));
            if (invalidPos.Count > 0 && !getAll)
            {
                return invalidPos;
            }

            invalidPos.AddRange(CheckGrid(boardState, getAll));

            return invalidPos;
        }

        /// <summary>
        /// Checks the row.
        /// </summary>
        /// <param name="boardState">State of the board.</param>
        /// <param name="getAll">if set to <c>true</c> [get all].</param>
        /// <returns></returns>
        private static List<SudokuTile> CheckRow(int[][] boardState, bool getAll)
        {
            List<SudokuTile> invalid = new List<SudokuTile>();
            for (int row = 0; row < 9; row++)
            {
                List<SudokuTile> numbers = new List<SudokuTile>();
                for (int col = 0; col < 9; col++)
                {
                    int currNum = boardState[row][col];
                    if (currNum != 0)
                    {
                        foreach (SudokuTile sudokuTile in numbers)
                        {
                            if (sudokuTile.Value == currNum)
                            {
                                invalid.Add(sudokuTile);
                                invalid.Add(new SudokuTile(row, col, currNum));
                                if (!getAll)
                                    return invalid;
                            }
                        }
                        numbers.Add(new SudokuTile(row, col, currNum));
                    }
                }
                numbers.Clear();
            }
            return invalid;
        }

        /// <summary>
        /// Checks the col.
        /// </summary>
        /// <param name="boardState">State of the board.</param>
        /// <param name="getAll">if set to <c>true</c> [get all].</param>
        /// <returns></returns>
        private static List<SudokuTile> CheckCol(int[][] boardState, bool getAll)
        {
            List<SudokuTile> invalid = new List<SudokuTile>();
            for (int col = 0; col < 9; col++) 
            {
                List<SudokuTile> numbers = new List<SudokuTile>();
                for (int row = 0; row < 9; row++)
                {
                    int currNum = boardState[row][col];
                    if (currNum != 0)
                    {
                        foreach (SudokuTile s in numbers)
                        {
                            if (s.Value == currNum)
                            {
                                invalid.Add(s);
                                invalid.Add(new SudokuTile(row, col, currNum));
                                if (!getAll)
                                    return invalid;
                            }
                        }
                        numbers.Add(new SudokuTile(row, col, currNum));
                    }
                }
                numbers.Clear();
            }
            return invalid;
        }

        /// <summary>
        /// Checks the grid.
        /// </summary>
        /// <param name="boardState">State of the board.</param>
        /// <param name="getAll">if set to <c>true</c> [get all].</param>
        /// <returns></returns>
        private static List<SudokuTile> CheckGrid(int[][] boardState, bool getAll)
        {
            List<SudokuTile> invalid = new List<SudokuTile>();
            for (int colStart = 0; colStart <= 6; colStart += 3)
            {
                for (int rowStart = 0; rowStart <= 6; rowStart += 3)
                {
                    List<SudokuTile> numbers = new List<SudokuTile>();
                    for (int c = 0; c < 3; c++)
                    {
                        for (int r = 0; r < 3; r++)
                        {
                            int col = c + colStart;
                            int row = r + rowStart;
                            int currNum = boardState[row][col];
                            if (currNum != 0)
                            {
                                foreach (SudokuTile s in numbers)
                                {
                                    if (s.Value == currNum)
                                    {
                                        invalid.Add(s);
                                        invalid.Add(new SudokuTile(row, col, currNum));
                                        if (!getAll)
                                            return invalid;
                                    }
                                }
                                numbers.Add(new SudokuTile(row, col, currNum));
                            }
                        }
                    }
                }
            }
            return invalid;
        }

        #endregion


        /// <summary>
        /// Generates random number
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        private int RandomNumber(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }
}