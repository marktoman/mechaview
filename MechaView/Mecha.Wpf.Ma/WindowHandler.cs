using System;
using System.Windows;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Mecha.View.Wpf;
using IO = System.IO;
using System.Windows.Input;
using System.Windows.Controls;
using System.Threading.Tasks;
using Mecha.ViewModel;
using Microsoft.Win32;
using Mecha.Wpf.Settings;

namespace Mecha.Wpf.Ma
{
    public class WindowHandler
    {
        ControlManager ctrlMan;
        private readonly MetroWindow window;
        private readonly AppSettings appSettings;

        public WindowHandler(MetroWindow window, AppSettings appSettings)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

            SetWindow();
        }

        void SetWindow()
        {
            if (!window.IsInitialized) throw new InvalidOperationException("Window has not been initialized");

            window.TitleCharacterCasing = CharacterCasing.Normal;
            window.Title = appSettings.Title;

            var dialogs = new Dialogs(ShowProgress, ShowError, ShowInfo, ShowConfirm);

            var settingPath = IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MechaView",
                appSettings.GetType().Assembly.GetName().Name,
                "settings.conf");

            ctrlMan = new ControlManager(appSettings.Content, window, settingPath, dialogs);

            window.DataContext = window.Content;

            var w = appSettings.Window.Width > 0
                ? appSettings.Window.Width
                : 600;
            window.MinWidth = w;
            window.Width = w;

            var h = appSettings.Window.Height > 0
                ? appSettings.Window.Height
                : 500;
            window.MinHeight = h;
            window.Height = h;

            window.Loaded += Window_Loaded;

            window.SizeChanged += Window_SizeChanged;
            window.LayoutUpdated += Window_LayoutUpdated;
        }

        Task<bool> ShowConfirm(string title, string msg, string affirmativeText, string dismissiveText)
        {
            return window.Dispatcher.Invoke(() =>
                window.ShowMessageAsync(
                        title,
                        msg,
                        MessageDialogStyle.AffirmativeAndNegative,
                        new MetroDialogSettings
                        {
                            AffirmativeButtonText = affirmativeText,
                            NegativeButtonText = dismissiveText,
                        })
                    .ContinueWith(x => x.Result == MessageDialogResult.Affirmative));
        }
        Task<IMechaProgress> ShowProgress(string title, string msg, bool isCancelable)
        {
            return window.Dispatcher.Invoke(() =>
                window.ShowProgressAsync(title, msg, isCancelable))
                    .ContinueWith(t => (IMechaProgress)new ProgressDialog(title, msg, isCancelable, t.Result));
        }
        Task ShowError(string title, string msg)
        {
            return window.Dispatcher.Invoke(() => window.ShowMessageAsync(title, msg, MessageDialogStyle.Affirmative));
        }
        Task ShowInfo(string title, string msg)
        {
            return window.Dispatcher.Invoke(() => window.ShowMessageAsync(title, msg, MessageDialogStyle.Affirmative));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ComputeHeight();
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            ComputeHeight();
        }

        void ComputeHeight()
        {
            if (ctrlMan == null) return;

            if (double.IsNaN(appSettings.Window.Height) || appSettings.Window.Height <= 0)
            {
                double titleBarHeight = 30;
                var margin = window.Margin.Top + window.Margin.Bottom;
                var padding = window.Padding.Top + window.Padding.Bottom;

                var h = ctrlMan.ComputeHeight() + margin + padding + titleBarHeight;
                window.MinHeight = h;
                window.Height = h;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ctrlMan.SetChildControlsLabelWidth();

            ComputeHeight();
        }
    }
}
