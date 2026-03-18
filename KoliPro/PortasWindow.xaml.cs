using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoliPro
{
    // Segédosztály a listához


    public sealed partial class PortasWindow : Window
    {
        private DispatcherTimer _clockTimer;
        private ObservableCollection<GuestInfo> _guests = new();
        private ObservableCollection<GuestInfo> _filteredGuests = new();

        public PortasWindow()
        {
            this.InitializeComponent();
            SetupClock();
            GuestListView.ItemsSource = _filteredGuests;

            LoadDataFromCloud();
            if (App.CurrentUser != null)
            {
                LoginInfoTextBlock.Text = $"Bejelentkezve: {App.CurrentUser.FullName} ({App.CurrentUser.Role}) | ";
            }
        }
        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (App.CurrentUser != null)
            {
                LoginInfoTextBlock.Text = $"Bejelentkezve: {App.CurrentUser.FullName} ({App.CurrentUser.Role}) | ";
            }
        }
        public static bool IsStillHere(string leaveTime) => leaveTime == "---";
        public static bool IsAlreadyOut(string leaveTime) => leaveTime != "---";


        private async void LoadDataFromCloud()
        {
            try
            {
                var result = await App.SupabaseClient.From<GuestInfo>()
                    .Get();
                _guests.Clear();
                foreach (var guest in result.Models)
                {
                    _guests.Add(guest);
                }
                UpdateFilteredList("");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Supabase hiba: {ex.Message}");
            }
        }
        private async void MenuCheckout_Click(object sender, RoutedEventArgs e)
        {
            var menuEntry = sender as MenuFlyoutItem;
            var guest = menuEntry.DataContext as GuestInfo;

            if (guest == null || guest.LeaveTime != "---") return;

            ContentDialog dialog = new ContentDialog
            {
                Title = "Kijelentkeztetés",
                Content = $"{guest.Name} kiléptetése?",
                PrimaryButtonText = "OK",
                CloseButtonText = "Mégse",
                XamlRoot = this.Content.XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                guest.LeaveTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");
                await App.SupabaseClient.From<GuestInfo>().Update(guest);
            }
        }


        private async void MenuReAdd_Click(object sender, RoutedEventArgs e)
        {
            var menuEntry = sender as MenuFlyoutItem;
            var oldGuest = menuEntry.DataContext as GuestInfo;

            if (oldGuest == null) return;

            var newGuest = new GuestInfo
            {
                Name = oldGuest.Name,
                CardId = oldGuest.CardId,
                Room = oldGuest.Room,
                ArriveTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss"),
                LeaveTime = "---"
            };

            var response = await App.SupabaseClient.From<GuestInfo>().Insert(newGuest);
            var saved = response.Models.FirstOrDefault();

            _guests.Add(saved ?? newGuest);
            UpdateFilteredList(GuestSearchBox.Text ?? "");
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
            TimeInput.Text = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss");
        }

        private void BackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            GuestGrid.Visibility = Visibility.Collapsed;
            DashboardGrid.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Collapsed;
        }

        private async void AddGuest_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameInput.Text)) return;

            var newGuest = new GuestInfo
            {
                Name = NameInput.Text,
                CardId = CardInput.Text,
                Room = RoomInput.Text,
                ArriveTime = DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss"),
                LeaveTime = "---"
            };

            try
            {
                var response = await App.SupabaseClient
                    .From<GuestInfo>()
                    .Insert(newGuest, new Postgrest.QueryOptions { Returning = Postgrest.QueryOptions.ReturnType.Representation });

                var savedGuest = response.Models.FirstOrDefault();
                var guestToDisplay = savedGuest ?? newGuest;

                _guests.Add(guestToDisplay);
                UpdateFilteredList(GuestSearchBox.Text ?? "");

                NameInput.Text = "";
                CardInput.Text = "";
                RoomInput.Text = "";
                TimeInput.Text = DateTime.Now.ToString("HH:mm:ss");

                System.Diagnostics.Debug.WriteLine("Sikeres mentés a felhõbe!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Supabase hiba: {ex.Message}");
            }
        }

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