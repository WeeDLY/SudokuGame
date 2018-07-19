namespace Library.Model
{
    /// <summary>
    /// Session is used after the user is logged in.
    /// </summary>
    /// <seealso cref="Library.Model.User" />
    public class Session : User
    {
        /// <summary>
        /// The authenticated
        /// </summary>
        private bool? _Authenticated;
        /// <summary>
        /// Gets or sets the authenticated.
        /// </summary>
        /// <value>
        /// The authenticated.
        /// </value>
        public bool? Authenticated { get => _Authenticated; set => _Authenticated = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="authenticated">The authenticated.</param>
        public Session(int id, string username, bool? authenticated = false) : base(id, username)
        {
            Authenticated = authenticated;
        }
    }
}