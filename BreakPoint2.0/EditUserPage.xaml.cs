using System;
using System.Windows;
using BLL; // Простір імен для вашої логіки бізнесу (BLL)

namespace BreakPoint2._0
{
    public partial class ProfilePage : Window
    {
        public ProfilePage()
        {
            InitializeComponent();
           
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string userName = UserNameTextBox.Text;
            string email = EmailTextBox.Text;
            string newPassword = PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show("Please fill in all fields before updating the password.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                UserService userService = new UserService();
                bool isUpdated = userService.UpdatePassword(email, newPassword);

                if (isUpdated)
                {
                    MessageBox.Show("Password updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to update the password. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
