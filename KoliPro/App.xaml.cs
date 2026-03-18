using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace KoliPro
{
    public partial class App : Application
    {
        public static string SupabaseUrl = "https://rntgjhkofmqrkospccsu.supabase.co";
        public static string SupabaseKey = "sb_publishable_nID58dov9MEu-6-VtPrKng__r74_qhW";
        public static Employee CurrentUser { get; set; }

        public static Supabase.Client SupabaseClient;

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            SupabaseClient = new Supabase.Client(SupabaseUrl, SupabaseKey);
            await SupabaseClient.InitializeAsync();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
