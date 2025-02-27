using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using Elixir.Views;

namespace Elixir
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SourceInitialized += (s, e) =>
            {
                WindowChrome.SetWindowChrome(this, new WindowChrome
                {
                    CaptionHeight = SystemParameters.CaptionHeight,
                    ResizeBorderThickness = SystemParameters.WindowResizeBorderThickness,
                    GlassFrameThickness = new Thickness(0, 0, 0, 1)
                });
            };
            this.StateChanged += MainWindow_StateChanged;
            MainFrame.Navigate(new ScreenCapView());
        }

        // タイトルバーのドラッグ移動
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // 最小化
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // 最大化・通常化
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        // ウィンドウの状態変更時の処理（アイコンを変更）
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.Margin = new Thickness(9);
                MaximizeButton.Content = "\uE923"; // 縮小アイコン
            }
            else
            {
                 this.Padding = new Thickness(0);
                MaximizeButton.Content = "\uE922"; // 最大化アイコン
            }
        }

        // ウィンドウを閉じる
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NavigateToScreenCap(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ScreenCapView());
        }

        private void NavigateToScreenRec(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ScreenRecView());
        }

        private void NavigateToSettings(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsView());
        }
    }
}
