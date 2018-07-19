using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Model
{
    /// <summary>
    /// SudokuTile.
    /// </summary>
    [NotMapped]
    public class SudokuTile
    {
        /// <summary>
        /// The row
        /// </summary>
        private int _Row;
        /// <summary>
        /// Gets or sets the row.
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public int Row { get => _Row; set => _Row = value; }

        /// <summary>
        /// The column
        /// </summary>
        private int _Column;
        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public int Column { get => _Column; set => _Column = value; }

        /// <summary>
        /// The value
        /// </summary>
        private int _Value;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value { get => _Value; set => _Value = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuTile" /> class.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        public SudokuTile(int row, int column, int value)
        {
            Row = row;
            Column = column;
            Value = value;
        }
    }
}