using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using WinRT.Interop;

namespace KoliPro
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass)) return;

            try
            {
                var result = await App.SupabaseClient
                    .From<Employee>()
                    .Where(x => x.Username == user)
                    .Where(x => x.Password == pass)
                    .Get();

                var loggedInUser = result.Models.FirstOrDefault();

                if (loggedInUser != null)
                {
                    App.CurrentUser = loggedInUser;

                    switch (loggedInUser.Role)
                    {
                        case "Portás":
                            var portasWin = new PortasWindow();
                            portasWin.Activate();
                            this.Close();
                            break;

                        case "Admin":
                             var adminWin = new AdminWindow();
                             adminWin.Activate();
                             this.Close();
                            break;

                        default:
                            ErrorText.Text = "Nincs jogosultsága a belépéshez!";
                            ErrorText.Visibility = Visibility.Visible;
                            break;
                    }
                }
                else
                {
                    ErrorText.Text = "Hibás felhasználónév vagy jelszó!";
                    ErrorText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login hiba: {ex.Message}");
            }
        }
    }
}