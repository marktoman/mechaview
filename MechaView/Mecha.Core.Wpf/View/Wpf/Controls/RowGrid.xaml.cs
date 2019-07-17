using System;
using System.Collections.Generic;
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

namespace Mecha.View.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for RowGrid.xaml
    /// </summary>
    public partial class RowGrid : Grid, ILabeled
    {
        public RowGrid()
        {
            InitializeComponent();
        }

        public double LabelWidth
        {
            get
            {
                return (Children.Cast<UIElement>().FirstOrDefault() as ILabeled)?.LabelWidth ?? 0;
            }
            set
            {
                var il = Children.Cast<UIElement>().FirstOrDefault() as ILabeled;
                if (il != null)
                    il.LabelWidth = value;
            }
        }
    }
}
