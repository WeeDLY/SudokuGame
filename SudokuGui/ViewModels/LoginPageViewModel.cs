using Library.Log;
using Library.Model;
using Library.Utility;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace SudokuGui.ViewModels
{
    /// <summary>
    /// ViewModel for LoginPage
    /// </summary>
    /// <seealso cref="Template10.Mvvm.ViewModelBase" />
    public class LoginPageViewModel : ViewModelBase
    {
        #region Properties/Fields

        /// <summary>
        /// The random
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// The database
        /// </summary>
        private DatabaseClient Database = new DatabaseClient(20);

        /// <summary>
        /// The username
        /// </summary>
        private string _Username = String.Empty;
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get => _Username; set => _Username = value; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { private get; set; } = "";

        /// <summary>
        /// The show progress ring
        /// </summary>
        private bool _ShowProgressRing;
        /// <summary>
        /// Gets or sets a value indicating whether [show progress ring].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show progress ring]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowProgressRing
        {
            get => _ShowProgressRing;
            set
            {
                Set(ref _ShowProgressRing, value);
                if (value)
                {
                    Timer_Tick(null, null);
                    timer.Start();
                }
                else
                {
                    WaitingTextProp = "";
                }
            }
        }

        /// <summary>
        /// The timer for Waiting messages, when trying to login.
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };

        /// <summary>
        /// The waiting text
        /// </summary>
        private string _WaitingTextProp;
        /// <summary>
        /// Gets or sets the waiting text.
        /// </summary>
        /// <value>
        /// The waiting text.
        /// </value>
        public string WaitingTextProp
        {
            get => _WaitingTextProp;
            set => Set(ref _WaitingTextProp, value);
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPageViewModel" /> class.
        /// </summary>
        public LoginPageViewModel()
        {
            timer.Tick += Timer_Tick;
            Views.Shell.HamburgerMenu.IsFullScreen = true;

            // Set up logger
            Logger.SetUpLogger();
        }

        /// <summary>
        /// Timer that shows Waiting text, when the user is waiting to be authenticated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Timer_Tick(object sender, object e)
        {
            WaitingTextProp = WaitingText.LoginText[random.Next(0, WaitingText.LoginText.Length)];
            if (!ShowProgressRing)
            {
                WaitingTextProp = "";
                timer.Stop();
            }
        }

        /// <summary>
        /// BtnLogin: Logs in user async
        /// </summary>
        public async void ButtonLogOnAsync()
        {
            if (ShowProgressRing)
                return;
            ShowProgressRing = true;

            string username = Username;
            string password = Password;

            if(username.Length <= 0 || password.Length <= 0)
            {
                ShowProgressRing = false;
                UserDialog.ShowMessageDialogAsync("Form error", "Please fill out username/password fields");
                return;
            }

            bool internetConnection = NetworkInterface.GetIsNetworkAvailable();
            if (internetConnection)
            {
                Session session = await Database.AuthenticateUserAsync(username, password);
                ShowProgressRing = false;

                if(session == null)
                {
                    UserDialog.ShowMessageDialogAsync("Login error", "Something went wrong, please retry");
                    return;
                }

                await LoginHandler(session);
            }
            // No internet connection, continue in offline mode.
            else
            {
                ShowProgressRing = false;
                GoToMainPage(new Session(-1, username));
            }
        }

        /// <summary>
        /// Logins the handler.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        private async Task LoginHandler(Session session)
        {
            switch (session.Authenticated)
            {
                // User exists, may have wrong password
                case true:
                    if (session.UserId == -1)
                    {
                        UserDialog.ShowMessageDialogAsync("Login failed", "Username/password was wrong");
                        await Logger.LogAsync(LogLevel.Info, "Login failed on: " + session.Username);
                    }
                    else
                    {
                        GoToMainPage(session);
                    }
                    break;
                // User does not exist
                case false:
                    UserDialogResponse respNoUser = await UserDialog.ShowMessageDialogOptionsAsync($"Login error", $"Username {session.Username} does not exist, do you want to create an account?");
                    if (respNoUser == UserDialogResponse.Yes)
                    {
                        GoToRegister(session.Username);
                    }
                    break;
                // User have internet, can't connect to database
                case null:
                default:
                    UserDialogResponse respServerError = await UserDialog.ShowMessageDialogOptionsAsync("Database error", "Could not contact the Database. Please retry later, or press yes to login in offline mode.");
                    if (respServerError == UserDialogResponse.Yes)
                    {
                        GoToMainPage(new Session(-1, session.Username));
                    }
                    break;
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// BtnRegister: Sends user to RegisterPage
        /// </summary>
        public void ButtonRegister()
        {
            GoToRegister(Username);
        }

        /// <summary>
        /// Texts the password key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
        public void TextPasswordKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                ButtonLogOnAsync();
            }
        }

        /// <summary>
        /// Goes to register.
        /// </summary>
        public void GoToRegister(string username) => NavigationService.Navigate(typeof(Views.RegisterPage), username);

        /// <summary>
        /// Goes to main page.
        /// </summary>
        /// <param name="session">The session.</param>
        public void GoToMainPage(Session session)
        {
            ShowProgressRing = false;
            NavigationService.Navigate(typeof(Views.MainPage), session);
        }
    }
}