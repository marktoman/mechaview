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
    /// Interaction logic for DateInput.xaml
    /// </summary>
    public partial class DateInput : UserControl, ILabeled
    {
        public DateInput()
        {
            InitializeComponent();
        }

        bool isSetup;

        VM.DateInput vm;
        internal VM.DateInput ViewModel
        {
            get { return vm; }
            set
            {
                if (isSetup)
                    throw new InvalidOperationException("ViewModel can be set only once.");
                isSetup = true;

                vm = value;

                DataContext = vm;
                label.DataContext = vm;
                dateBox.DataContext = vm;

                var binding = new Binding(vm.Path)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnExceptions = true,
                };
                dateBox.SetBinding(DatePicker.SelectedDateProperty, binding);

                //if (vm == null || vm.Value == default)
                //    dateBox.SelectedDate = DateTime.Now;
            }
        }

        public double LabelWidth
        {
            get { return label.ActualWidth; }
            set { label.Width = value; }
        }
    }
}
