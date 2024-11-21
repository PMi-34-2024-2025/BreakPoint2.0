using BLL;
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
        private Tracking _tracking;
        private UserService _userService;
        public MainPage()
        {
            InitializeComponent();
            _tracking = new Tracking();
            _userService = new UserService();
        }
        private void GetStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            // Показуємо панель зі статистикою та приховуємо кнопку "Отримати статистику"
            StatisticsPanel.Visibility = Visibility.Visible;
            GetStatisticsButton.Visibility = Visibility.Collapsed;

        }
        private void ShowAllGamesTimeButton_Click(object sender, RoutedEventArgs e)
        {
            
            //отримання юзера
            int userId = 1;


            var gamesTime = _tracking.GetAllGamesTimeForUser(userId); //  метод для отримання часу ігор
            GamesListBox.Items.Clear(); // Очищаємо список перед заповненням новими даними

            foreach (var game in gamesTime)
            {
                GamesListBox.Items.Add(game); // Додаємо кожну гру в список
            }

            StatisticsPanel.Visibility = Visibility.Visible; // Показуємо панель статистики
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
