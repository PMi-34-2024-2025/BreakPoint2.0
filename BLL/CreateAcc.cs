using System;
using Npgsql;

namespace BLL
{
    public class CreateAcc
    {
        private const string ConnectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";
        public int CurrentUserId; 
        public bool Register(string nickname, string email, string password)
        {
            // Валідація введених даних
            if (string.IsNullOrWhiteSpace(nickname) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("All fields must be filled.");
            }

            if (!email.Contains("@"))
            {
                throw new ArgumentException("Invalid email format.");
            }

            // Логіка реєстрації користувача
            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Перевірка, чи існує вже користувач із таким ніком
                using (var checkCommand = new NpgsqlCommand("SELECT COUNT(*) FROM \"Users\" WHERE \"UserName\" = @Nickname", connection))
                {
                    checkCommand.Parameters.AddWithValue("@Nickname", nickname);
                    var userExists = (long)checkCommand.ExecuteScalar() > 0;

                    if (userExists)
                    {
                        throw new InvalidOperationException("User with this nickname already exists.");
                    }
                }

                // Додавання нового користувача
                using (var insertCommand = new NpgsqlCommand(
                    "INSERT INTO \"Users\" (\"UserName\", \"UserPassword\", \"UserEmail\", \"IsAdmin\") VALUES (@Nickname, @Password, @Email, @IsAdmin)", connection))
                {
                    insertCommand.Parameters.AddWithValue("@Nickname", nickname);
                    insertCommand.Parameters.AddWithValue("@Password", password); // У реальних проєктах пароль має бути хешований!
                    insertCommand.Parameters.AddWithValue("@Email", email);
                    insertCommand.Parameters.AddWithValue("@IsAdmin", false);

                    var rowsAffected = insertCommand.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine($"User registered: {nickname}, {email}");
                        return true;
                    }
                }
            }

            return false;
        }

        // Функція для входу користувача за ніком
        public bool Login(string nickname, string password)
        {
            // Валідація введених даних
            if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Both fields must be filled.");
            }

            using (var connection = new NpgsqlConnection(ConnectionString))
            {
                connection.Open();

                // Перевірка наявності користувача з таким ніком
                using (var checkCommand = new NpgsqlCommand("SELECT \"UserId\", \"UserPassword\" FROM \"Users\" WHERE \"UserName\" = @Nickname", connection))
                {
                    checkCommand.Parameters.AddWithValue("@Nickname", nickname);
                    using (var reader = checkCommand.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            throw new InvalidOperationException("User not found.");
                        }

                        // Отримання даних користувача
                        int userId = reader.GetInt32(0);
                        string storedPassword = reader.GetString(1);
                      

                        // У реальних проєктах перевірка пароля повинна бути через хешування
                        if (storedPassword != password)
                        {
                            throw new InvalidOperationException("Invalid password.");
                        }

                        // Збереження ID користувача у поточній сесії
                        CurrentUserId = userId;

                        Console.WriteLine($"User logged in successfully. UserID: {CurrentUserId}");
                        return true; // Вхід успішний
                    }
                }
            }
        }

    }
}