using Mecha.Wpf.Settings;
using System.Windows;

namespace Mecha.Wpf.Ma.Dev
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
            IApp app = new global::MechaApp();
            handler = new AppHandler(app, this);
        }
    }
}
