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
    using System.Drawing;

    public  class MainPageHelper
    {
        private string connectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!"; // замініть на ваші налаштування бази даних
        private int userId;

        public MainPageHelper()
        {
            userId = CreateAcc.CurrentUserId; // Отримуємо ID поточного користувача
        }
        // Форматування тривалості у вигляді год:хв:сек
        public static string FormatDuration(double sessionDuration)
        {
            int hours = (int)(sessionDuration / 60);
            int minutes = (int)(sessionDuration % 60);
            int seconds = (int)((sessionDuration * 60) % 60);
            return $"{hours}h {minutes}m {seconds}s";
        }


        public (string UserName, string UserEmail) GetUserDetailsById(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException("Invalid UserId.");
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand(
                    "SELECT \"UserName\", \"UserEmail\" FROM \"Users\" WHERE \"UserId\" = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string userName = reader.GetString(0);
                            string userEmail = reader.GetString(1);
                            return (userName, userEmail);
                        }
                        else
                        {
                            throw new InvalidOperationException("User not found.");
                        }
                    }
                }
            }
        }



        // Метод для отримання статистики за сьогодні для користувача
        
        public ObservableCollection<GameStatistic> GetAllGamesTimeForUser()
        {
            var gamesTime = new ObservableCollection<GameStatistic>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

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
        // Функція для малювання кругової діаграми на Canvas
    

        // Функція для генерації випадкових кольорів для секторів
       
    }

    // Клас для зберігання інформації про статистику гри
    public class GameStatistic
    {
        public string ApplicationName { get; set; }
        public double SessionDuration { get; set; }
        public string StringSessionDuration { get; set; }   
    }
}
