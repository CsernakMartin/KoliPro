using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace KoliPro
{
    // Segédosztály a listához
    public class GuestInfo
    {
        public string Name { get; set; }
        public string CardId { get; set; }
        public string Room { get; set; }
        public string ArriveTime { get; set; }
        public string LeaveTime { get; set; }
    }

    public sealed partial class PortasWindow : Window
    {
        private DispatcherTimer _clockTimer;
        // Ez a lista tárolja a vendégeket a memóriában (vázlat szinten)
        private ObservableCollection<GuestInfo> _guests = new();
        private ObservableCollection<GuestInfo> _filteredGuests = new();

        public PortasWindow()
        {
            this.InitializeComponent();
            SetupClock();
            GuestListView.ItemsSource = _filteredGuests;
        }

        private void SetupClock()
        {
            _clockTimer = new DispatcherTimer();
            _clockTimer.Interval = TimeSpan.FromSeconds(1);
            _clockTimer.Tick += (s, e) => ClockTextBlock.Text = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");
            _clockTimer.Start();
            ClockTextBlock.Text = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");
        }

        // NAVIGÁCIÓ
        private void OpenGuestManagement_Click(object sender, RoutedEventArgs e)
        {
            DashboardGrid.Visibility = Visibility.Collapsed;
            GuestGrid.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
            TimeInput.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            GuestGrid.Visibility = Visibility.Collapsed;
            DashboardGrid.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Collapsed;
        }

        // VENDÉG HOZZÁADÁSA
        private void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text)) return;

            var newGuest = new GuestInfo
            {
                Name = NameInput.Text,
                CardId = CardInput.Text,
                Room = RoomInput.Text,
                ArriveTime = TimeInput.Text,
                LeaveTime = "---"
            };

            _guests.Add(newGuest);
            UpdateFilteredList("");

            // Mezõk ürítése
            NameInput.Text = "";
            CardInput.Text = "";
            RoomInput.Text = "";
            TimeInput.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // KERESÉS LOGIKA
        private void GuestSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            UpdateFilteredList(sender.Text);
        }

        private void UpdateFilteredList(string query)
        {
            _filteredGuests.Clear();
            var filtered = _guests.Where(g =>
                g.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                g.CardId.Contains(query, StringComparison.OrdinalIgnoreCase));

            foreach (var guest in filtered)
            {
                _filteredGuests.Add(guest);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _clockTimer.Stop();
            var loginWin = new MainWindow();
            loginWin.Activate();
            this.Close();
        }

        // Egyéb vázlatok
        private void KeyIssuance_Click(object sender, RoutedEventArgs e) { /* Késõbb */ }
        private void ReportResident_Click(object sender, RoutedEventArgs e) { /* Késõbb */ }
    }
}