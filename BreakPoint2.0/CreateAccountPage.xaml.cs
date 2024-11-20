using System;
using System.Windows;
using System.Windows.Controls;
using BLL;

namespace BreakPoint2._0
{
    public partial class CreateAccountPage : Page
    {
        public CreateAccountPage()
        {
            InitializeComponent();
        }

        private void OnSignUpButtonClick(object sender, RoutedEventArgs e)
        {
            // Ініціалізація об'єкта CreateAcc
            var createAcc = new CreateAcc();

            // Зчитування введених користувачем даних
            string nickname = NicknameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                // Виклик методу реєстрації
                bool isRegistered = createAcc.Register(nickname, email, password);

                if (isRegistered)
                {
                    // Відображення повідомлення про успіх
                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ArgumentException ex)
            {
                // Помилка валідації
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Інші можливі помилки
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
