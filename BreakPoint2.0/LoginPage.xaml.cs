using System;
using System.Windows;
using System.Windows.Controls;
using BLL; // Простір імен для вашої логіки бізнесу (BLL)

namespace BreakPoint2._0
{
    public partial class LoginPage : Page
    {
        private CreateAcc _accountManager;

        public LoginPage()
        {
            InitializeComponent();
            _accountManager = new CreateAcc(); // Ініціалізація менеджера акаунтів
        }

        // Обробник події для кнопки "Log In"
        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                // Викликаємо метод для входу в акаунт
                bool isLoggedIn = _accountManager.Login(username, password);

                if (isLoggedIn)
                {
                    int userId = _accountManager.CurrentUserId; 
                    MessageBox.Show($"Login successful! Your ID: {userId}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Перехід на головну сторінку або іншу сторінку після успішного входу
                    NavigationService.Navigate(new MainPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обробник події для кнопки "Create Account"
        private void OnCreateAccountButtonClick(object sender, RoutedEventArgs e)
        {
            // Навігація до сторінки реєстрації
            NavigationService.Navigate(new CreateAccountPage());
        }
    }
}
