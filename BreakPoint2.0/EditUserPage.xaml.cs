using System;
using System.Windows;
using System.Windows.Controls;
using BLL;

namespace BreakPoint2._0
{
    public partial class ProfilePage : Page
    {
        private readonly UserService _userService;
        private User _currentUser;

        public ProfilePage()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                int currentUserId = CreateAcc.CurrentUserId;
                _currentUser = _userService.GetUserById(currentUserId);

                if (_currentUser != null)
                {
                    UserNameTextBox.Text = _currentUser.UserName;
                    EmailTextBox.Text = _currentUser.Email;
                    PasswordBox.Password = new string('●', _currentUser.PasswordHash.Length); 
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading user data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyChangesButton_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("User name and email cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentUser.UserName == userName && _currentUser.Email == email && password == new string('●', _currentUser.PasswordHash.Length))
            {
                MessageBox.Show("To make changes, modify something first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string updatedPassword = password == new string('●', _currentUser.PasswordHash.Length)
                ? _currentUser.PasswordHash
                : password;

            if (_userService.UpdateUser(CreateAcc.CurrentUserId, userName, email, updatedPassword))
            {
                MessageBox.Show("Changes applied successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadUserData();
            }
            else
            {
                MessageBox.Show("Failed to apply changes.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToMainPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainPage());
        }
    }
}
