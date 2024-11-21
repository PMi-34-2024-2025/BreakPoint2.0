using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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

            // Перетворюємо SelectedDatesCollection на List<DateTime>
            List<DateTime> datesList = new List<DateTime>(selectedDates.Cast<DateTime>());

            // Отримуємо загальний час для вибраної гри за обрані дати
            double totalTime = statistics.GetTotalTimeForGame(selectedGame, datesList);

            // Оновлюємо мітку з загальним часом
            TotalTimeLabel.Content = $"{totalTime:F2} секунд";
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
