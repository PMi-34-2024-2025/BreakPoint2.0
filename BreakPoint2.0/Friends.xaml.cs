using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLL;

namespace BreakPoint2._0
{
    public partial class Friends : Page
    {
        private FriendShip _friendShip;
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
                MessageBox.Show("Friend's nickname cannot be empty.");
            }

            _friendShip.AddFriendToMe(friendName);
        }

        private bool isFriendsListVisible = false;

        private void ViewFriendsButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFriendsListVisible)
            {
                FriendsListBox.Items.Clear();
            }
            else
            {
                var friendsSessions = _friendShip.GetFriends();

                if (friendsSessions.Count == 0)
                {
                    MessageBox.Show("You have no friends, LOSER.");
                    return;
                }
                //FriendsListBox.Width = 220; 
                //FriendsListBox.Height = 250; 

                foreach (var friendSession in friendsSessions)
                {
                    var displayText = $"Friend Name: {friendSession.FriendName}\n" +
                                      $"Game: {friendSession.GameName}\n" +
                                      $"Session Start: {friendSession.Start}\n" +
                                      $"Session End: {friendSession.End}\n";

                    var friendItem = new StackPanel { Orientation = Orientation.Vertical };

                    friendItem.Children.Add(new TextBlock { Text = displayText });

                    var removeButton = new Button { Content = "Remove Friend" };

                    removeButton.Click += (s, args) =>
                    {
                        _friendShip.RemoveFriend(friendSession.FriendId);
                        ViewFriendsButton_Click(sender, e);
                    };

                    friendItem.Children.Add(removeButton);

                    FriendsListBox.Items.Add(friendItem);
                }
            }

            isFriendsListVisible = !isFriendsListVisible;
        }
    }
}
