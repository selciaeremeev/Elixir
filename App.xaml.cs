using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Elixir;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow window = new MainWindow();
        window.Icon = new BitmapImage(new Uri("pack://application:,,,/Assets/Icons/Icon.ico"));
        window.Show();
    }
}

