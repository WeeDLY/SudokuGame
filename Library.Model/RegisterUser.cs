using System;
using System.Collections.Generic;

namespace Library.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Library.Model.User" />
    public class RegisterUser : User
    {
        #region Properties/Fields

        /// <summary>
        /// The date joined
        /// </summary>
        private DateTime _DateJoined;
        /// <summary>
        /// Gets or sets the date joined.
        /// </summary>
        /// <value>
        /// The date joined.
        /// </value>
        public DateTime DateJoined { get => _DateJoined; set => _DateJoined = value; }

        /// <summary>
        /// The password
        /// </summary>
        private string _Password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get => _Password; set => _Password = value; }

        /// <summary>
        /// The salt
        /// </summary>
        private string _Salt;
        /// <summary>
        /// Gets or sets the salt.
        /// </summary>
        /// <value>
        /// The salt.
        /// </value>
        public string Salt { get => _Salt; set => _Salt = value; }

        /// <summary>
        /// Gets or sets the sudoku puzzles.
        /// </summary>
        /// <value>
        /// The sudoku puzzles.
        /// </value>
        public virtual List<SudokuPuzzle> SudokuPuzzles { get; } = new List<SudokuPuzzle>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUser"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="dateJoined">The date joined.</param>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        public RegisterUser(string username, string password, string salt) : base(username)
        {
            DateJoined = DateTime.Now;
            Password = password;
            Salt = salt;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterUser"/> class.
        /// Empty Constructor for Newtonsoft.Json, used when deserealizing
        /// </summary>
        public RegisterUser() : base(null) { }
    }
}