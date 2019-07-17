using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using Mecha.ViewModel.Attributes;
using VM = Mecha.ViewModel.Elements;

namespace Mecha.View.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : UserControl, ILabeled
    {
        bool isSetup;

        VM.TextInput vm;
        internal VM.TextInput ViewModel
        {
            get { return vm; }
            set
            {
                if (isSetup)
                    throw new InvalidOperationException("ViewModel can be set only once.");
                isSetup = true;

                vm = value;

                vm.OnValidation += Validate;
                DataContext = vm;
                label.DataContext = vm;
                txtBox.DataContext = vm;

                var attrib = vm.Attribute as TextInputAttribute;
                if (attrib?.Multiline == true)
                {
                    txtBox.TextWrapping = TextWrapping.Wrap;
                    txtBox.AcceptsReturn = true;
                }

                var binding = new Binding(vm.Path)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnExceptions = true,
                };
                binding.ValidationRules.Add(new MandatoryInputRule(vm));
                txtBox.SetBinding(TextBox.TextProperty, binding);
            }
        }

        void Validate(object vm, VM.ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(txtBox.Text))
                txtBox.Text = null;
            
            result.HasError = txtBox.GetBindingExpression(TextBox.TextProperty).HasError;
        }

        public double LabelWidth
        {
            get { return label.ActualWidth; }
            set { label.Width = value; }
        }

        public TextInput()
        {
            InitializeComponent();
        }
    }
}
