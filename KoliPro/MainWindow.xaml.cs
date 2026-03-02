using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI;
using System;
using WinRT.Interop;
using Microsoft.UI.Windowing;

namespace KoliPro
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

        }

        
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string user = UsernameBox.Text;
            string pass = PasswordBox.Password;

            if (pass == "1")
            {
                if (user == "1")
                {
                    var PortasWin = new PortasWindow();
                    PortasWin.Activate();
                    this.Close();
                }
            }
            else
            {
                ErrorText.Text = "Hibás felhasználónév vagy jelszó!";
                ErrorText.Visibility = Visibility.Visible;
            }
        }
    }
}