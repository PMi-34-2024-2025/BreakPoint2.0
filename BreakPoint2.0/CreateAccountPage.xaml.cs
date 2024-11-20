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
            var createAcc = new CreateAcc();

            string nickname = NicknameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                bool isRegistered = createAcc.Register(nickname, email, password);

                if (isRegistered)
                {
                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    var loginPage = new LoginPage();
                    this.NavigationService.Navigate(loginPage);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new LoginPage());
        }

    }
}