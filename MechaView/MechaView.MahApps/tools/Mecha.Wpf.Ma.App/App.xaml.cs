using Mecha.Wpf.Settings;
using System.Windows;

namespace Mecha.Wpf.Ma.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly AppHandler handler;
        public AppSettings Settings => handler.Settings;

        public App()
        {
            IApp app = new global::App();
            handler = new AppHandler(app, this);
        }
    }
}
