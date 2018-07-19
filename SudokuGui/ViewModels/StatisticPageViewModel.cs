using Library.Log;
using Library.Model;
using Library.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace SudokuGui.ViewModels
{
    /// <summary>
    /// ViewModel for StatisticPage
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class StatisticPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Properties/Fields

        /// <summary>
        /// The database
        /// </summary>
        private DatabaseClient Database = new DatabaseClient();
        /// <summary>
        /// The search delay
        /// </summary>
        private const int SearchDelay = 500;
        /// <summary>
        /// The maximum column height
        /// </summary>
        private const int MaxColumnHeight = 300;

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The search
        /// </summary>
        private string _Search;
        /// <summary>
        /// Gets or sets the search.
        /// </summary>
        /// <value>
        /// The search.
        /// </value>
        public string Search
        {
            get => _Search;
            set => Set(ref _Search, value);
        }

        /// <summary>
        /// The ascending
        /// </summary>
        private bool _Ascending;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StatisticPageViewModel" /> is ascending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ascending; otherwise, <c>false</c>.
        /// </value>
        public bool Ascending
        {
            get => _Ascending;
            set => Set(ref _Ascending, value);
        }

        /// <summary>
        /// The cleared puzzles
        /// </summary>
        private int _ClearedPuzzles;
        /// <summary>
        /// Gets or sets the cleared puzzles.
        /// </summary>
        /// <value>
        /// The cleared puzzles.
        /// </value>
        public int ClearedPuzzles
        {
            get => _ClearedPuzzles;
            set
            {
                if(_ClearedPuzzles != value)
                {
                    _ClearedPuzzles = value;
                    OnPropertyChanged(nameof(ClearedPuzzles));
                    OnPropertyChanged(nameof(ClearedPuzzlesColumn));
                }
            }
        }

        /// <summary>
        /// The failed puzzles
        /// </summary>
        private int _FailedPuzzles;
        /// <summary>
        /// Gets or sets the failed puzzles.
        /// </summary>
        /// <value>
        /// The failed puzzles.
        /// </value>
        public int FailedPuzzles
        {
            get => _FailedPuzzles;
            set
            {
                if (_FailedPuzzles != value)
                {
                    _FailedPuzzles = value;
                    OnPropertyChanged(nameof(FailedPuzzles));
                    OnPropertyChanged(nameof(FailedPuzzlesColumn));
                }
            }
        }

        /// <summary>
        /// Gets the cleared puzzles column.
        /// </summary>
        /// <value>
        /// The cleared puzzles column.
        /// </value>
        public int ClearedPuzzlesColumn
        {
            get
            {
                if (SudokuList.Count == 0)
                    return 1;
                return (MaxColumnHeight / SudokuList.Count) * ClearedPuzzles;
            }
        }

        /// <summary>
        /// Gets the failed puzzles column.
        /// </summary>
        /// <value>
        /// The failed puzzles column.
        /// </value>
        public int FailedPuzzlesColumn
        {
            get
            {
                if (SudokuList.Count == 0)
                    return 1;
                return (MaxColumnHeight / SudokuList.Count) * FailedPuzzles;
            }
        }

        /// <summary>
        /// The search timer
        /// </summary>
        private Timer searchTimer = null;

        private bool _ShowProgressRing;
        public bool ShowProgressRing
        {
            get => _ShowProgressRing;
            set
            {
                _ShowProgressRing = value;
                OnPropertyChanged(nameof(ShowProgressRing));
            }
        }


        /// <summary>
        /// The sudoku list
        /// </summary>
        private List<SudokuPuzzle> _SudokuList;
        /// <summary>
        /// Gets or sets the sudoku list.
        /// </summary>
        /// <value>
        /// The sudoku list.
        /// </value>
        public List<SudokuPuzzle> SudokuList { get => _SudokuList; set => _SudokuList = value; }

        /// <summary>
        /// The sudoku top
        /// </summary>
        private ObservableCollection<SudokuPuzzle> _SudokuTop;
        /// <summary>
        /// Gets or sets the sudoku top.
        /// </summary>
        /// <value>
        /// The sudoku top.
        /// </value>
        public ObservableCollection<SudokuPuzzle> SudokuTop { get => _SudokuTop; set { Set(ref _SudokuTop, value); } }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticPageViewModel"/> class.
        /// </summary>
        public StatisticPageViewModel()
        {
            Ascending = true;
            SudokuList = new List<SudokuPuzzle>();
            SudokuTop = new ObservableCollection<SudokuPuzzle>();
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region AutoFireSearch
        /// <summary>
        /// Disposes the objects.
        /// </summary>
        private void DisposeObjects()
        {
            if (searchTimer != null)
            {
                searchTimer.Dispose();
                searchTimer = null;
            }
        }

        /// <summary>
        /// Timers the elapsed.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void TimerElapsed(Object obj)
        {
            try
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                async () =>
                {
                    await BtnSearchAsync();
                });
            }
            catch(OperationCanceledException e)
            {
                await Logger.LogAsync(LogLevel.Info, $"{nameof(e)}: {e.Message}");
            }
            catch(Exception e)
            {
                await Logger.LogAsync(LogLevel.Warning, $"{nameof(e)}: {e.Message}");
            }
        }

        /// <summary>
        /// Texts the search text changing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="TextBoxTextChangingEventArgs"/> instance containing the event data.</param>
        public void TextSearch_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            try
            {
                TextBox t = sender as TextBox;
                Search = t.Text;
           
                DisposeObjects();
                searchTimer = new Timer(TimerElapsed, null, SearchDelay, SearchDelay);
            }
            catch(InvalidCastException e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}"));
            }
            catch (Exception e)
            {
                Task.Run(async () => await Logger.LogAsync(LogLevel.Critical, $"{nameof(e)}: {e.Message}"));
            }
        }
        #endregion


        /// <summary>
        /// BTNs the search asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task BtnSearchAsync()
        {
            ShowProgressRing = true;
            DisposeObjects();

            int userId = await Database.GetUserIdAsync(Search);

            if(userId == -1)
            {
                UserDialog.ShowMessageDialogAsync("Stat search", $"Username {Search} was not found");
                ResetStats();
                ShowProgressRing = false;
                return;
            }

            SudokuList = await Database.GetUserSudokuPuzzlesAsync(userId);

            DisplayStats();

            ShowProgressRing = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Displays the stats.
        /// </summary>
        private void DisplayStats()
        {
            ResetStats();
            GetTop5(Ascending);

            SudokuList.ForEach(sudoku =>
            {
                if (sudoku.PuzzleCleared)
                    ClearedPuzzles++;
                else
                    FailedPuzzles++;
            });
        }

        /// <summary>
        /// Resets the stats.
        /// </summary>
        private void ResetStats()
        {
            SudokuTop.Clear();

            FailedPuzzles = 0;
            ClearedPuzzles = 0;
        }

        /// <summary>
        /// Gets the top5.
        /// </summary>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        private void GetTop5(bool ascending)
        {
            SudokuList.Sort(new SudokuPuzzleComparer(ascending));
            int added = 0;
            foreach(SudokuPuzzle puzzle in SudokuList)
            {
                if(added < 5)
                {
                    SudokuTop.Add(puzzle);
                }
                else
                {
                    break;
                }
                added++;
            }
        }
    }
}