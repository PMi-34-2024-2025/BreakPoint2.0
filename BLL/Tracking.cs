using Npgsql;  // Для роботи з PostgreSQL
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using BLL;

namespace BLL
{
    public class Tracking
    {
        private string connectionString = "Host=breakdatabase.postgres.database.azure.com;Port=5432;Database=BreakDB;Username=postgres;Password=12345678bp!"; // замініть на ваші налаштування бази даних

        public ObservableCollection<ApplicationTime> TrackedTimes { get; set; } = new ObservableCollection<ApplicationTime>();
        public ObservableCollection<SessionResult> SessionResults { get; set; } = new ObservableCollection<SessionResult>();

        private string? SelectedApp { get; set; }
        private DateTime LastSwitchTime = DateTime.Now;
        private bool IsTracking = false;
        private Thread? trackingThread;

        public Tracking()
        {
            LoadApplications();
        }

        // Завантажуємо програми, які зараз працюють
        private void LoadApplications()
        {
            var apps = GetRunningApplications();
            foreach (var app in apps)
            {
                TrackedTimes.Add(new ApplicationTime { Name = app });
            }
        }

        // Отримуємо список всіх запущених програм
        private ObservableCollection<string> GetRunningApplications()
        {
            var applications = new ObservableCollection<string>();
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainWindowHandle != IntPtr.Zero && IsWindowVisible(process.MainWindowHandle))
                    {
                        applications.Add(process.ProcessName);
                    }
                }
                catch
                {
                    // Ігноруємо помилки
                }
            }
            return applications;
        }

        // Додаємо програму до бази даних, якщо її ще немає
        private void AddGameToDatabase(string gameName)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Перевірка, чи є вже програма в базі
                var command = new NpgsqlCommand("SELECT \"GameId\" FROM public.\"Games\" WHERE \"GameName\" = @gameName", connection);
                command.Parameters.AddWithValue("gameName", gameName);

                var result = command.ExecuteScalar();
                if (result == null) // Якщо програма не знайдена, додаємо її
                {
                    var insertCommand = new NpgsqlCommand("INSERT INTO public.\"Games\" (\"GameName\") VALUES (@gameName) RETURNING \"GameId\"", connection);
                    insertCommand.Parameters.AddWithValue("gameName", gameName);
                    var gameId = insertCommand.ExecuteScalar();
                    // Зберігаємо GameId для подальшого використання
                    TrackedTimes.First(t => t.Name == gameName).GameId = (int)gameId;
                }
                else
                {
                    TrackedTimes.First(t => t.Name == gameName).GameId = (int)result;
                }
            }
        }

        // Додаємо сесію в базу даних
        private void AddSessionToDatabase(int userId, int gameId, DateTime start, DateTime end)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var command = new NpgsqlCommand(
                    "INSERT INTO public.\"Sessions\" (\"UserId\", \"GameId\", \"GameName\", \"Start\", \"End\") VALUES (@userId, @gameId, @gameName, @start, @end)", connection);

                command.Parameters.AddWithValue("userId", userId);
                command.Parameters.AddWithValue("gameId", gameId);
                command.Parameters.AddWithValue("gameName", SelectedApp);  // Використовуємо назву програми
                command.Parameters.AddWithValue("start", start);
                command.Parameters.AddWithValue("end", end);

                command.ExecuteNonQuery();
            }
        }

        // Запускаємо відстеження для вибраної програми
        public void StartTracking(string selectedApp)
        {
            if (string.IsNullOrEmpty(selectedApp)) return;

            SelectedApp = selectedApp;
            IsTracking = true;
            LastSwitchTime = DateTime.Now;

            // Додати програму в базу даних, якщо її ще немає
            AddGameToDatabase(selectedApp);

            trackingThread = new Thread(TrackActiveWindow)
            {
                IsBackground = true
            };
            trackingThread.Start();
        }

        
        private void TrackActiveWindow()
        {
            while (IsTracking)
            {
                IntPtr hwnd = GetForegroundWindow();
                if (hwnd == IntPtr.Zero)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                uint processId;
                GetWindowThreadProcessId(hwnd, out processId);
                string activeApp;

                try
                {
                    activeApp = Process.GetProcessById((int)processId).ProcessName;
                }
                catch
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (activeApp == SelectedApp)
                {
                    // Якщо активна вибрана програма
                    var trackedApp = TrackedTimes.FirstOrDefault(t => t.Name == activeApp);
                    if (trackedApp != null)
                    {
                        // Додаємо час, що минув з останньої зміни
                        trackedApp.TimeSpent += DateTime.Now - LastSwitchTime;
                        LastSwitchTime = DateTime.Now;
                    }
                }
                else
                {
                    // Якщо активна інша програма, оновлюємо LastSwitchTime
                    LastSwitchTime = DateTime.Now;
                }

                Thread.Sleep(1000);
            }
        }

        
        public void StopTracking()
        {
            IsTracking = false;

            if (trackingThread != null && trackingThread.IsAlive)
            {
                trackingThread.Join();
            }

            var trackedApp = TrackedTimes.FirstOrDefault(t => t.Name == SelectedApp);
            if (trackedApp != null)
            {
                // Записуємо час сесії, якщо програма була активною
                var sessionTime = trackedApp.TimeSpent;
                SessionResults.Add(new SessionResult
                {
                    ApplicationName = trackedApp.Name,
                    SessionDuration = sessionTime
                });

                int userId = CreateAcc.CurrentUserId; // Викликаємо вашу функцію GetCurrentUserId
                AddSessionToDatabase(userId, trackedApp.GameId, LastSwitchTime - sessionTime, LastSwitchTime);
            }
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        
    }

    public class ApplicationTime : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private TimeSpan _timeSpent;
        public int GameId { get; set; }  // Зберігаємо GameId

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public TimeSpan TimeSpent
        {
            get => _timeSpent;
            set
            {
                _timeSpent = value;
                OnPropertyChanged(nameof(TimeSpent));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SessionResult
    {
        public string ApplicationName { get; set; } = string.Empty;
        public TimeSpan SessionDuration { get; set; }
    }
}
