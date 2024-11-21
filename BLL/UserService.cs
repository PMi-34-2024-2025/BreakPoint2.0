using System;
using System.Data;
using System.Linq;
using Npgsql;

namespace BLL
{
    public class UserService
    {
        private const string ConnectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";

        public User GetUserById(int userId)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT \"UserName\", \"UserEmail\", \"UserPassword\" FROM \"Users\" WHERE \"UserId\" = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserName = reader.GetString(0),
                                Email = reader.GetString(1),
                                PasswordHash = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateUser(int userId, string userName, string email, string newPassword)
        {
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("UPDATE \"Users\" SET \"UserName\" = @UserName, \"UserEmail\" = @Email, \"UserPassword\" = @Password WHERE \"UserId\" = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", newPassword);
                    command.Parameters.AddWithValue("@UserId", userId);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

    }

    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
