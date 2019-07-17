using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for PasswordInput.xaml
    /// </summary>
    public partial class PasswordInput : UserControl, ILabeled
    {
        bool isSetup;

        VM.PasswordInput vm;
        internal VM.PasswordInput ViewModel
        {
            get { return vm; }
            set
            {
                if (isSetup)
                    throw new Exception("ViewModel cannot be set more than once.");
                isSetup = true;

                vm = value;
                DataContext = vm;

                label.DataContext = vm;

                passBox.DataContext = vm;
                //passBox.SetValue(PasswordBoxAssistant.BindPassword, true);
                var binding = new Binding(vm.Path)
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    NotifyOnValidationError = true,
                    ValidatesOnDataErrors = true
                };
                binding.ValidationRules.Add(new MandatoryInputRule(vm)
                {
                });
                passBox.SetBinding(PasswordBoxAssistant.BoundPassword, binding);

                vm.OnValidation += Validate;

                //passBox.Password = vm.Value as string;
                //passBox.PasswordChanged += PassBox_PasswordChanged;
                //(vm.Parent.Source as INotifyPropertyChanged).PropertyChanged += Source_PropertyChanged;
            }
        }

        void Validate(object vm, VM.ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(passBox.Password))
            {
                //FocusManager.SetFocusedElement(passBox.Parent, passBox);
                passBox.Password = " ";
                passBox.Password = null;
            }

            result.HasError = passBox.GetBindingExpression(PasswordBoxAssistant.BoundPassword).HasError;
        }

        //private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    if (passBox.Password != (vm.Value as string))
        //    {
        //        vm.Value = passBox.Password;
        //    }
        //}

        //private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == vm.Property.Name &&
        //        passBox.Password != (vm.Value as string))
        //    {
        //        passBox.Dispatcher.Invoke(() => passBox.Password = vm.Value as string);
        //    }
        //}

        public double LabelWidth
        {
            get { return label.ActualWidth; }
            set { label.Width = value; }
        }

        public PasswordInput()
        {
            InitializeComponent();
        }
    }

    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BoundPassword = DependencyProperty.RegisterAttached(
            nameof(BoundPassword),
            typeof(string),
            typeof(PasswordBoxAssistant),
            new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword = DependencyProperty.RegisterAttached(
            nameof(UpdatingPassword),
            typeof(bool),
            typeof(PasswordBoxAssistant),
            new PropertyMetadata(false));

        static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (PasswordBox)d;

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d == null)
                return;

            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            if (!(bool)box.GetValue(UpdatingPassword))
            {
                box.Password = (string)e.NewValue;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)sender;

            // set a flag to indicate that we're updating the password
            box.SetValue(UpdatingPassword, true);
            // push the new password into the BoundPassword property
            box.SetValue(BoundPassword, box.Password);
            box.SetValue(UpdatingPassword, false);
        }
    }
}
