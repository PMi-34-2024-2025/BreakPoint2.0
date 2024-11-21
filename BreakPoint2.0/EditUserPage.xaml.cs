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
                    PasswordTextBox.Password = new string('•', 8);
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
            string password = PasswordTextBox.Password;

            if (userName == _currentUser.UserName &&
                email == _currentUser.Email &&
                password == new string('•', 8))
            {
                MessageBox.Show("To make changes, modify something first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_userService.UpdateUser(CreateAcc.CurrentUserId, userName, email, password)) 
            {
                MessageBox.Show("Changes applied successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
