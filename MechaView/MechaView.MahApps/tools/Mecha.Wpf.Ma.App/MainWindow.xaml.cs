using MahApps.Metro.Controls;
using System.Windows;

namespace Mecha.Wpf.Ma.App
{
    public partial class MainWindow : MetroWindow
    {
        readonly WindowHandler handler;

        public MainWindow()
        {
            InitializeComponent();

            var wpfApp = (App)Application.Current;
            handler = new WindowHandler(this, wpfApp.Settings);
        }
    }
}
