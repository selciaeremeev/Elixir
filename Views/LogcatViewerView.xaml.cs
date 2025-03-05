using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Elixir.Helpers;
using Elixir.Models;
using Microsoft.Win32;

namespace Elixir.Views
{
    public partial class LogcatViewerView : UserControl
    {
        private static LogcatViewerView? _instance;
        public static LogcatViewerView Instance => _instance ??= new LogcatViewerView();

        private bool IsStart = false;
        private Process? logcatProcess;
        private string currentDeviceId = string.Empty;
        private readonly ObservableCollection<LogEntry> logEntries = new();
        private ICollectionView logView;

        public LogcatViewerView()
        {
            InitializeComponent();
            LogcatDataGrid.Items.Clear();
            logView = CollectionViewSource.GetDefaultView(logEntries);
            LogcatDataGrid.ItemsSource = logEntries;

            var deviceComboBox = DeviceSelectorControl?.FindName("DeviceComboBox") as ComboBox;
            if (deviceComboBox != null)
            {
                deviceComboBox.SelectionChanged += DeviceComboBox_SelectionChanged;

                if (deviceComboBox.SelectedItem != null)
                {
                    currentDeviceId = deviceComboBox.SelectedItem.ToString() ?? string.Empty;
                }
            }
        }

        // デバイス選択時の処理
        private void DeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem != null)
            {
                currentDeviceId = comboBox.SelectedItem.ToString() ?? string.Empty;
            }
        }

        private async void ToggleLogcat_Click(object sender, RoutedEventArgs e)
        {
            if (IsStart)
            {
                StopLogcat();
                IsStart = false;
                ToggleLogcatButtonIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.Play48;
                ToggleLogcatButtonIcon.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            else
            {
                if (string.IsNullOrEmpty(currentDeviceId))
                {
                    MessageBox.Show("Please select a device.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                logEntries.Clear();
                StopLogcat();

                logcatProcess = AdbHelper.StartLogcat(currentDeviceId);

                if (logcatProcess == null)
                {
                    MessageBox.Show("Failed to start Logcat.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                logcatProcess.OutputDataReceived += (_, ea) =>
                {
                    if (!string.IsNullOrEmpty(ea.Data))
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (LogEntry.Parse(ea.Data) is LogEntry logEntry)
                            {
                                logEntries.Add(logEntry);
                            }
                        }));
                    }
                };

                IsStart = true;
                ToggleLogcatButtonIcon.Symbol = Wpf.Ui.Controls.SymbolRegular.Stop16;
                ToggleLogcatButtonIcon.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private async void StopLogcat()
        {
            if (logcatProcess != null)
            {
                try
                {
                    if (!logcatProcess.HasExited)
                    {
                        await Task.Delay(200);
                        logcatProcess.Kill();
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine($"The process has already exited: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while stopping Logcat: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    logcatProcess.Dispose();
                    logcatProcess = null;
                }
            }
        }

        private void ClearLogcat_Click(object sender, RoutedEventArgs e)
        {
            logEntries.Clear();
            AdbHelper.ClearLogcat(currentDeviceId);
        }

        private void SaveLogcat_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Log Files (*.log)|*.log|All Files (*.*)|*.*",
                DefaultExt = ".log",
                FileName = "logcat_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".log"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine("#\tDATE\tPID\tTID\tLEVEL\tTAG\tMESSAGE");

                        int index = 1;
                        foreach (var entry in logEntries)
                        {
                            writer.WriteLine($"{index}\t{entry.Date}\t{entry.PID}\t{entry.TID}\t{entry.Level}\t{entry.Tag}\t{entry.Message}");
                            index++;
                        }
                    }
                    MessageBox.Show("Logs have been saved successfully.", "Save Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LogcatDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchTextBox.Text.ToLower();

            if (string.IsNullOrEmpty(filter))
            {
                logView.Filter = null;
            }
            else
            {
                logView.Filter = item =>
                {
                    if (item is LogEntry entry)
                    {
                        return entry.Date.ToLower().Contains(filter) ||
                               entry.PID.ToLower().Contains(filter) ||
                               entry.TID.ToLower().Contains(filter) ||
                               entry.Level.ToLower().Contains(filter) ||
                               entry.Tag.ToLower().Contains(filter) ||
                               entry.Message.ToLower().Contains(filter);
                    }
                    return false;
                };
            }
        }
    }
}
