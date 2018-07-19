using SudokuGui.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

namespace SudokuGui.Views
{
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
            Views.Shell.HamburgerMenu.IsFullScreen = true;
        }

        private void PasswordBox_PasswordChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.DataContext != null)
                ((dynamic)this.DataContext).Password = ((PasswordBox)sender).Password;
        }
        private void VerifyPasswordBox_PasswordChanged(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.DataContext != null)
                ((dynamic)this.DataContext).VerifyPassword = ((PasswordBox)sender).Password;
        }
    }
}