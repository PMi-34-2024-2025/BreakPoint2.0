using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace BreakPoint2._0
{
    public partial class TrackingApps : Page
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        public ObservableCollection<ApplicationTime> TrackedTimes { get; set; } = new ObservableCollection<ApplicationTime>();
        public ObservableCollection<SessionResult> SessionResults { get; set; } = new ObservableCollection<SessionResult>();

        private string? SelectedApp { get; set; }
        private DateTime LastSwitchTime = DateTime.Now;
        private bool IsTracking = false;
        private Thread? trackingThread;

        public TrackingApps()
        {
            InitializeComponent();
            DataContext = this;

            LoadApplications();
        }

        private void LoadApplications()
        {
            var apps = GetRunningApplications();
            foreach (var app in apps)
            {
                TrackedTimes.Add(new ApplicationTime { Name = app });
            }
            ApplicationsComboBox.ItemsSource = TrackedTimes;
            ApplicationsComboBox.SelectionChanged += ApplicationsComboBox_SelectionChanged;
        }

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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedApp)) return;

            IsTracking = true;
            LastSwitchTime = DateTime.Now;

            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            trackingThread = new Thread(TrackActiveWindow)
            {
                IsBackground = true
            };
            trackingThread.Start();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            IsTracking = false;

            if (trackingThread != null && trackingThread.IsAlive)
            {
                trackingThread.Join();
            }

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;

            // Додаємо час до відповідної програми в першому списку
            var trackedApp = TrackedTimes.FirstOrDefault(t => t.Name == SelectedApp);
            if (trackedApp != null)
            {
                var sessionTime = DateTime.Now - LastSwitchTime;
                trackedApp.TimeSpent += sessionTime;

                // Додаємо запис про сесію в другий список
                SessionResults.Add(new SessionResult
                {
                    ApplicationName = trackedApp.Name,
                    SessionDuration = sessionTime
                });
            }
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

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (activeApp == SelectedApp)
                    {
                        LastSwitchTime = DateTime.Now;
                    }
                });

                Thread.Sleep(1000);
            }
        }

        private void ApplicationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApplicationsComboBox.SelectedItem is ApplicationTime app)
            {
                SelectedApp = app.Name;
                StartButton.IsEnabled = true;
            }
        }
    }

    public class ApplicationTime : INotifyPropertyChanged
    {
        private TimeSpan _timeSpent;

        public string Name { get; set; } = string.Empty;

        public TimeSpan TimeSpent
        {
            get => _timeSpent;
            set
            {
                if (_timeSpent != value)
                {
                    _timeSpent = value;
                    OnPropertyChanged(nameof(TimeSpent));  // Сповіщаємо про зміну
                }
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
