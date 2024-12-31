using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PoE2_Whisper
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            txtLogLocation.Text = Properties.Settings.Default.PoEClientLogLocation;
            txtWhisperRegex.Text = Properties.Settings.Default.WhisperRegEx;
            txtPushBulletToken.Text = Properties.Settings.Default.PushBulletToken;
            txtPushBulletDeviceName.Text = Properties.Settings.Default.PushBulletDeviceName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update settings with values from text boxes
            Properties.Settings.Default.PoEClientLogLocation = txtLogLocation.Text;
            Properties.Settings.Default.WhisperRegEx = txtWhisperRegex.Text;
            Properties.Settings.Default.PushBulletToken = txtPushBulletToken.Text;
            Properties.Settings.Default.PushBulletDeviceName = txtPushBulletDeviceName.Text;

            // Save the changes
            Properties.Settings.Default.Save();

            // Optionally, close the settings window after saving
            this.Close();

            //Or show a message
            MessageBox.Show("Settings saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
