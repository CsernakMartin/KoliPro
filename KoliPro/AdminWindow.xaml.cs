using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace KoliPro
{
    public sealed partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            this.InitializeComponent();
            if (App.CurrentUser != null)
            {
                AdminInfoTextBlock.Text = $"Bejelentkezve: {App.CurrentUser.FullName} (Admin) | ";
            }
        }
        private string _selectedPhotoPath;

        private async void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _selectedPhotoPath = file.Path;
                var bitmap = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(_selectedPhotoPath));
                ResidentPhotoPreview.Source = bitmap;
            }
        }
        private async void SaveResident_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ResidentNameInput.Text) || string.IsNullOrWhiteSpace(ResidentRoomInput.Text))
            {
                return;
            }

            var saveButton = sender as Button;
            saveButton.IsEnabled = false;

            try
            {
                string photoUrl = "";

                if (!string.IsNullOrEmpty(_selectedPhotoPath))
                {
                    string fileName = $"{ResidentRoomInput.Text}_{ResidentNameInput.Text.Replace(" ", "_")}{Path.GetExtension(_selectedPhotoPath)}";

                    byte[] fileBytes = File.ReadAllBytes(_selectedPhotoPath);

                    await App.SupabaseClient.Storage
                        .From("residents_photos")
                        .Upload(fileBytes, fileName);

                    photoUrl = App.SupabaseClient.Storage
                        .From("residents_photos")
                        .GetPublicUrl(fileName);
                }

                var newResident = new Resident
                {
                    FullName = ResidentNameInput.Text,
                    RoomNumber = ResidentRoomInput.Text,
                    Email = ResidentEmailInput.Text,
                    PhotoUrl = photoUrl
                };

                await App.SupabaseClient.From<Resident>().Insert(newResident);

                ResidentNameInput.Text = "";
                ResidentRoomInput.Text = "";
                ResidentEmailInput.Text = "";
                _selectedPhotoPath = null;
                ResidentPhotoPreview.Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/placeholder_user.png"));

                ContentDialog successDialog = new ContentDialog
                {
                    Title = "Siker",
                    Content = "Lakó sikeresen elmentve!",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await successDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Hiba a mentésnél: {ex.Message}");
            }
            finally
            {
                saveButton.IsEnabled = true;
            }
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWin = new MainWindow();
            loginWin.Activate();
            this.Close();
        }

        private void OpenResidentManagement_Click(object sender, RoutedEventArgs e)
        {
            AdminDashboardGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            ResidentGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            BackButton.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        private void BackToAdminDashboard_Click(object sender, RoutedEventArgs e)
        {
            ResidentGrid.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            AdminDashboardGrid.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            BackButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        }
        private void OpenRoomConfig_Click(object sender, RoutedEventArgs e) { /* Később */ }
        private void OpenErrorReports_Click(object sender, RoutedEventArgs e) { /* Később */ }
    }
}
