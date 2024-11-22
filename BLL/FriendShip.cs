using Npgsql;
using System;

namespace BLL
{
    public class FriendShip//nothing
    {
        private const string ConnectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";

        public void AddFriendToMe(string NameFriend)
        {
            try
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
                            Console.WriteLine("This user does not exist");
                            return;
                        }

                        friendId = (int)result;
                    }

                    using (var checkCommand = new NpgsqlCommand(
                        "SELECT COUNT(*) FROM \"Friendships\" " +
                        "WHERE (\"User1Id\" = @User1Id AND \"User2Id\" = @User2Id) OR (\"User1Id\" = @User2Id AND \"User2Id\" = @User1Id)", connection))
                    {
                        checkCommand.Parameters.AddWithValue("@User1Id", CreateAcc.CurrentUserId);
                        checkCommand.Parameters.AddWithValue("@User2Id", friendId);

                        var existingFriendship = (long)checkCommand.ExecuteScalar();
                        if (existingFriendship > 0)
                        {
                            Console.WriteLine("This friendship already exists.");
                            return;
                        }
                    }

                    using (var insertCommand = new NpgsqlCommand(
                        "INSERT INTO \"Friendships\" (\"User1Id\", \"User2Id\") VALUES (@User1Id, @User2Id)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@User1Id", CreateAcc.CurrentUserId);
                        insertCommand.Parameters.AddWithValue("@User2Id", friendId);

                        var rowsAffected = insertCommand.ExecuteNonQuery();
                        Console.WriteLine(rowsAffected > 0 ? "Friend added successfully." : "Failed to add friend.");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Invalid operation: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        public List<FriendSessionInfo> GetFriends()
        {
            var friends = new List<FriendSessionInfo>();

            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error opening database connection: " + ex.Message);
                        throw new InvalidOperationException("Unable to connect to the database.", ex);
                    }

                    using (var command = new NpgsqlCommand(
                        "SELECT f.\"User2Id\", u.\"UserName\", s.\"SessionId\", s.\"GameId\", s.\"GameName\", s.\"Start\", s.\"End\" " +
                        "FROM \"Friendships\" f " +
                        "JOIN \"Users\" u ON u.\"UserId\" = f.\"User2Id\" " +
                        "LEFT JOIN \"Sessions\" s ON s.\"UserId\" = f.\"User2Id\" " +
                        "WHERE f.\"User1Id\" = @CurrentId", connection))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@CurrentId", CreateAcc.CurrentUserId);

                            using (var reader = command.ExecuteReader())
                            {
                                var friendSessionsDict = new Dictionary<int, FriendSessionInfo>();

                                while (reader.Read())
                                {
                                    try
                                    {
                                        var friendId = reader.GetInt32(0);
                                        var friendName = reader.GetString(1);
                                        var sessionId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                                        var gameId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3);
                                        var gameName = reader.IsDBNull(4) ? null : reader.GetString(4);
                                        var start = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                                        var end = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);

                                        if (!friendSessionsDict.ContainsKey(friendId))
                                        {
                                            friendSessionsDict[friendId] = new FriendSessionInfo
                                            {
                                                FriendId = friendId,
                                                FriendName = friendName
                                            };
                                        }

                                        if (start.HasValue && end.HasValue &&
                                            (friendSessionsDict[friendId].End == null || end > friendSessionsDict[friendId].End))
                                        {
                                            friendSessionsDict[friendId].SessionId = sessionId ?? 0;
                                            friendSessionsDict[friendId].GameId = gameId ?? 0;
                                            friendSessionsDict[friendId].GameName = gameName;
                                            friendSessionsDict[friendId].Start = start.Value;
                                            friendSessionsDict[friendId].End = end.Value;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error reading data for a friend: {ex.Message}");
                                        // Optionally, skip this friend and continue.
                                    }
                                }

                                friends.AddRange(friendSessionsDict.Values);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error executing SQL command: " + ex.Message);
                            throw new InvalidOperationException("Failed to fetch friends data.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw new InvalidOperationException("A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }

            return friends;
        }





        public void RemoveFriend(int friendId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error opening database connection: " + ex.Message);
                        throw new InvalidOperationException("Unable to connect to the database.", ex);
                    }

                    using (var command = new NpgsqlCommand(
                        "DELETE FROM \"Friendships\" WHERE (\"User1Id\" = @CurrentId AND \"User2Id\" = @FriendId) " +
                        "OR (\"User1Id\" = @FriendId AND \"User2Id\" = @CurrentId)", connection))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@CurrentId", CreateAcc.CurrentUserId);
                            command.Parameters.AddWithValue("@FriendId", friendId);

                            var rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                Console.WriteLine("No friendship record found to delete.");
                                throw new InvalidOperationException("The specified friendship does not exist.");
                            }

                            Console.WriteLine($"Successfully removed friendship with FriendId: {friendId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error executing SQL command: " + ex.Message);
                            throw new InvalidOperationException("Failed to remove friendship.", ex);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                throw new InvalidOperationException("A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected error: " + ex.Message);
                throw new InvalidOperationException("An unexpected error occurred.", ex);
            }
        }



        public class FriendSessionInfo
        {
            public int FriendId { get; set; }
            public string FriendName { get; set; }  // Додаємо поле для імені друга
            public int SessionId { get; set; }
            public int GameId { get; set; }
            public string GameName { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}
