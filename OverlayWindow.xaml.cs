using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Elixir
{
    public partial class OverlayWindow : Window
    {
        public OverlayWindow()
        {
            InitializeComponent();
            this.Loaded += OverlayWindow_Loaded;
        }

        public async void BeginPeekAnimation()
        {
            double windowWidth = this.ActualWidth;
            double windowHeight = this.ActualHeight;

            // Image
            string[] keys = new string[] 
            {
                    "GirlPeeking01", "GirlPeeking02", "GirlPeeking03", "GirlPeeking04", "GirlPeeking05",
                    "GirlPeeking06", "GirlPeeking07", "GirlPeeking08", "GirlPeeking09", "GirlPeeking10"
            };

            Random random = new Random();
            string choiceImg = keys[random.Next(keys.Length)];

            BRImage.Source = (BitmapImage)Application.Current.Resources[choiceImg];
            BLImage.Source = (BitmapImage)Application.Current.Resources[choiceImg];

            // TramslateTransform
            TranslateTransform[] options = new TranslateTransform[] { BRImageTransform, BLImageTransform };
            TranslateTransform choiceTrans = options[random.Next(options.Length)];

            double fromX = 0, toX = 0, fromY = 0, toY = 0;

            if (choiceTrans == BRImageTransform)
            {
                BRImage.Opacity = 1;

                // 右下から右上へ (画像サイズベースに置換)
                fromX = BRImage.ActualWidth * 1.5;  // 右に50%はみ出た位置から
                toX = BRImage.ActualWidth;          // 画面の右端 (反転考慮済み)
                fromY = BRImage.ActualHeight * 1.2; // 下に20%はみ出た位置から
                toY = 0;                            // 画面の右上
            }
            else if (choiceTrans == BLImageTransform)
            {
                BLImage.Opacity = 1;

                // 左下から左上へ (画像サイズベースに置換)
                fromX = -BLImage.ActualWidth * 0.5; // 左に50%分はみ出た位置から
                toX = 0;                            // 画面の左端
                fromY = BLImage.ActualHeight * 1.2; // 下に20%はみ出た位置から
                toY = 0;                            // 画面の左上
            }

            // スライドインとアウトの実行
            await SlideIn(choiceTrans, fromX, toX, fromY, toY);
            await Task.Delay(1000);  // 1秒待機
            await SlideOut(choiceTrans, toX, fromX, toY, fromY);
        }

        private Task SlideIn(TranslateTransform trans, double fromX, double toX, double fromY, double toY)
        {
            var slideInXAnimation = new DoubleAnimation
            {
                From = fromX,
                To = toX,
                Duration = TimeSpan.FromSeconds(1.5),
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.7
            };

            var slideInYAnimation = new DoubleAnimation
            {
                From = fromY,
                To = toY,
                Duration = TimeSpan.FromSeconds(1.5),
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.7
            };

            var tcs = new TaskCompletionSource<bool>();

            int completedCount = 0;
            slideInXAnimation.Completed += (s, e) => { if (++completedCount == 2) tcs.SetResult(true); };
            slideInYAnimation.Completed += (s, e) => { if (++completedCount == 2) tcs.SetResult(true); };

            trans.BeginAnimation(TranslateTransform.XProperty, slideInXAnimation);
            trans.BeginAnimation(TranslateTransform.YProperty, slideInYAnimation);

            return tcs.Task;
        }

        private Task SlideOut(TranslateTransform trans, double fromX, double toX, double fromY, double toY)
        {
            var slideOutXAnimation = new DoubleAnimation
            {
                From = fromX,
                To = toX,
                Duration = TimeSpan.FromSeconds(1.5),
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.7
            };

            var slideOutYAnimation = new DoubleAnimation
            {
                From = fromY,
                To = toY,
                Duration = TimeSpan.FromSeconds(1.5),
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.7
            };

            var tcs = new TaskCompletionSource<bool>();

            int completedCount = 0;
            slideOutXAnimation.Completed += (s, e) => { if (++completedCount == 2) tcs.SetResult(true); };
            slideOutYAnimation.Completed += (s, e) => { if (++completedCount == 2) tcs.SetResult(true); };

            trans.BeginAnimation(TranslateTransform.XProperty, slideOutXAnimation);
            trans.BeginAnimation(TranslateTransform.YProperty, slideOutYAnimation);

            return tcs.Task;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = (int)GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
