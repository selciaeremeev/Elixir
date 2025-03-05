using Elixir.Views;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Wpf.Ui.Animations;

namespace Elixir;

public partial class MainWindow : Window
{
    public OverlayWindow overlay;

    public MainWindow()
    {
        InitializeComponent();
        this.StateChanged += MainWindow_StateChanged;
        this.Loaded += MainWindow_Loaded;
        this.LocationChanged += MainWindow_LocationChanged;
        this.SizeChanged += MainWindow_SizeChanged;
        MainContentFrame.Navigated += MainContentFrame_Navigated;
        MainContentFrame.Navigate(ScreenCaptureView.Instance);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        overlay = new OverlayWindow
        {
            Owner = this
        };
        overlay.Show();
        UpdateOverlayPositionAndSize();
    }

    private void MainWindow_StateChanged(object sender, System.EventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            WindowGrid.Margin = new Thickness(7);
            ToggleMaximizeText.Text = "\uE923";
        }
        else if (this.WindowState == WindowState.Normal)
        {
            WindowGrid.Margin = new Thickness(0);
            ToggleMaximizeText.Text = "\uE922";
        }

        if (overlay != null)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    overlay.WindowState = WindowState.Maximized;
                    break;
                case WindowState.Normal:
                    overlay.WindowState = WindowState.Normal;
                    UpdateOverlayPositionAndSize(); // 通常時は位置とサイズを更新
                    break;
            }
        }
    }

    private void ToggleMaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
        }
        else
        {
            this.WindowState = WindowState.Normal;
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void NavigateToHome(object sender, RoutedEventArgs e)
    {
        MainContentFrame.Navigate(HomeView.Instance);
    }

    private void NavigateToEasyShell(object sender, RoutedEventArgs e)
    {
        MainContentFrame.Navigate(EasyShellView.Instance);
    }

    private void NavigateToLogcatViewer(object sender, RoutedEventArgs e)
    {
        MainContentFrame.Navigate(LogcatViewerView.Instance);
    }

    private void NavigateToLiveMirror(object sender, RoutedEventArgs e)
    {
        MainContentFrame.Navigate(LiveMirrorView.Instance);
    }

    private void NavigateToScreenCapture(object sender, RoutedEventArgs e)
    {
        MainContentFrame.Navigate(ScreenCaptureView.Instance);
    }

    private void NavigateToPreferences(object sender, RoutedEventArgs e)
    {
        MainContentFrame.Navigate(PreferencesView.Instance);
    }

    private void MainContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        var currentView = MainContentFrame.Content as FrameworkElement;
        ClearButtonEffects();

        if (currentView != null)
        {
            var fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300)))
            {
                EasingFunction = new QuadraticEase()
            };
            currentView.BeginAnimation(FrameworkElement.OpacityProperty, fadeInAnimation);
        }

        if (currentView is HomeView)
        {
            HomeButton.Foreground = new SolidColorBrush(Colors.White);
            HomeButtonText.FontFamily = (FontFamily)FindResource("InterBold");
            ShowBorderWithAnimation(HomeButtonBorder);
        }
        else if (currentView is EasyShellView)
        {
            EasyShellButton.Foreground = new SolidColorBrush(Colors.White);
            EasyShellButtonText.FontFamily = (FontFamily)FindResource("InterBold");
            ShowBorderWithAnimation(EasyShellButtonBorder);
        }
        else if (currentView is LogcatViewerView)
        {
            LogcatViewerButton.Foreground = new SolidColorBrush(Colors.White);
            LogcatViewerButtonText.FontFamily = (FontFamily)FindResource("InterBold");
            ShowBorderWithAnimation(LogcatViewerButtonBorder);
        }
        else if (currentView is LiveMirrorView)
        {
            LiveMirrorButton.Foreground = new SolidColorBrush(Colors.White);
            LiveMirrorButtonText.FontFamily = (FontFamily)FindResource("InterBold");
            ShowBorderWithAnimation(LiveMirrorButtonBorder);
        }
        else if (currentView is ScreenCaptureView)
        {
            ScreenCaptureButton.Foreground = new SolidColorBrush(Colors.White);
            ScreenCaptureButtonText.FontFamily = (FontFamily)FindResource("InterBold");
            ShowBorderWithAnimation(ScreenCaptureButtonBorder);
        }
        else if (currentView is PreferencesView)
        {
            PreferencesButton.Foreground = new SolidColorBrush(Colors.White);
            PreferencesButtonText.FontFamily = (FontFamily)FindResource("InterBold");
            ShowBorderWithAnimation(PreferencesButtonBorder);
        }
    }

    private void ShowBorderWithAnimation(Border target)
    {
        target.Visibility = Visibility.Visible;
        target.Background = new SolidColorBrush(Colors.White);
        TransitionAnimationProvider.ApplyTransition(target, Transition.SlideLeft, 300);
    }

    private void ClearButtonEffects()
    {
        SolidColorBrush LightGray = new(Color.FromArgb(255, 240, 240, 240));

        HomeButton.Foreground = LightGray;
        EasyShellButton.Foreground = LightGray;
        LogcatViewerButton.Foreground = LightGray;
        LiveMirrorButton.Foreground = LightGray;
        ScreenCaptureButton.Foreground = LightGray;
        PreferencesButton.Foreground = LightGray;

        FontFamily InterRegular = (FontFamily)FindResource("InterRegular");

        HomeButtonText.FontFamily = InterRegular;
        EasyShellButtonText.FontFamily = InterRegular;
        LogcatViewerButtonText.FontFamily = InterRegular;
        LiveMirrorButtonText.FontFamily = InterRegular;
        ScreenCaptureButtonText.FontFamily = InterRegular;
        PreferencesButtonText.FontFamily = InterRegular;

        HomeButtonBorder.Visibility = Visibility.Collapsed;
        EasyShellButtonBorder.Visibility = Visibility.Collapsed;
        LogcatViewerButtonBorder.Visibility = Visibility.Collapsed;
        LiveMirrorButtonBorder.Visibility = Visibility.Collapsed;
        ScreenCaptureButtonBorder.Visibility = Visibility.Collapsed;
        PreferencesButtonBorder.Visibility = Visibility.Collapsed;
    }

    // MainWindow の位置が変わったときに OverlayWindow も移動
    private void MainWindow_LocationChanged(object sender, EventArgs e)
    {
        if (overlay != null)
        {
            UpdateOverlayPositionAndSize();
        }
    }

    // MainWindow のサイズが変わったときに OverlayWindow もリサイズ
    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (overlay != null)
        {
            UpdateOverlayPositionAndSize();
        }
    }

    private void UpdateOverlayPositionAndSize()
    {
        var mainWindowPos = this.PointToScreen(new Point(0, 0));
        overlay.Left = mainWindowPos.X;
        overlay.Top = mainWindowPos.Y;
        overlay.Width = this.ActualWidth;
        overlay.Height = this.ActualHeight;

        Debug.WriteLine($"Overlay位置: {overlay.Left}, {overlay.Top} サイズ: {overlay.Width}x{overlay.Height}");
    }
}