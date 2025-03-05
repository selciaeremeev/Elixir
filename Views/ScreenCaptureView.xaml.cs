using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Elixir.Helpers;
using Path = System.IO.Path;

namespace Elixir.Views
{
    public partial class ScreenCaptureView : UserControl
    {
        private static ScreenCaptureView? _instance;
        public static ScreenCaptureView Instance => _instance ??= new ScreenCaptureView();

        private readonly string _screenshotDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screen Capture");
        private string _currentScreenshotPath = "";

        public ScreenCaptureView()
        {
            InitializeComponent();
            EnsureScreenshotDirectory();
            LoadLatestScreenshot();
        }

        private void EnsureScreenshotDirectory()
        {
            Directory.CreateDirectory(_screenshotDirectory);
        }

        private void CaptureScreen(object sender, RoutedEventArgs e)
        {
            var deviceComboBox = DeviceSelectorControl?.DeviceComboBoxControl;
            if (deviceComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a device first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string deviceId = deviceComboBox.SelectedItem.ToString();
            string screenshotPath = Path.Combine(_screenshotDirectory, $"{deviceId}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            try
            {
                if (AdbHelper.CaptureScreen(deviceId, screenshotPath))
                {
                    LoadScreenshot(screenshotPath);
                }
                else
                {
                    MessageBox.Show("Failed to capture screenshot.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error capturing screenshot: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLatestScreenshot()
        {
            var files = Directory.Exists(_screenshotDirectory)
                ? Directory.GetFiles(_screenshotDirectory, "*.png").OrderByDescending(File.GetCreationTime).ToList()
                : new();

            if (files.Any())
            {
                try
                {
                    LoadScreenshot(files.First());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load the latest screenshot: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ScreenshotImage.Source = null;  // エラー時は何も表示しない
                }
            }
            else
            {
                ScreenshotImage.Source = null;  // スクリーンショットがなければ何も表示しない
            }
        }

        private void LoadScreenshot(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    ScreenshotImage.Source = null;  // ファイルがない場合は空
                    return;
                }

                _currentScreenshotPath = filePath;

                // 非ロックで読み込む
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        fileStream.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = memoryStream;
                        bitmap.EndInit();
                        bitmap.Freeze();  // UIスレッド以外でも使えるようにFreeze

                        ScreenshotImage.Source = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading screenshot: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ScreenshotImage.Source = null;  // エラー時は何も表示しない
            }
        }
    }
}
