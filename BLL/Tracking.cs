// BLL/TrackingService.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BLL
{
    public class Tracking
    {
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

        private void LoadApplications()
        {
            var apps = GetRunningApplications();
            foreach (var app in apps)
            {
                TrackedTimes.Add(new ApplicationTime { Name = app });
            }
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

        public void StartTracking(string selectedApp)
        {
            if (string.IsNullOrEmpty(selectedApp)) return;

            SelectedApp = selectedApp;
            IsTracking = true;
            LastSwitchTime = DateTime.Now;

            trackingThread = new Thread(TrackActiveWindow)
            {
                IsBackground = true
            };
            trackingThread.Start();
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

                if (activeApp == SelectedApp)
                {
                    LastSwitchTime = DateTime.Now;
                }

                Thread.Sleep(1000);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
    }

    public class ApplicationTime
    {
        public string Name { get; set; } = string.Empty;
        public TimeSpan TimeSpent { get; set; }
    }

    public class SessionResult
    {
        public string ApplicationName { get; set; } = string.Empty;
        public TimeSpan SessionDuration { get; set; }
    }
}

