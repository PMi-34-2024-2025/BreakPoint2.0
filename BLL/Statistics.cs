using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BLL
{
    public class Statistics
    {
        private string connectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!";
        private int userId;

        public Statistics()
        {
            userId = CreateAcc.CurrentUserId; // Отримуємо ID поточного користувача
        }

        public List<string> GetGameNames()
        {
            List<string> gameNames = new List<string>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT DISTINCT \"GameName\" FROM public.\"Sessions\" WHERE \"UserId\" = @UserId";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                gameNames.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при отриманні GameName: {ex.Message}");
            }

            return gameNames;
        }

        public double GetTotalTimeForGame(string gameName, List<DateTime> selectedDates)
        {
            double totalSeconds = 0;

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Формуємо умови для фільтрації по датах
                    var dateConditions = string.Join(" OR ", selectedDates.ConvertAll(date => $"(\"Start\"::date = '{date:yyyy-MM-dd}')"));

                    string query = $"SELECT SUM(EXTRACT(epoch FROM \"End\" - \"Start\")) AS total_seconds FROM public.\"Sessions\"WHERE \"UserId\" = @UserId AND \"GameName\" = @GameName AND ({dateConditions})";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@GameName", gameName);

                        var result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            totalSeconds = Convert.ToDouble(result);
                        }
}
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при обчисленні загального часу для гри: {ex.Message}");
            }

            return totalSeconds; // Повертаємо час в секундах
        }
    }
}
