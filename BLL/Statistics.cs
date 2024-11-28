using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static BLL.Statistics;
using static System.Net.Mime.MediaTypeNames;

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
        public class GameStatistics
        {
            public string ApplicationName { get; set; }
            public double SessionDuration { get; set; }
            public int Month {  get; set; }
            public int Day { get; set; }
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

        public List<GameStatistics> GetGameStatisticsForMonth(string gameName, int year, int month)
        {
            List<GameStatistics> gameStatistics = new List<GameStatistics>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"SELECT EXTRACT(DAY FROM \"Start\") AS day, SUM(EXTRACT(epoch FROM \"End\" - \"Start\")) AS total_secondsFROMpublic.\"Sessions\"WHERE \"UserId\" = @UserId AND \"GameName\" = @GameName AND EXTRACT(YEAR FROM \"Start\") = @Year AND EXTRACT(MONTH FROM \"Start\") = @MonthGROUP BYdayORDER BYday;";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@GameName", gameName);
                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@Month", month);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int day = reader.GetInt32(0);
                        double totalSeconds = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);

                    gameStatistics.Add(new GameStatistics
                        {
                            ApplicationName = gameName,
                            SessionDuration = totalSeconds, // В секундах
                            Month = month,
                            Day = day
                       });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка при отриманні статистики для місяця: {ex.Message}");
    }

    return gameStatistics;
}



    }
}
