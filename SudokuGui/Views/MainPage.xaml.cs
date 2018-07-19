using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SudokuGui.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t.IsReadOnly)
                return;
            string c = e.OriginalKey.ToString().Replace("Number", "");
            if(int.TryParse(c, out int num))
            {
                // This removes, weird cases like æ,ø,å, etc
                if(num.ToString().Length <= 1)
                    t.Text = num.ToString();
            }
            e.Handled = true;
        }
    }
}