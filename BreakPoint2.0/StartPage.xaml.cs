using System.Windows;
using System.Windows.Controls;

namespace BreakPoint2._0
{
    public partial class StartPage : Page
    {
        private Frame _mainFrame;

        public StartPage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            // Переходимо на сторінку логування
            _mainFrame.Navigate(new LoginPage());
        }
    }
}
