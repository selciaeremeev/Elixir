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
    public partial class PreferencesView : UserControl
    {
        private static PreferencesView? _instance;
        public static PreferencesView Instance => _instance ??= new PreferencesView();

        public PreferencesView()
        {
            InitializeComponent();
            SelectedAppearance = "Normal";
        }

        // 選択されたAppearanceを保持
        public string SelectedAppearance { get; private set; }

        // ComboBoxのSelectionChangedイベントハンドラ
        private void AppearanceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppearanceComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedAppearance = selectedItem.Content.ToString();
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAppearance == "Anime Girl")
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null && mainWindow.overlay != null)
                {
                    // OverlayWindowでアニメーションを開始
                    mainWindow.overlay.BeginPeekAnimation();
                }
            }
            else
            {
                // Normalまたはその他の処理
                MessageBox.Show("Normalモードが適用されました。", "適用完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
