using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System;
using System.Collections.ObjectModel;
using System.Windows;


namespace BLL
{
    using System;
    using System.Collections.Generic;

    public  class MainPageHelper
    {
        private string connectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!"; // замініть на ваші налаштування бази даних
        private int userId;

        public MainPageHelper()
        {
            userId = CreateAcc.CurrentUserId; // Отримуємо ID поточного користувача
        }


        // Метод для отримання статистики за сьогодні для користувача
        // Ваш метод для отримання статистики
        public ObservableCollection<GameStatistic> GetAllGamesTimeForUser()
        {
            var gamesTime = new ObservableCollection<GameStatistic>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Get the start and end of the current day (midnight to midnight)
                var todayStart = DateTime.Today;
                var todayEnd = DateTime.Today.AddDays(1).AddSeconds(-1);

                var command = new NpgsqlCommand(
                    "SELECT \"GameId\", \"GameName\", EXTRACT(EPOCH FROM (\"End\" - \"Start\")) AS duration " +
                    "FROM public.\"Sessions\" " +
                    "WHERE \"UserId\" = @UserId " +
                    "AND \"Start\" >= @TodayStart AND \"End\" <= @TodayEnd", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@TodayStart", todayStart);
                command.Parameters.AddWithValue("@TodayEnd", todayEnd);

                // Temporary dictionary to aggregate durations by GameId
                var gameDurations = new Dictionary<int, double>();
                var gameNames = new Dictionary<int, string>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var gameId = reader.GetInt32(0);
                        var gameName = reader.GetString(1);
                        var duration = reader.GetDouble(2);

                        if (!gameDurations.ContainsKey(gameId))
                        {
                            gameDurations[gameId] = 0;
                            gameNames[gameId] = gameName;
                        }

                        gameDurations[gameId] += duration;
                    }
                }

                // Transform aggregated data into the result collection
                foreach (var gameId in gameDurations.Keys)
                {
                    gamesTime.Add(new GameStatistic
                    {
                        ApplicationName = gameNames[gameId],
                        SessionDuration = gameDurations[gameId] / 60 // Convert seconds to minutes
                    });
                }
            }

            return gamesTime;
        }

    }

    // Клас для зберігання інформації про статистику гри
    public class GameStatistic
    {
        public string ApplicationName { get; set; }
        public double SessionDuration { get; set; }
    }
}
