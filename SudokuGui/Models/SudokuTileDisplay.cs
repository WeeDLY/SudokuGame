using Library.Model;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SudokuGui.Models
{
    /// <summary>
    /// SudokuTileDisplay, this is used to mark colors, etc on SudokuTiles on the board
    /// </summary>
    /// <seealso cref="Library.Model.SudokuTile" />
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class SudokuTileDisplay : SudokuTile, INotifyPropertyChanged
    {
        /// <summary>
        /// The move able
        /// </summary>
        private bool _MoveAble;
        /// <summary>
        /// Gets or sets a value indicating whether [move able].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [move able]; otherwise, <c>false</c>.
        /// </value>
        public bool MoveAble { get => _MoveAble; set => _MoveAble = value; }

        /// <summary>
        /// The misplaced
        /// </summary>
        private bool _Misplaced;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SudokuTileDisplay"/> is misplaced.
        /// </summary>
        /// <value>
        ///   <c>true</c> if misplaced; otherwise, <c>false</c>.
        /// </value>
        public bool Misplaced { get => _Misplaced; set => _Misplaced = value; }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The background color
        /// </summary>
        private SolidColorBrush _BackgroundColor;
        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public SolidColorBrush BackgroundColor
        {
            get => _BackgroundColor;
            set
            {
                _BackgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuTileDisplay"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <param name="moveAble">if set to <c>true</c> [move able].</param>
        public SudokuTileDisplay(int row, int column, int value, bool moveAble) : base(row, column, value)
        {
            MoveAble = moveAble;
            if (MoveAble)
            {
                BackgroundColor = new SolidColorBrush(Colors.LightBlue);
            }
            else
            {
                BackgroundColor = new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}