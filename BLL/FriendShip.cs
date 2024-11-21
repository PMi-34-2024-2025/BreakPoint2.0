using Npgsql;
using System;

namespace BLL
{
    public class FriendShip
    {
        private const string ConnectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";

        // Виправлений метод додавання друга
        public void AddFriendToMe(string NameFriend)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                int friendId;
                using (var command = new NpgsqlCommand("SELECT \"UserId\" FROM \"Users\" WHERE \"UserName\" = @FriendNickname", connection))
                {
                    command.Parameters.AddWithValue("@FriendNickname", NameFriend);

                    var result = command.ExecuteScalar();
                    if (result == null)
                    {
                        throw new InvalidOperationException("User with this nickname does not exist.");
                    }

                    friendId = (int)result;
                }

                // Створення запису про дружбу
                using (var insertCommand = new NpgsqlCommand(
                    "INSERT INTO \"Friendships\" (\"User1Id\", \"User2Id\") VALUES (@User1Id, @User2Id)", connection))
                {
                    insertCommand.Parameters.AddWithValue("@User1Id", CreateAcc.CurrentUserId);
                    insertCommand.Parameters.AddWithValue("@User2Id", friendId);

                    var rowsAffected = insertCommand.ExecuteNonQuery();
                }
            }
        }

        public List<int> GetFriends()
        {
            var friends = new List<int>();

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();  

                using (var command = new NpgsqlCommand(
                    "SELECT \"User2Id\" " +
                    "FROM \"Friendships\" " +
                    "WHERE \"User1Id\" = @CurrentId", connection))
                {
                    command.Parameters.AddWithValue("@CurrentId", CreateAcc.CurrentUserId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            friends.Add(reader.GetInt32(0)); 
                        }
                    }
                }
            }

            return friends;
        }

    }
}
