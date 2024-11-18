using BreakPoint2._0;
using System.Windows;
using System.Windows.Controls;

namespace BreakPoint2._0
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Завантажуємо стартову сторінку
            MainFrame.Navigate(new StartPage(MainFrame));
        }
    }
}
