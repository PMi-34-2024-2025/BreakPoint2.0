using BLL; 
using System;
using System.Windows;
using System.Windows.Controls;

namespace BreakPoint2._0
{
    public partial class Friends : Page
    {
        private FriendShip _friendShip;  // Додаємо змінну для FriendShip
        List<int> listFriend;
        public Friends()
        {
            InitializeComponent();
            _friendShip = new FriendShip();  
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }

        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            string friendName = UserNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(friendName))
            {
                // Можна додати повідомлення для користувача
                MessageBox.Show("Friend's nickname cannot be empty.");
            }

            // Викликаємо метод AddFriendToMe через екземпляр _friendShip
            _friendShip.AddFriendToMe(friendName);
        }

        private void ViewFriendsButton_Click(object sender, RoutedEventArgs e)
        {
            listFriend = _friendShip.GetFriends();
            FriendsListBox.Items.Clear(); 

            if (listFriend.Count == 0)
            {
                MessageBox.Show("You have no friends.");
                return;
            }

            foreach (var friendId in listFriend)
            {
                FriendsListBox.Items.Add($"Friend ID: {friendId}"); 
            }
        }
    }
}
