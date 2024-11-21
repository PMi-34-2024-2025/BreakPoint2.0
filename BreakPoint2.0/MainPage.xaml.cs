using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BreakPoint2._0
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            
        }
        private void GetStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            // Показуємо панель зі статистикою та приховуємо кнопку "Отримати статистику"
            StatisticsPanel.Visibility = Visibility.Visible;
            GetStatisticsButton.Visibility = Visibility.Collapsed;

        }
        private void ShowAllGamesTimeButton_Click(object sender, RoutedEventArgs e)
        {
            
           
        }
        private void OnStatisticButtonClick(object sender, RoutedEventArgs e)
        {
            // Перехід на сторінку статистики
            NavigationService.Navigate(new Statistics());
        }

       
        private void OnFriendButtonClick(object sender, RoutedEventArgs e)
        {
            // Перехід на сторінку друзів
            NavigationService.Navigate(new Friends());
        }
        private void OnEditUserPageClick(object sender, RoutedEventArgs e)
        {
            // Перехід на сторінку акаунут
            NavigationService.Navigate(new ProfilePage());
        }

        // доробити log out  
        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
           // вихід з акаунту

        }

        private void OnStartStretchingClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TrackingApps());
        }
    }
}
