using Elixir.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class EasyShellView : UserControl
    {
        private static EasyShellView? _instance;
        public static EasyShellView Instance => _instance ??= new EasyShellView();

        public EasyShellView()
        {
            InitializeComponent();
        }
    }
}
