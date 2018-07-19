using Library.Model;
using Library.Utility;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;

namespace SudokuGui.ViewModels
{
    /// <summary>
    /// RegisterPage's ViewModel
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class RegisterPageViewModel : ViewModelBase
    {
        #region Properties/Fields

        /// <summary>
        /// The database
        /// </summary>
        private DatabaseClient Database = new DatabaseClient();

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
        /// The password
        /// </summary>
        private string _Password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password
        {
            get => _Password;
            set => _Password = value;
        }

        /// <summary>
        /// The verify password
        /// </summary>
        private string _VerifyPassword;
        /// <summary>
        /// Gets or sets the verify password.
        /// </summary>
        /// <value>
        /// The verify password.
        /// </value>
        public string VerifyPassword
        {
            get => _VerifyPassword;
            set => _VerifyPassword = value;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterPageViewModel"/> class.
        /// </summary>
        public RegisterPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if(parameter != null)
            {
                Username = parameter as string;
            }
            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        /// <summary>
        /// Button: Register new account
        /// </summary>
        public async void ButtonRegisterAsync()
        {
            // Check network connection
            bool internetConnection = NetworkInterface.GetIsNetworkAvailable();
            if(!internetConnection)
            {
                UserDialog.ShowMessageDialogAsync("Network connection", "You have no internet connection, logging you on in offline mode");
                GoToMainPage(new Session(-1, Username));
                return;
            }

            // Check if user exists
            bool? exists = await Database.GetRegisterUserExistsAsync(Username);
            if (exists == true)
            {
                UserDialog.ShowMessageDialogAsync("Username taken", $"{Username} is already taken");
                return;
            }
            else if(exists == null)
            {
                UserDialog.ShowMessageDialogAsync("Server error", "No contact with server, please try again later.");
                return;
            }

            // Check if password matches
            if (Password == VerifyPassword)
            {
                CreateUserAsync(Username, Password);
            }
            else
            {
                UserDialog.ShowMessageDialogAsync("Password error", "Passwords do not match");
            }
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        private async void CreateUserAsync(string username, string password)
        {
            string salt = Hashing.GenerateSalt();
            string hashedPassword = Hashing.GetSha256Hash(password, salt);

            RegisterUser r = new RegisterUser(username, hashedPassword, salt);
            bool registerUser = await Database.RegisterUserAsync(r);

            if (!registerUser)
            {
                UserDialog.ShowMessageDialogAsync("Register user", $"Could not register user {username}");
                return;
            }

            // Get user ID and authenticate
            int userId = await Database.GetUserIdAsync(username);
            if (userId != -1)
            {
                GoToMainPage(new Session(userId, username, true));
            }
            else
            {
                UserDialog.ShowMessageDialogAsync("Authenticate user", $"Could not authenticate {username}");
            }
        }

        /// <summary>
        /// Goes to main page.
        /// </summary>
        /// <param name="session">The session.</param>
        private void GoToMainPage(Session session)
        {
            NavigationService.Navigate(typeof(Views.MainPage), session);
        }

        /// <summary>
        /// Goes to login page.
        /// </summary>
        public void GoToLoginPage() => NavigationService.Navigate(typeof(Views.LoginPage), 0);
    }
}