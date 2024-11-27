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
                    totalSeconds += game.SessionDuration * 60; //  хвилини в секунди
                    game.StringSessionDuration = MainPageHelper.FormatDuration(game.SessionDuration);
                    //GamesListBox.Items.Add($"{game.ApplicationName}: {MainPageHelper.FormatDuration(game.SessionDuration)}");
                    GamesListBox.Items.Add(game);
                   

                }
                // Розрахунок загального часу
                int totalHours = (int)(totalSeconds / 3600);
                int remainingSecondsAfterHours = (int)(totalSeconds % 3600);
                int totalMinutes = remainingSecondsAfterHours / 60;
                int totalRemainingSeconds = remainingSecondsAfterHours % 60;
                
                TotalTimeTextBlock.Text = $"{totalHours}h {totalMinutes}m {totalRemainingSeconds}s";
                // Перетворюємо ObservableCollection на List
                List<GameStatistic> gameStatisticsList = userGamesTime.ToList();

                DrawPieChart(gameStatisticsList, totalSeconds);

                //StatisticsPanel.Visibility = Visibility.Visible;
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

        // Функція для малювання кругової діаграми на Canvas
        
        private void DrawPieChart(List<GameStatistic> gameStatistics, double totalSeconds)
        {
            GraphCanvas.Children.Clear(); // Очищуємо попередню графіку

            // Центр і радіус для діаграми
            double centerX = 100; // Відносно центру Canvas
            double centerY = 100;
            double radius = 75; // Радіус кругової діаграми

            double startAngle = 0; // Початковий кут

            // Попередньо визначені кольори (без повторів)
            List<Brush> colors = new List<Brush>
    {
        Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow,
        Brushes.Orange, Brushes.Purple, Brushes.Cyan, Brushes.Magenta,
        Brushes.Brown, Brushes.Pink, Brushes.LightBlue, Brushes.LightGreen
    };
            int colorIndex = 0;

            double legendX = 220; // Координати для розміщення легенди (збоку Canvas)
            double legendY = 10;

            foreach (var game in gameStatistics)
            {
                // Розрахунок відсотків для сектора
                double gamePercentage = (game.SessionDuration * 60) / totalSeconds;
                double sweepAngle = gamePercentage * 360;

                // Використовуємо кольори з попередньо визначеного списку
                Brush segmentColor = colors[colorIndex % colors.Count];
                colorIndex++;

                // Малювання сектора
                PathFigure pathFigure = new PathFigure
                {
                    StartPoint = new Point(centerX, centerY),
                    IsClosed = true
                };

                // Початкова точка сектора
                double startRad = (Math.PI / 180) * startAngle;
                Point startPoint = new Point(
                    centerX + radius * Math.Cos(startRad),
                    centerY + radius * Math.Sin(startRad)
                );

                pathFigure.Segments.Add(new LineSegment(startPoint, true));

                // Кінцева точка сектора
                double endAngle = startAngle + sweepAngle;
                double endRad = (Math.PI / 180) * endAngle;
                Point endPoint = new Point(
                    centerX + radius * Math.Cos(endRad),
                    centerY + radius * Math.Sin(endRad)
                );

                pathFigure.Segments.Add(new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = sweepAngle > 180
                });

                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(pathFigure);

                Path path = new Path
                {
                    Fill = segmentColor,
                    Data = geometry
                };

                // Додавання на Canvas
                GraphCanvas.Children.Add(path);

                // Додавання легенди збоку
                Rectangle legendColorBox = new Rectangle
                {
                    Width = 20,
                    Height = 20,
                    Fill = segmentColor,
                    Margin = new Thickness(legendX, legendY, 0, 0)
                };

                TextBlock legendText = new TextBlock
                {
                    Text = game.ApplicationName,
                    Foreground = Brushes.White,
                    FontSize = 12,
                    Margin = new Thickness(legendX + 25, legendY + 3, 0, 0)
                };

                // Додавання на Canvas
                GraphCanvas.Children.Add(legendColorBox);
                GraphCanvas.Children.Add(legendText);

                // Оновлюємо координату Y для наступного запису легенди
                legendY += 30;

                // Оновлюємо початковий кут
                startAngle += sweepAngle;
            }
        }

    }
}
