using Library.Model;
using Library.Utility;
using System;
using System.Threading.Tasks;
using Template10.Mvvm;


namespace SudokuGui.ViewModels
{
    /// <summary>
    /// ViewModel for SettingsPage
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class SettingsPageViewModel : ViewModelBase
    {
        #region Properties/Fields

        public SettingsPartViewModel SettingsPartViewModel { get; } = new SettingsPartViewModel();

        /// <summary>
        /// The database
        /// </summary>
        private DatabaseClient Database = new DatabaseClient();

        /// <summary>
        /// The username
        /// </summary>
        private string _UserName = String.Empty;
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get => _UserName; set => _UserName = value; }

        /// <summary>
        /// The new username
        /// </summary>
        private string _NewUsername = String.Empty;
        /// <summary>
        /// Gets or sets the new username.
        /// </summary>
        /// <value>
        /// The new username.
        /// </value>
        public string NewUsername { get => _NewUsername; set => _NewUsername = value; }

        /// <summary>
        /// The password
        /// </summary>
        private string _Password = String.Empty;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get => _Password; set => _Password = value; }

        #endregion


        /// <summary>
        /// BTNs the delete user asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task ButtonDeleteUserAsync()
        {
            // Checking user input
            if (!ValidInput(false))
            {
                UserDialog.ShowMessageDialogAsync("Form error", "Please fill out username and password");
                return;
            }

            Views.Busy.SetBusy(true, "Deleting user: " + Username);

            bool deletedUser = await DeleteUserAsync(Username, Password);
            Views.Busy.SetBusy(false);

            if (deletedUser)
            {
                UserDialog.ShowMessageDialogAsync("Deleted user", $"User {Username} was deleted successfully");
                GotoLoginPage();
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Deletes the user asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<bool> DeleteUserAsync(string username, string password)
        {
            Session session = await Database.AuthenticateUserAsync(username, password);
            if (session.UserId != -1 && session.Authenticated == true)
            {
                // Delete puzzles by user
                bool deletedPuzzles = await Database.DeletePuzzlesAsync(session.UserId);
                if (deletedPuzzles)
                {
                    // Delete user, only if all puzzles was deleted
                    bool deletedUser = await Database.DeleteUserAsync(session.UserId);
                    if (deletedUser)
                        return true;
                    else
                        return false;
                }
            }

            UserDialog.ShowMessageDialogAsync("Delete user error", $"Unable to delete user {username}. Please check your credentials and your network connection");
            return false;
        }

        /// <summary>
        /// BTNs the change username asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task ButtonChangeUsernameAsync()
        {
            // Checking user input
            if (!ValidInput(true))
            {
                UserDialog.ShowMessageDialogAsync("Form error", "Please fill out username, password and new password");
                return;
            }

            Views.Busy.SetBusy(true, $"Changing username from: {Username} to: {NewUsername}");

            bool changedUsername = await ChangeUsernameAsync(Username, Password, NewUsername);
            Views.Busy.SetBusy(false);

            if (changedUsername)
            {
                UserDialog.ShowMessageDialogAsync("Username changed", $"Username changed from {Username} to {NewUsername}");
                GotoLoginPage();
            }
        }

        /// <summary>
        /// Changes the username asynchronous.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="newUsername">The new username.</param>
        /// <returns></returns>
        public async Task<bool> ChangeUsernameAsync(string username, string password, string newUsername)
        {
            Session session = await Database.AuthenticateUserAsync(username, password);

            // Session is authorized, can change username
            if(session.Authenticated == true && session.UserId != -1)
            {
                int newUserId = await Database.GetUserIdAsync(newUsername);
                if(newUserId != -1)
                {
                    UserDialog.ShowMessageDialogAsync("Change username", $"Username {newUsername} is already taken");
                    return false;
                }
                return await Database.PutChangeUsernameAsync(session, newUsername);
            }
            return false;
        }

        /// <summary>
        /// Valids the input.
        /// </summary>
        /// <param name="includeNewUsername">if set to <c>true</c> [include new username].</param>
        /// <returns></returns>
        private bool ValidInput(bool includeNewUsername)
        {
            if (Username.Length <= 0 && Password.Length <= 0)
                return false;

            if (includeNewUsername)
                return NewUsername.Length > 0;
            else
                return true;
        }

        /// <summary>
        /// Gotoes the login page.
        /// </summary>
        public void GotoLoginPage() => NavigationService.Navigate(typeof(Views.LoginPage));
    }

    /// <summary>
    /// Settings, that are default and with template10
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class SettingsPartViewModel : ViewModelBase
    {
        /// <summary>
        /// The settings
        /// </summary>
        Services.SettingsServices.SettingsService _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPartViewModel"/> class.
        /// </summary>
        public SettingsPartViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                // designtime
            }
            else
            {
                _settings = Services.SettingsServices.SettingsService.Instance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show hamburger button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show hamburger button]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHamburgerButton
        {
            get { return _settings.ShowHamburgerButton; }
            set { _settings.ShowHamburgerButton = value; base.RaisePropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is full screen.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is full screen; otherwise, <c>false</c>.
        /// </value>
        public bool IsFullScreen
        {
            get { return _settings.IsFullScreen; }
            set
            {
                _settings.IsFullScreen = value;
                base.RaisePropertyChanged();
                if (value)
                {
                    ShowHamburgerButton = false;
                }
                else
                {
                    ShowHamburgerButton = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use shell back button].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use shell back button]; otherwise, <c>false</c>.
        /// </value>
        public bool UseShellBackButton
        {
            get { return _settings.UseShellBackButton; }
            set { _settings.UseShellBackButton = value; base.RaisePropertyChanged(); }
        }
    }
}