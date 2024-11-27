using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BLL;

namespace BreakPoint2._0
{
    public partial class Statistics : Page
    {
        BLL.Statistics statistics = new BLL.Statistics();

        public Statistics()
        {
            InitializeComponent();
            LoadGameNames();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedGame = MyComboBox.SelectedItem as string;

            // Перевірка, чи вибрано значення
            if (string.IsNullOrEmpty(selectedGame))
            {
                MessageBox.Show("Будь ласка, виберіть гру.");
                return;
            }

            // Отримуємо вибрані дати з календаря
            var selectedDates = MyCalendar.SelectedDates;

            if (selectedDates == null || selectedDates.Count == 0)
            {
                MessageBox.Show("Будь ласка, виберіть хоча б одну дату.");
                return;
            }

            DateTime selectedDate = MyCalendar.SelectedDate.Value;
            int year = selectedDate.Year;
            int month = selectedDate.Month;

            // Перетворюємо SelectedDatesCollection на List<DateTime>
            List<DateTime> datesList = new List<DateTime>(selectedDates.Cast<DateTime>());

            // Отримуємо загальний час для вибраної гри за обрані дати
            //double totalTime = statistics.GetTotalTimeForGame(selectedGame, datesList);

            // Оновлюємо мітку з загальним часом
            TotalTimeLabel.Content = $"{totalTime:F2} seconds";
            // Отримуємо дані про активність протягом вибраного місяця
            var statistics = statistics.GetGameStatisticsForMonth(selectedGame, year, month);

            ///Dictionary<int, double> dailyStatistics = statistics.GetGameStatisticsForMonth(selectedGame, year, month);

            // Оновлюємо графік
            //DrawBarChart(dailyStatistics, DateTime.Now.Day);
        }

        private void DrawBarChart(Dictionary<int, double> dailyData, int currentDay)
        {
            ChartCanvas.Children.Clear(); // Очищуємо попередній графік

            double canvasWidth = ChartCanvas.ActualWidth;
            double canvasHeight = ChartCanvas.ActualHeight;

            if (dailyData.Count == 0)
            {
                MessageBox.Show("Немає даних для відображення.");
                return;
            }

            double maxTime = dailyData.Values.Max();
            double barWidth = canvasWidth / dailyData.Count;

            int day = 1;
            foreach (var entry in dailyData)
            {
                double barHeight = (entry.Value / maxTime) * (canvasHeight - 20); // Нормалізуємо висоту

                // Колір для поточного дня
                Brush barColor = entry.Key == currentDay ? Brushes.Red : Brushes.Blue;

                // Додаємо прямокутник
                Rectangle bar = new Rectangle
                {
                    Width = barWidth - 5,
                    Height = barHeight,
                    Fill = barColor
                };

                Canvas.SetLeft(bar, (day - 1) * barWidth + 5);
                Canvas.SetBottom(bar, 0);
                ChartCanvas.Children.Add(bar);

                // Додаємо підпис
                TextBlock label = new TextBlock
                {
                    Text = entry.Key.ToString(),
                    FontSize = 12,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Canvas.SetLeft(label, (day - 1) * barWidth + barWidth / 4);
                Canvas.SetBottom(label, -20);
                ChartCanvas.Children.Add(label);

                day++;
            }
        }



        private void BackToButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        
        private void LoadGameNames()
        {
            List<string> gameNames = statistics.GetGameNames();

            foreach (var game in gameNames)
            {
                Console.WriteLine(game); // Для перевірки роботи, можна потім видалити
            }

            MyComboBox.ItemsSource = gameNames; // Заповнюємо ComboBox іграми
        }
    }
}
