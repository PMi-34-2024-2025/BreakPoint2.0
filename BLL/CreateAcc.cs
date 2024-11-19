using System;
using Npgsql;

namespace BLL
{
    public class CreateAcc
    {
        private const string ConnectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";

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

                // Перевірка, чи існує вже користувач із такою поштою
                using (var checkEmailCommand = new NpgsqlCommand("SELECT COUNT(*) FROM \"Users\" WHERE \"UserEmail\" = @Email", connection))
                {
                    checkEmailCommand.Parameters.AddWithValue("@Email", email);
                    var emailExists = (long)checkEmailCommand.ExecuteScalar() > 0;

                    if (emailExists)
                    {
                        throw new InvalidOperationException("User with this email already exists.");
                    }
                }

                // Перевірка, чи існує вже користувач із таким нікнеймом
                using (var checkNicknameCommand = new NpgsqlCommand("SELECT COUNT(*) FROM \"Users\" WHERE \"UserName\" = @Nickname", connection))
                {
                    checkNicknameCommand.Parameters.AddWithValue("@Nickname", nickname);
                    var nicknameExists = (long)checkNicknameCommand.ExecuteScalar() > 0;

                    if (nicknameExists)
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


        // Функція для входу користувача
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
                using (var checkCommand = new NpgsqlCommand("SELECT \"UserPassword\" FROM \"User\" WHERE \"UserNickname\" = @Nickname", connection))
                {
                    checkCommand.Parameters.AddWithValue("@Nickname", nickname);
                    var result = checkCommand.ExecuteScalar();

                    if (result == null)
                    {
                        throw new InvalidOperationException("User not found.");
                    }

                    // У реальних проєктах перевірка пароля повинна бути через хешування
                    string storedPassword = (string)result;
                    if (storedPassword != password)
                    {
                        throw new InvalidOperationException("Invalid password.");
                    }

                    Console.WriteLine("User logged in successfully.");
                    return true; // Вхід успішний
                }
            }
        }
    }
}
