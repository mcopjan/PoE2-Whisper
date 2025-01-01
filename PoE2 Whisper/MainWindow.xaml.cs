using PoE2_Whisper;
using PoE2_Whisper.Services;
using Poe2Notify.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace PoE2Whisper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isRunning = false;
        private bool isWindowsNotificationsEnabled;
        private bool isExternalNotificationsEnabled;
        private long lastFileLength = 0;
        private bool _poeLogFileExists;
        private DispatcherTimer timer;
        private readonly WindowsNotificationService _windowsNotifications;
        private readonly PushBulletService _pushBulletService;
        private bool _isLogFileOpenedForTheFirstTime = true;
        TimeSpan checkWhispersInterval = TimeSpan.FromSeconds(10); //every 10s


        public string DefaultLogFilePath => PoE2_Whisper.Properties.Settings.Default.PoEClientLogLocation;
        public string WhisperRegexPattern => PoE2_Whisper.Properties.Settings.Default.WhisperRegEx;
        public string PushbulletAccessToken => PoE2_Whisper.Properties.Settings.Default.PushBulletToken;
        public string PushbulletDeviceName => PoE2_Whisper.Properties.Settings.Default.PushBulletDeviceName;


        public MainWindow()
        {
            _windowsNotifications = new WindowsNotificationService();
            _pushBulletService = new PushBulletService();
            InitializeComponent();
            InitializeTimer();
            _poeLogFileExists = CheckIfPoeLogFileExists();
            ToolTipService.SetInitialShowDelay(CircleIcon, 0);
            ToolTipService.SetShowDuration(CircleIcon, int.MaxValue);

            PoE2_Whisper.Properties.Settings.Default.SettingsSaving += Settings_SettingsSaving;
        }



        private void Settings_SettingsSaving(object sender, CancelEventArgs e)
        {
            CheckIfPoeLogFileExists();
        }

        private bool CheckIfPoeLogFileExists()
        {
            if (File.Exists(DefaultLogFilePath))
            {
                CircleIcon.Source = new BitmapImage(new Uri("Assets/orangecircle48.png", UriKind.Relative));
                CircleIcon.ToolTip = new ToolTip { Content = $"Notifications disabled." };
                LogTextBox.Text += $"File '{DefaultLogFilePath}' found. Please enable notifications in Settings. {Environment.NewLine}";
                WinNotificationsMenuItem.IsEnabled = true;
                ExtNotificationsMenuItem.IsEnabled = true;
                return true;
            }
            else
            {
                CircleIcon.Source = new BitmapImage(new Uri("Assets/redcircle48.png", UriKind.Relative));
                CircleIcon.ToolTip = new ToolTip { Content = $"File '{DefaultLogFilePath}' does not exist." };
                LogTextBox.Text += $"File '{DefaultLogFilePath}' not found! Modify path in Configuration -> Settings.{Environment.NewLine}";
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                WinNotificationsMenuItem.IsEnabled = false;
                ExtNotificationsMenuItem.IsEnabled = false;
                return false;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo("explorer.exe", e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = checkWhispersInterval;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string content = ReadNewLines(DefaultLogFilePath);
                Match match = Regex.Match(content, WhisperRegexPattern, RegexOptions.Multiline);

                if (match.Success)
                {
                    Console.WriteLine("Name: " + match.Groups[1].Value);      // match name of a person sending a whisper
                    Console.WriteLine("Message: " + match.Groups[2].Value);   // match a message


                    LogTextBox.Text += $"Match found: {match.Value}{Environment.NewLine}";

                    // Implement your logic for Windows and/or External notifications here
                    if (isWindowsNotificationsEnabled)
                    {
                        _windowsNotifications.ShowNotification("PoE2 Whisper", $"From {match.Groups[1].Value}: {match.Groups[2].Value}");
                    }

                    if (isExternalNotificationsEnabled)
                    {
                        _pushBulletService.ShowNotification(PushbulletAccessToken, PushbulletDeviceName, $"PoE2 Whisper -> From {match.Groups[1].Value}: {match.Groups[2].Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogTextBox.Text += $"An error occured while processing notifications: {ex.Message}{Environment.NewLine}";
            }
            
        }

        public long GetFileLength(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            catch (IOException ex)
            {
                throw new Exception($"Could not get length of file {ex.Message}");
            }
        }



        private string ReadNewLines(string filePath)
        {
            if (_isLogFileOpenedForTheFirstTime)
            {
                // ignore all the previous whispers, only scan for the ones since app was launched
                lastFileLength = GetFileLength(DefaultLogFilePath);
                _isLogFileOpenedForTheFirstTime = false;
            }
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                StringBuilder sb = new StringBuilder();
                if (fileInfo.Length > lastFileLength)
                {
                    using (FileStream fs = new FileStream(DefaultLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        sr.BaseStream.Seek(lastFileLength, SeekOrigin.Begin); // Seek to the last read position
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            sb.AppendLine(line);
                        }
                    }
                    lastFileLength = fileInfo.Length;
                }
                return sb.ToString();
            }
            catch (IOException ex)
            {
                LogTextBox.Text += $"An error occured while reading {DefaultLogFilePath} ->  {ex.Message}{Environment.NewLine}";
                return string.Empty;
            }
        }

        private void Click_NotificationsButton(object sender, RoutedEventArgs e)
        {
            NotificationsContextMenu.PlacementTarget = NotificationsButton; 
            NotificationsContextMenu.IsOpen = true;
        }

        private void Click_ClearButton(object sender, RoutedEventArgs e)
        {
            LogTextBox.Clear();
        }

        

        private void WindowsNotificationsCheckbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                // Handle Windows Notifications logic
                LogTextBox.AppendText($"Windows Notifications: {(isChecked ? "Enabled" : "Disabled")} at {DateTime.Now}\n");
                LogTextBox.ScrollToEnd();
                isWindowsNotificationsEnabled = ((CheckBox)sender).IsChecked.Value;
                UpdateWatcherStatus();
            }
        }

        private void ExternalNotificationsCheckbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                bool isChecked = checkBox.IsChecked ?? false;
                // Handle External Notifications logic
                LogTextBox.AppendText($"External Notifications: {(isChecked ? "Enabled" : "Disabled")} at {DateTime.Now}\n");
                LogTextBox.ScrollToEnd();
                isExternalNotificationsEnabled = ((CheckBox)sender).IsChecked.Value;
                UpdateWatcherStatus();
            }
        }

        private void OpenSettingsWindow(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void Click_CloseButton(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateWatcherStatus()
        {
            bool anyNotificationsEnabled = isWindowsNotificationsEnabled || isExternalNotificationsEnabled;

            if (anyNotificationsEnabled)
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                    Timer_Tick(null, EventArgs.Empty);
                    CircleIcon.Source = new BitmapImage(new Uri("Assets/greencircle48.png", UriKind.Relative));
                    CircleIcon.ToolTip = new ToolTip { Content = $"Notifications enabled." };
                }

            }
            else
            {
                CircleIcon.Source = new BitmapImage(new Uri("Assets/orangecircle48.png", UriKind.Relative));
                CircleIcon.ToolTip = new ToolTip { Content = $"Notifications disabled." };
                timer.Stop();
            }

        }



        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                isRunning = !isRunning;
                button.Content = isRunning ? "Stop" : "Start";
                LogTextBox.AppendText($"{(isRunning ? "Started" : "Stopped")} at {DateTime.Now}\n");
                LogTextBox.ScrollToEnd();
            }
        }
    }
}
