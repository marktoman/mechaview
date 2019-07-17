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
using VM = Mecha.ViewModel.Elements;

namespace Mecha.View.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for TextView.xaml
    /// </summary>
    public partial class TextView : UserControl
    {
        VM.TextView vm;
        internal VM.TextView ViewModel
        {
            get { return vm; }
            set
            {
                vm = value;
                DataContext = vm;

                if (vm.StateAttribute?.Height > 0)
                    Height = vm.StateAttribute.Height;

                label.DataContext = vm;
                label.SetBinding(Label.ContentProperty, new Binding(vm.Path));
            }
        }

        public TextView()
        {
            InitializeComponent();
        }
    }
}
