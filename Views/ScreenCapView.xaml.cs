using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using Elixir.Helpers;
using Path = System.IO.Path;

namespace Elixir.Views
{
    public partial class ScreenCapView : UserControl
    {
        private readonly string ScreenshotDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScreenCap");
        private string currentScreenshotPath = "";

        public ScreenCapView()
        {
            InitializeComponent();
            EnsureScreenshotDirectory();
            RefreshDeviceList();
            LoadLatestScreenshot();

            // Set default drawing attributes
            DrawingCanvas.DefaultDrawingAttributes = new DrawingAttributes
            {
                Color = Colors.Red,
                Width = 3,
                Height = 3,
                FitToCurve = true
            };
        }

        private void EnsureScreenshotDirectory()
        {
            if (!Directory.Exists(ScreenshotDirectory))
            {
                Directory.CreateDirectory(ScreenshotDirectory);
            }
        }

        private void RefreshDevices(object sender, RoutedEventArgs e)
        {
            RefreshDeviceList();
        }

        private void RefreshDeviceList()
        {
            DeviceList.Items.Clear();
            string[] devices = AdbHelper.GetConnectedDevices();

            if (devices.Length > 0)
            {
                foreach (var device in devices)
                {
                    DeviceList.Items.Add(device);
                }
                DeviceList.SelectedIndex = 0;
                StatusMessage.Text = $"{devices.Length} device(s) found.";
            }
            else
            {
                StatusMessage.Text = "No devices connected.";
            }
        }

        private void CaptureScreen(object sender, RoutedEventArgs e)
        {
            if (DeviceList.SelectedItem == null)
            {
                StatusMessage.Text = "No device selected.";
                return;
            }

            string deviceId = DeviceList.SelectedItem.ToString();
            string screenshotPath = Path.Combine(ScreenshotDirectory, $"{deviceId}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            StatusMessage.Text = $"Capturing screen from {deviceId}...";

            bool success = AdbHelper.CaptureScreen(deviceId, screenshotPath);
            if (success)
            {
                StatusMessage.Text = $"Screenshot captured successfully! ({screenshotPath})";
                LoadScreenshot(screenshotPath);
            }
            else
            {
                StatusMessage.Text = "Error capturing screen.";
            }
        }

        private void LoadLatestScreenshot()
        {
            if (Directory.Exists(ScreenshotDirectory))
            {
                var files = Directory.GetFiles(ScreenshotDirectory, "*.png")
                                     .OrderByDescending(File.GetCreationTime)
                                     .ToList();

                if (files.Any())
                {
                    LoadScreenshot(files.First());
                }
            }
        }

        private void LoadScreenshot(string filePath)
        {
            if (File.Exists(filePath))
            {
                currentScreenshotPath = filePath;
                BitmapImage bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                ScreenshotImage.Source = bitmap;
            }
        }

        private void OpenColorPicker(object sender, RoutedEventArgs e)
        {
            // カラーダイアログを開く
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 選択した色を InkCanvas のペン色に設定
                DrawingCanvas.DefaultDrawingAttributes.Color = Color.FromArgb(
                    colorDialog.Color.A,
                    colorDialog.Color.R,
                    colorDialog.Color.G,
                    colorDialog.Color.B
                );

                // 選択した色を表示用の Rectangle に反映
                if (sender is Rectangle colorPreview)
                {
                    colorPreview.Fill = new SolidColorBrush(DrawingCanvas.DefaultDrawingAttributes.Color);
                }
            }
        }

        private void SetPenMode(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.EditingMode = InkCanvasEditingMode.Ink;
            DrawingCanvas.DefaultDrawingAttributes.Color = Colors.Red;
            StatusMessage.Text = "Drawing Mode: Pen";
        }

        private void SetEraserMode(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            StatusMessage.Text = "Drawing Mode: Eraser";
        }

        private void ClearCanvas(object sender, RoutedEventArgs e)
        {
            DrawingCanvas.Strokes.Clear();
            StatusMessage.Text = "Canvas Cleared";
        }

        private void UndoLastStroke(object sender, RoutedEventArgs e)
        {
            if (DrawingCanvas.Strokes.Count > 0)
            {
                DrawingCanvas.Strokes.RemoveAt(DrawingCanvas.Strokes.Count - 1);
            }
        }

        private void SaveEditedImage(object sender, RoutedEventArgs e)
        {
            if (ScreenshotImage.Source is BitmapSource originalImage)
            {
                string savePath = Path.Combine(ScreenshotDirectory, $"Edited_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                int width = (int)ScreenshotImage.ActualWidth;
                int height = (int)ScreenshotImage.ActualHeight;

                if (width == 0 || height == 0) return;

                // 背景画像とペイント内容を合成するための RenderTargetBitmap
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                DrawingVisual dv = new DrawingVisual();

                using (DrawingContext dc = dv.RenderOpen())
                {
                    // 背景画像を描画
                    dc.DrawImage(originalImage, new Rect(0, 0, width, height));
                }

                renderBitmap.Render(dv);

                // `InkCanvas` の内容を Bitmap に合成
                RenderTargetBitmap inkCanvasBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                inkCanvasBitmap.Render(DrawingCanvas);

                // 合成用の DrawingVisual
                DrawingVisual finalVisual = new DrawingVisual();
                using (DrawingContext finalDc = finalVisual.RenderOpen())
                {
                    // 背景画像
                    finalDc.DrawImage(renderBitmap, new Rect(0, 0, width, height));
                    // `InkCanvas` の内容をオーバーレイ
                    finalDc.DrawImage(inkCanvasBitmap, new Rect(0, 0, width, height));
                }

                RenderTargetBitmap finalBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                finalBitmap.Render(finalVisual);

                // 画像を保存
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(finalBitmap));
                    encoder.Save(stream);
                }

                StatusMessage.Text = $"Edited image saved successfully! ({savePath})";
            }
        }

    }
}
