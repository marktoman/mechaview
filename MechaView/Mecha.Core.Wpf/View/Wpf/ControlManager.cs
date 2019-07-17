using System;
using System.Collections.Generic;
using System.Text;
using Mecha.View;
using Ctrl = System.Windows.Controls;
using Win = System.Windows;
using System.Reflection;
using System.Windows.Data;
using Mecha.ViewModel;
using System.Linq;
using System.Threading.Tasks;
using MechaCtrl = Mecha.View.Wpf.Controls;
using VM = Mecha.ViewModel.Elements;
using SettingManagement;
using System.Security.Cryptography;

namespace Mecha.View.Wpf
{
    public class ControlManager
    {
        readonly Ctrl.ContentControl contentControl;
        readonly string settingPath;
        readonly VM.Container appForm;

        public ControlManager(
            Type appFormType,
            Ctrl.ContentControl contentControl,
            string settingPath,
            Dialogs dialogs)
        {
            if (dialogs == null)
                throw new ArgumentNullException(nameof(dialogs));
            if (contentControl == null)
                throw new ArgumentNullException(nameof(contentControl));
            if (string.IsNullOrWhiteSpace(settingPath))
                throw new ArgumentNullException(nameof(settingPath));

            this.contentControl = contentControl;
            this.settingPath = settingPath;

            appForm = new VM.Container(appFormType);

            Load();

            var ctrlBuilder = new ControlBuilder(dialogs);

            var mainPanel = ctrlBuilder.CreateControls(appForm.Elements);

            contentControl.Content = mainPanel;
        }

        static IEnumerable<KeyValuePair<string, VM.StateElement>> GetPersistentElements(IEnumerable<VM.Element> elements, int lvl = 0)
        {
            foreach (var e in elements)
            {
                var se = e as VM.StateElement;
                if (se?.StateAttribute?.Persistent == true)
                {
                    yield return new KeyValuePair<string, VM.StateElement>(se.Name, se);
                }

                var container = e as VM.Container;
                if (container != null)
                {
                    foreach (var x in GetPersistentElements(container.Elements, lvl + 1))
                        yield return x;
                }
            }
        }

        public void Load()
        {
            try
            {
                var vals = SettingManager.Load(settingPath);
                var pes = GetPersistentElements(appForm.Elements).ToDictionary(x => x.Key, x => x.Value);
                var peVals = vals.Join(pes,
                    x => x.Key,
                    x => x.Key,
                    (x, y) => new { x.Key, x.Value, Element = y.Value });

                foreach (var x in peVals)
                {
                    if (x.Element.GetType() == typeof(VM.PasswordInput))
                    {
                        var pass = x.Value as string;
                        if (pass != null)
                            x.Element.Value = Decrypt(pass);
                    }
                    else
                    {
                        x.Element.Value = x.Value;
                    }
                }
            }
            catch { }
        }
        public void Save()
        {
            try
            {
                var vals = GetPersistentElements(appForm.Elements)
                    .Select(x => new SettingItem(
                            name: x.Key,
                            type: x.Value.Property.PropertyType,
                            value: x.Value.GetType() == typeof(VM.PasswordInput) &&
                                    !string.IsNullOrEmpty(x.Value.Value as string)
                                ? Encrypt(x.Value.Value as string)
                                : x.Value.Value));

                SettingManager.Save(settingPath, vals);
            }
            catch { }
        }

        public void SetChildControlsLabelWidth()
        {
            SetChildControlsLabelWidth(contentControl.Content as MechaCtrl.MainPanel);
        }
        void SetChildControlsLabelWidth(Win.FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (element is MechaCtrl.RowGrid)
                throw new Exception(nameof(MechaCtrl.RowGrid) + " is a single-row container.");

            var ctrls = (element as Ctrl.Panel)?.Children.Cast<Win.UIElement>().ToArray();
            Win.UIElement[] elements;

            if (ctrls != null)
            {
                var labeled = ctrls.OfType<MechaCtrl.ILabeled>().ToArray();
                if (labeled.Length > 0)
                {
                    double lw = labeled.Max(x => x.LabelWidth);
                    foreach (var x in labeled)
                        x.LabelWidth = lw;
                }

                elements = ctrls.Except(labeled.Select(x => x as Win.UIElement)).ToArray();
            }
            else
            {
                elements = new[] { element };
            }

            foreach (var e in elements)
            {
                if (e is Ctrl.ContentControl)
                {
                    var c = (e as Ctrl.ContentControl).Content as Win.FrameworkElement;
                    if (c != null)
                        SetChildControlsLabelWidth(c);
                }
                else if (e is Ctrl.Panel && !(e is MechaCtrl.RowGrid))
                {
                    SetChildControlsLabelWidth(e as Ctrl.Panel);
                }
            }
        }

        public double ComputeHeight()
        {
            return ComputeHeight(contentControl.Content as MechaCtrl.MainPanel);
        }
        double ComputeHeight(Win.FrameworkElement e)
        {
            var margin = e.Margin.Top + e.Margin.Bottom;

            var p = e as Ctrl.StackPanel; //not WrapPanel
            if (p != null)
            {
                return p.Children
                    .OfType<Win.FrameworkElement>()
                    .Sum(x => ComputeHeight(x)) + margin;
            }
            return e.ActualHeight + margin;
        }

        string Encrypt(string value)
        {
            var ba = ProtectedData.Protect(
                Encoding.UTF8.GetBytes(value),
                Encoding.UTF8.GetBytes("QhJuh44BuHgf4PeWsxL3gSPL"),
                DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(ba);
        }
        string Decrypt(string value)
        {
            var ba = ProtectedData.Unprotect(
                Convert.FromBase64String(value),
                Encoding.UTF8.GetBytes("QhJuh44BuHgf4PeWsxL3gSPL"),
                DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(ba);
        }
    }
}
