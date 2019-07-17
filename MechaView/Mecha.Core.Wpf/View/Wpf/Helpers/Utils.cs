using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mecha.View.Wpf.Helpers
{
    internal static class Utils
    {
        public static Size MeasureString(this Label ctrl)
        {
            if (string.IsNullOrEmpty(ctrl.Content?.ToString()))
                return new Size();

            return MeasureString(ctrl, ctrl.Content.ToString());
        }
        public static Size MeasureString(this Label ctrl, string candidate)
        {
            var ft = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(ctrl.FontFamily, ctrl.FontStyle, ctrl.FontWeight, ctrl.FontStretch),
                ctrl.FontSize,
                Brushes.Black);

            return new Size(ft.Width, ft.Height);
        }
    }
}
