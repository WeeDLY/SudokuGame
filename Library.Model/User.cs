using System.ComponentModel.DataAnnotations;

namespace Library.Model
{
    /// <summary>
    /// Abstract class, used in RegisterUser and Session
    /// </summary>
    public abstract class User
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Key]
        public virtual int UserId { get; set; }

        /// <summary>
        /// The username
        /// </summary>
        private string _Username;
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get => _Username; set => _Username = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        /// <param name="username">The username.</param>
        public User(string userName)
        {
            Username = userName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="username">The username.</param>
        public User(int userId, string userName)
        {
            UserId = userId;
            Username = userName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// Empty Constructor for Newtonsoft.Json, used when deserealizing
        /// </summary>
        public User() { }
    }
}
