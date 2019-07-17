using MahApps.Metro;
using Mecha.Wpf.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Mecha.Wpf.Ma
{
    public class AppHandler
    {
        private readonly Application wpfApp;

        public bool Debug { get; private set; }
        public AppSettings Settings { get; }

        public AppHandler(IApp app, Application wpfApp)
        {
            var s = new AppSettings { Window = new WindowSettings() };
            app.Init(s);

            if (string.IsNullOrWhiteSpace(s.Title))
                throw new ArgumentNullException(nameof(s.Title));
            if (s.Content == null)
                throw new ArgumentNullException(nameof(s.Content));

            if (s.Window == null)
                s.Window = new WindowSettings();

            Settings = s;

#if DEBUG
            Debug = true;
#endif

            wpfApp.DispatcherUnhandledException += App_DispatcherUnhandledException;
            wpfApp.Startup += App_Startup;
            this.wpfApp = wpfApp;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(sender, e.Exception);
        }

        void HandleException(object sender, Exception exception)
        {
            if (!Debug)
                MessageBox.Show(exception.Message, "Error - " + Settings.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show(exception.ToString(), "Error - " + Settings.Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var accent = Settings.Window.Accent.ToString();

            bool useDarkTheme;
            if (Settings.Window.ColorMode != ColorMode.Auto)
            {
                useDarkTheme = Settings.Window.ColorMode == ColorMode.Dark;
            }
            else
            {
                try
                {
                    var useLightThemeStr = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1)?.ToString() ?? "";
                    useDarkTheme = int.TryParse(useLightThemeStr, out int val) && val == 0;
                }
                catch
                {
                    useDarkTheme = false;
                }
            }

            var theme = useDarkTheme ? "BaseDark" : "BaseLight";
            ThemeManager.ChangeAppStyle(wpfApp, ThemeManager.GetAccent(accent), ThemeManager.GetAppTheme(theme));

            foreach (var par in e.Args)
            {
                switch (par)
                {
                    case "--debug":
                        Debug = true;
                        break;
                    default:
                        throw new InvalidOperationException($"Parameter '{par}' doesn't exist. ");
                }
            }
        }
    }
}
