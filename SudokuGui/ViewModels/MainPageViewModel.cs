using Library.Log;
using Library.Model;
using Library.Utility;
using SudokuGui.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SudokuGui.ViewModels
{
    /// <summary>
    /// ViewModel for MainPage
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class MainPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Properties/Fields

        /// <summary>
        /// The database
        /// </summary>
        private DatabaseClient Database = new DatabaseClient();

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The user session
        /// </summary>
        private GameSession _UserSession;
        /// <summary>
        /// Gets or sets the user session.
        /// </summary>
        /// <value>
        /// The user session.
        /// </value>
        public GameSession UserSession
        {
            get => _UserSession;
            set { Set(ref _UserSession, value); }
        }

        /// <summary>
        /// The application text
        /// </summary>
        private string _ApplicationText;
        /// <summary>
        /// Gets or sets the application text.
        /// </summary>
        /// <value>
        /// The application text.
        /// </value>
        public string ApplicationText
        {
            get => _ApplicationText;
            set { Set(ref _ApplicationText, value); }
        }

        /// <summary>
        /// The sudoku board display
        /// </summary>
        private ObservableCollection<SudokuTileDisplay> _SudokuBoardDisplay;
        /// <summary>
        /// Gets or sets the sudoku board display.
        /// </summary>
        /// <value>
        /// The sudoku board display.
        /// </value>
        public ObservableCollection<SudokuTileDisplay> SudokuBoardDisplay
        {
            get => _SudokuBoardDisplay;
            set { Set(ref _SudokuBoardDisplay, value); }
        }

        /// <summary>
        /// The generate sudoku board
        /// </summary>
        private bool _GenerateSudokuBoard;
        /// <summary>
        /// Gets or sets a value indicating whether [generate sudoku board].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [generate sudoku board]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateSudokuBoard
        {
            get => _GenerateSudokuBoard;
            set
            {
                _GenerateSudokuBoard = value;
                OnPropertyChanged(nameof(GenerateSudokuBoard));
            }
        }

        /// <summary>
        /// The show new puzzle button
        /// </summary>
        private bool _ShowNewPuzzleButton = true;
        /// <summary>
        /// Gets or sets a value indicating whether [show new puzzle button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show new puzzle button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNewPuzzleButton
        {
            get => _ShowNewPuzzleButton;
            set
            {
                if(_ShowNewPuzzleButton != value)
                {
                    _ShowNewPuzzleButton = value;
                    OnPropertyChanged(nameof(ShowNewPuzzleButton));
                }
            }
        }

        /// <summary>
        /// The enable buttons
        /// </summary>
        private bool _EnableButtons;
        /// <summary>
        /// Gets or sets a value indicating whether [enable buttons].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable buttons]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableButtons
        {
            get => _EnableButtons;
            set
            {
                if (_EnableButtons != value)
                {
                    _EnableButtons = value;
                    OnPropertyChanged(nameof(EnableButtons));
                }
            }
        }

        /// <summary>
        /// The cancel token
        /// </summary>
        private CancellationTokenSource _CancelToken;
        /// <summary>
        /// Gets or sets the cancel token.
        /// </summary>
        /// <value>
        /// The cancel token.
        /// </value>
        public CancellationTokenSource CancelToken
        {
            get => _CancelToken;
            set => _CancelToken = value;
        }

        /// <summary>
        /// If the SudokuPuzzle, has already been registered to the Database
        /// </summary>
        private bool RegisterPuzzle = false;

        #endregion

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            SudokuBoardDisplay = new ObservableCollection<SudokuTileDisplay>();
        }

        #region Navigation

        /// <summary>
        /// Called when [navigated to asynchronous].
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="suspensionState">State of the suspension.</param>
        /// <returns></returns>
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            NavigationService.ClearHistory();

            Views.Shell.HamburgerMenu.IsFullScreen = false;

            if(parameter != null)
            {
                try
                {
                    UserSession = new GameSession(parameter as Session);
                }
                catch(InvalidCastException e)
                {
                    await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}");
                }
                catch (Exception e)
                {
                    await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}");
                }
            }

            if(UserSession.CurrentSudoku.SudokuStartPosition.Count > 0)
            {
                UserSession.CurrentSudoku.SudokuStartPosition.Clear();
            }
            if(SudokuBoardDisplay.Count > 0)
            {
                SudokuBoardDisplay.Clear();
            }

            string offline = (UserSession.Authenticated == true) ? "" : "(Offline)";
            ApplicationText = $"Sudoku - {UserSession.Username}{offline}";

            await Task.CompletedTask;
        }

        /// <summary>
        /// Called when [navigated from asynchronous].
        /// </summary>
        /// <param name="suspensionState">State of the suspension.</param>
        /// <param name="suspending">if set to <c>true</c> [suspending].</param>
        /// <returns></returns>
        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="E:NavigatingFromAsync" /> event.
        /// </summary>
        /// <param name="args">The <see cref="NavigatingEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            ButtonCancelNewPuzzle();

            args.Cancel = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Gotoes the settings.
        /// </summary>
        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        /// <summary>
        /// Gotoes the privacy.
        /// </summary>
        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        /// <summary>
        /// Goes to statistics.
        /// </summary>
        public void GoToStatistics() =>
            NavigationService.Navigate(typeof(Views.StatisticPage));

        /// <summary>
        /// Goes to logout.
        /// </summary>
        public void GoToLogout()
        {
            NavigationService.ClearHistory();
            NavigationService.Navigate(typeof(Views.LoginPage));
        }
        #endregion

        /// <summary>
        /// Fills the board with the correct solution.
        /// Please note that this does not actually Solve, the SudokuBoard.
        /// </summary>
        public void ButtonSolve()
        {
            if(UserSession.CurrentSudoku.SolvedBoard == null)
            {
                UserDialog.ShowMessageDialogAsync("Error message", "Something went wrong, please create a new sudoku puzzle");
                return;
            }
            FillBoard(UserSession.CurrentSudoku.SolvedBoard, UserSession.CurrentSudoku.SudokuStartPosition);
            CompletedPuzzleAsync(false);
        }

        /// <summary>
        /// Cancels the new puzzle.
        /// </summary>
        public void ButtonCancelNewPuzzle()
        {
            if(CancelToken != null)
            {
                if (CancelToken.Token.CanBeCanceled)
                {
                    GenerateSudokuBoard = false;
                    CancelToken.Cancel();
                }
            }
        }

        /// <summary>
        /// Generate New SudokuPuzzle
        /// </summary>
        public async void ButtonNewPuzzleAsync()
        {
            if(UserSession.PreloadedReady)
            {
                UserSession.CurrentSudoku = new SudokuGame(UserSession.PreloadedSudoku);
                UserSession.PreloadedReady = false;

                FillBoard(UserSession.CurrentSudoku.SudokuBoard, UserSession.CurrentSudoku.SudokuStartPosition);
                UserSession.CurrentSudoku.StartTimer();

                ResetButtons();
                return;
            }

            GenerateSudokuBoard = true;
            EnableButtons = false;
            ShowNewPuzzleButton = false;

            CancelToken = new CancellationTokenSource(3000);

            try
            {
                await Task.Run(() => { UserSession.CurrentSudoku.PopulateBoardAsync(20, CancelToken.Token); }, CancelToken.Token);
            }
            catch(TaskCanceledException e)
            {
                await Logger.LogAsync(LogLevel.Info, $"{nameof(e)}: {e.Message}");
                ResetButtons();
                return;
            }
            catch(Exception e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
                return;
            }

            if (UserSession.CurrentSudoku.SolvedBoard == null)
            {
                if (GenerateSudokuBoard)
                {
                    ButtonNewPuzzleAsync();
                }
                else
                {
                    ResetButtons();
                }
            }
            else
            {
                FillBoard(UserSession.CurrentSudoku.SudokuBoard, UserSession.CurrentSudoku.SudokuStartPosition);
                UserSession.CurrentSudoku.StartTimer();

                ResetButtons();
            }
        }

        /// <summary>
        /// Preloads a SudokuPuzzle for the user.
        /// </summary>
        private async void Preload()
        {
            UserSession.PreloadedSudoku = new SudokuGame();
            CancellationTokenSource cancelToken = new CancellationTokenSource(3000);
            await Task.Run(() => { UserSession.PreloadedSudoku.PopulateBoardAsync(20, cancelToken.Token); }, cancelToken.Token);
            if(UserSession.PreloadedSudoku.SolvedBoard == null)
            {
                Preload();
            }
            else
                UserSession.PreloadedReady = true;
        }

        /// <summary>
        /// Resets the buttons.
        /// </summary>
        private void ResetButtons()
        {
            GenerateSudokuBoard = false;
            ShowNewPuzzleButton = true;
            EnableButtons = true;
            RegisterPuzzle = false;

            // Preloads a new puzzle.
            Preload();
        }

        /// <summary>
        /// Checks the progress of the SudokuPuzzle
        /// </summary>
        public void ButtonCheckProgress()
        {
            if (UserSession.CurrentSudoku.SudokuBoard == null)
                return;

            bool filledBoard = UpdateBoardDisplay();

            List<SudokuTile> playerMistakes = UserSession.CurrentSudoku.ValidBoard(UserSession.CurrentSudoku.SudokuBoard, true);
            try
            {
                foreach(SudokuTileDisplay tile in SudokuBoardDisplay)
                {
                    if (tile.MoveAble)
                        continue;
                    tile.BackgroundColor = new SolidColorBrush(Colors.White);
                    foreach(SudokuTile invalidTiles in playerMistakes)
                    {
                        SolidColorBrush brush = new SolidColorBrush();
                        if (invalidTiles.Row == tile.Row && invalidTiles.Column == tile.Column)
                        {
                            tile.BackgroundColor = new SolidColorBrush(Colors.Red);
                            break;
                        }
                    }
                }
            }
            catch(ArgumentOutOfRangeException e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}"));
                return;
            }
            catch(Exception e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}"));
                throw;
            }

            if(filledBoard && playerMistakes.Count == 0)
            {
                if (UserSession.CurrentSudoku.StartDate != default(DateTime))
                    CompletedPuzzleAsync(true);
            }
        }

        /// <summary>
        /// BtnCheat. Fills the board, but do not send request, that the program solved it for you
        /// </summary>
        public void ButtonCheat()
        {
            FillBoard(UserSession.CurrentSudoku.SolvedBoard, UserSession.CurrentSudoku.SudokuStartPosition);
        }

        /// <summary>
        /// User completed the puzzle
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        private async void CompletedPuzzleAsync(bool result)
        {
            if (UserSession.Authenticated != true)
            {
                UserSession.CurrentSudoku.CompletedPuzzle(result);
                PuzzleCompletedDialog();
                return;
            }

            if (!RegisterPuzzle)
            {
                UserSession.CurrentSudoku.CompletedPuzzle(result);
                if(UserSession.UserId != 1)
                {
                    bool registered = await Database.RegisterPuzzleAsync(UserSession.CurrentSudoku, UserSession.UserId);
                    // Could not register puzzle result, ask user if he/she wants to retry
                    if (!registered)
                    {
                        UserDialogResponse userResp = await UserDialog.ShowMessageDialogOptionsAsync("Unable to register puzzle", "Do you want to retry registering the puzzle?");
                        if (userResp == UserDialogResponse.Yes)
                            CompletedPuzzleAsync(result);
                    }
                }
            }
            else
            {
                RegisterPuzzle = true;
                UserSession.CurrentSudoku = new SudokuGame();
            }
            PuzzleCompletedDialog();
        }

        /// <summary>
        /// Puzzles the completed dialog.
        /// </summary>
        private void PuzzleCompletedDialog()
        {
            if (UserSession.CurrentSudoku.PuzzleCleared)
            {
                UserDialog.ShowMessageDialogAsync("Puzzle Cleared", $"You completed the puzzle in: {UserSession.CurrentSudoku.GetTimer}");
            }
            else
            {
                UserDialog.ShowMessageDialogAsync("Game Over", $"You failed the Sudoku Puzzle, better luck next time");
            }
        }


        /// <summary>
        /// Updates the progress and replaces SudokuTileDisplay, with new objects, that are colored correctly.
        /// </summary>
        /// <returns>
        /// true if board is filled, false otherwise
        /// </returns>
        private bool UpdateBoardDisplay()
        {
            bool filledBoard = true;

            try
            {
                foreach (SudokuTileDisplay tile in SudokuBoardDisplay)
                {
                    if (tile.Value == 0)
                        filledBoard = false;
                    UserSession.CurrentSudoku.SudokuBoard[tile.Row][tile.Column] = tile.Value;
                }
                return filledBoard;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}"));
            }
            catch(Exception e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}"));
                throw;
            }
            return false;
        }
        
        /// <summary>
        /// Fills the board with SudokuBoard
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="startPosition">The start position.</param>
        private void FillBoard(int[][] board, List<SudokuTile> startPosition)
        {
            try
            {
                SudokuBoardDisplay.Clear();
                for (int row = 0; row < board.Length; row++)
                {
                    for (int col = 0; col < board.Length; col++)
                    {
                        SudokuBoardDisplay.Add(new SudokuTileDisplay(row, col, board[row][col], TileMoveAble(startPosition, row, col)));
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}"));
            }
            catch(Exception e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Error, $"{nameof(e)}: {e.Message}"));
            }
        }

        /// <summary>
        /// Tiles the move able.
        /// </summary>
        /// <param name="sudokuTiles">The sudoku tiles.</param>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private bool TileMoveAble(List<SudokuTile> sudokuTiles, int row, int col)
        {
            foreach(SudokuTile sTile in sudokuTiles)
            {
                if (sTile.Row == row && sTile.Column == col)
                    return true;
            }
            return false;
        }
    }
}