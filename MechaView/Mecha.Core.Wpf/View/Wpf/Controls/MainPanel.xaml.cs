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
    /// Interaction logic for MainPanel.xaml
    /// </summary>
    public partial class MainPanel : StackPanel
    {
        public MainPanel()
        {            
            InitializeComponent();
        }

        public static readonly DependencyProperty HasRootPanels = DependencyProperty.RegisterAttached(
            nameof(HasRootPanels),
            typeof(bool),
            typeof(MainPanel),
            new PropertyMetadata(false));

        public static bool GetHasRootPanels(DependencyObject d)
        {
            return (bool)d.GetValue(HasRootPanels);
        }
        public static void SetHasRootPanels(DependencyObject d, bool value)
        {
            d.SetValue(HasRootPanels, value);
        }

        public bool HasRootPanelControls
        {
            get { return (bool)GetValue(HasRootPanels); }
            set { SetValue(HasRootPanels, value); }
        }
    }
}
