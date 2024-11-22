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
        private CreateAcc _accountManager;
        int currentUserId = CreateAcc.CurrentUserId;
        public MainPage()
        {
            InitializeComponent();
            
            try
            {
            
                // Отримати дані користувача
                var (userName, userEmail) = new MainPageHelper().GetUserDetailsById(currentUserId);

                // Встановити значення у TextBlock
                UserNameTextBlock.Text = userName;
                AccountEmailTextBlock.Text = userEmail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }


        }
        private void GetStatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            // Показуємо панель зі статистикою та приховуємо кнопку "Отримати статистику"
            StatisticsPanel.Visibility = Visibility.Visible;
            GetStatisticsButton.Visibility = Visibility.Collapsed;


        }
        private void ShowAllGamesTimeButton_Click(object sender, RoutedEventArgs e)
        {
       
            var mainPageHelper = new MainPageHelper();
            try
            {
                // Отримуємо статистику для поточного користувача
                var userGamesTime = mainPageHelper.GetAllGamesTimeForUser();

                // Очищаємо попередні дані в ListBox
                GamesListBox.Items.Clear();
                double totalSeconds = 0;
                // Додаємо нові елементи в ListBox
                foreach (var game in userGamesTime)
                {
                    GamesListBox.Items.Add(game);
                    totalSeconds += game.SessionDuration * 60; //  хвилини в секунди

                }
                // Розрахунок загального часу
                int totalHours = (int)(totalSeconds / 3600);
                int remainingSecondsAfterHours = (int)(totalSeconds % 3600);
                int totalMinutes = remainingSecondsAfterHours / 60;
                int totalRemainingSeconds = remainingSecondsAfterHours % 60;
                
                TotalTimeTextBlock.Text = $"{totalHours}h {totalMinutes}m {totalRemainingSeconds}s";

                
                StatisticsPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                // Обробка помилок
                MessageBox.Show($"Помилка при отриманні статистики: {ex.Message}");
            }
            

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
