using Elixir.Helpers;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Elixir.Views
{
    public partial class DeviceSelector : UserControl
    {
        public DeviceSelector()
        {
            InitializeComponent();
            RefreshDeviceComboBox();
        }

        public ComboBox DeviceComboBoxControl => DeviceComboBox;

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDeviceComboBox();
        }

        private void RefreshDeviceComboBox()
        {
            DeviceComboBox.Items.Clear();
            try
            {
                var devices = AdbHelper.GetConnectedDevices();
                if (devices.Any())
                {
                    foreach (var device in devices) DeviceComboBox.Items.Add(device);
                    DeviceComboBox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No devices connected.", "Device Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing devices: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
