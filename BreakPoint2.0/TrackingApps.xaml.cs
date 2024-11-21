using BLL;
using System.Windows;
using System.Windows.Controls;

namespace BreakPoint2._0
{
    public partial class TrackingApps : Page
    {
        private readonly Tracking _trackingService;

        public TrackingApps()
        {
            InitializeComponent();
            _trackingService = new Tracking();
            DataContext = _trackingService;

            // Після ініціалізації класу заповнюємо ComboBox даними
            ApplicationsComboBox.ItemsSource = _trackingService.TrackedTimes;
            ApplicationsComboBox.SelectionChanged += ApplicationsComboBox_SelectionChanged;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedApp = (ApplicationsComboBox.SelectedItem as ApplicationTime)?.Name;
            if (selectedApp != null)
            {
                _trackingService.StartTracking(selectedApp);
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _trackingService.StopTracking();
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void ApplicationsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedApp = (ApplicationsComboBox.SelectedItem as ApplicationTime)?.Name;
            StartButton.IsEnabled = selectedApp != null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPage());
        }
    }
}
