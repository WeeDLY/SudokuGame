using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SudokuGui.Views
{
    /// <summary>
    /// StatisticPage Code Behind
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.Page" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class StatisticPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticPage"/> class.
        /// </summary>
        public StatisticPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Disabled;
        }
    }
}