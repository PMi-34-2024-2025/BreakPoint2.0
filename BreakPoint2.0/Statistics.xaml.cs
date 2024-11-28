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
        private int UserId;

        public Statistics()
        {
            InitializeComponent();
            LoadGameNames();
            UserId = CreateAcc.CurrentUserId;
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
            double totalTime = statistics.GetTotalTimeForGame(selectedGame, datesList);
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalTime);
            // Оновлюємо мітку з загальним часом
            // Форматування часу у вигляді "години:хвилини:секунди"
            TotalTimeLabel.Content = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

            // Отримуємо тривалості сесій, згруповані по датах
            var durationsByDate = statistics.GetDurationsGroupedByDate(selectedGame, UserId, datesList);

            var histogramData = new Dictionary<int, double>();

            foreach (var entry in durationsByDate)
            {
                // Додаємо день місяця як ключ і тривалість як значення
                histogramData[entry.Key.Day] = entry.Value;
            }

            // Викликаємо метод для побудови гістограми
            DrawHistogram(histogramData, ChartCanvas);


        }

        private void DrawHistogram(Dictionary<int, double> data, Canvas chartCanvas)
        {
            chartCanvas.Children.Clear(); // Очистити Canvas перед малюванням

            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;

            // Отримуємо рік і місяць із вибраної дати в календарі
            DateTime selectedDate = MyCalendar.SelectedDate ?? DateTime.Now; // Використовуємо поточну дату, якщо нічого не вибрано
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            double columnWidth = canvasWidth / daysInMonth * 0.8; // Ширина стовпчика з відступом
            double maxDataValue = data.Values.Count > 0 ? data.Values.Max() : 1;

            // Відстань між позначками
            double stepX = canvasWidth / daysInMonth;

            // Створення підказки (Tooltip) для відображення часу
            TextBlock tooltip = new TextBlock
            {
                Background = Brushes.LightYellow,
                Padding = new Thickness(5),
                Visibility = Visibility.Collapsed
            };
            chartCanvas.Children.Add(tooltip);

            // Сьогоднішній день
            DateTime today = DateTime.Now;

            // Малювання стовпчиків для кожного дня вибраного місяця
            for (int day = 1; day <= daysInMonth; day++)
            {
                // Висота стовпчика (якщо даних немає — 0)
                double value = data.ContainsKey(day) ? data[day] : 0;
                double columnHeight = (value / maxDataValue) * (canvasHeight * 0.8);

                // Колір для сьогоднішнього дня та інших
                Brush columnColor = Brushes.Blue;
                if (day == today.Day && selectedDate.Month == today.Month && selectedDate.Year == today.Year)
                {
                    columnColor = Brushes.Green; // Інший колір для сьогоднішнього дня
                }
                else if (day == selectedDate.Day)
                {
                    columnColor = Brushes.Red; // Виділення вибраного дня
                }

                // Малюємо стовпчик
                Rectangle column = new Rectangle
                {
                    Width = columnWidth,
                    Height = columnHeight,
                    Fill = columnColor,
                };

                // Позиція стовпчика
                Canvas.SetLeft(column, stepX * (day - 1) + stepX * 0.1); // Відступ зліва
                Canvas.SetBottom(column, 20); // Відступ знизу
                chartCanvas.Children.Add(column);

                // Додаємо текст під позначками
                TextBlock dayLabel = new TextBlock
                {
                    Text = day.ToString(),
                    Foreground = Brushes.Black,
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center
                };

                Canvas.SetLeft(dayLabel, stepX * (day - 1) + stepX * 0.2); // Рівняння по центру стовпчика
                Canvas.SetTop(dayLabel, canvasHeight - 10); // Позиція підпису
                chartCanvas.Children.Add(dayLabel);

                // Додаємо подію для наведення миші
                column.MouseEnter += (sender, args) =>
                {
                    // Показати Tooltip з часом, проведеним у програмі в цей день
                    tooltip.Visibility = Visibility.Visible;

                    // Перетворюємо час у формат годин:хвилин:секунд
                    TimeSpan timeSpan = TimeSpan.FromSeconds(value);
                    tooltip.Text = $"Час: {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                    Canvas.SetLeft(tooltip, Canvas.GetLeft(column) + columnWidth / 2 - tooltip.ActualWidth / 2);
                    Canvas.SetTop(tooltip, Canvas.GetBottom(column) - 30); // Розміщення над стовпчиком
                };

                // Додаємо подію для виходу миші
                column.MouseLeave += (sender, args) =>
                {
                    // При виході з області стовпчика приховати Tooltip
                    tooltip.Visibility = Visibility.Collapsed;
                };
            }

            // Додаємо вертикальну вісь
            Line yAxis = new Line
            {
                X1 = 10,
                Y1 = 10,
                X2 = 10,
                Y2 = canvasHeight - 20,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            chartCanvas.Children.Add(yAxis);

            // Додаємо горизонтальну вісь
            Line xAxis = new Line
            {
                X1 = 10,
                Y1 = canvasHeight - 20,
                X2 = canvasWidth - 10,
                Y2 = canvasHeight - 20,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            chartCanvas.Children.Add(xAxis);
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
