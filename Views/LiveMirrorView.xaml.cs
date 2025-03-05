using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;


namespace Elixir.Views
{
    public partial class LiveMirrorView : UserControl
    {
        private static LiveMirrorView? _instance;
        public static LiveMirrorView Instance => _instance ??= new LiveMirrorView();

        public LiveMirrorView()
        {
            InitializeComponent();
        }
    }
}
