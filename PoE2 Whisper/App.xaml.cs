using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PoE2Whisper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private void ApplyTheme()
        {
            string registryKeyPath = @"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
            object themeValue = Registry.GetValue(registryKeyPath, "AppsUseLightTheme", null);

            bool isLightTheme = themeValue != null && (int)themeValue == 1;

            string themeResource = isLightTheme ? "Themes/LightTheme.xaml" : "Themes/DarkTheme.xaml";

            ResourceDictionary themeDictionary = new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/{themeResource}")
            };

            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(themeDictionary);
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ApplyTheme();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
            ApplyTheme();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
            base.OnExit(e);
        }
    }
}
