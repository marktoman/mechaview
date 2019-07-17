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
using Mecha.Helpers;
using VM = Mecha.ViewModel.Elements;

namespace Mecha.View.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for EnumInput.xaml
    /// </summary>
    public partial class EnumInput : UserControl, ILabeled
    {
        public ComboBoxItem[] Items { get; private set; }

        VM.EnumInput vm;
        internal VM.EnumInput ViewModel
        {
            get { return vm; }
            set
            {
                vm = value;

                Items = EnumHelper.GetComboBoxItems(vm.Property.PropertyType);
                comboBox.ItemsSource = Items;

                DataContext = vm;
                label.DataContext = vm;

                comboBox.DataContext = vm;
                comboBox.SetBinding(
                    ComboBox.SelectedItemProperty,
                    new Binding(vm.Path)
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Converter = new EnumConverter(vm, Items),
                    });
            }
        }

        public double LabelWidth
        {
            get { return label.ActualWidth; }
            set { label.Width = value; }
        }

        public EnumInput()
        {
            InitializeComponent();
        }
    }
    
    internal class EnumConverter : IValueConverter
    {
        readonly VM.EnumInput enumInput;
        readonly ComboBoxItem[] cbItems;
        public EnumConverter(VM.EnumInput enumInput, ComboBoxItem[] cbItems)
        {
            if (enumInput == null)
                throw new ArgumentNullException(nameof(enumInput));
            if (cbItems == null)
                throw new ArgumentNullException(nameof(cbItems));

            this.enumInput = enumInput;
            this.cbItems = cbItems;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return cbItems.Single(x => x.Name == ((Enum)value).Name());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ComboBoxItem)value).Value;
        }
    }

    public class ComboBoxItem
    {
        public string Name { get; set; }
        public Enum Value { get; set; }
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public static class EnumHelper
    {
        public static string Name(this Enum eValue)
        {
            if (eValue == null)
                throw new ArgumentNullException(nameof(eValue));

            return Enum.GetName(eValue.GetType(), eValue);
        }

        /// <summary>
        /// Gets the description of a specific enum value.
        /// </summary>
        public static string DisplayName(this Enum eValue)
        {
            var descAttr = eValue.GetType()
                .GetField(eValue.ToString())
                //TODO: support both DescriptionAttribute (tooltip) and DisplayNameAttribute
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            
            return descAttr?.Description
                ?? Utils.NameToDisplayName(eValue.ToString());
        }

        /// <summary>
        /// Returns an enumerable collection of all values and descriptions for an enum type.
        /// </summary>
        public static ComboBoxItem[] GetComboBoxItems<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            return GetComboBoxItems(typeof(TEnum));
        }

        public static ComboBoxItem[] GetComboBoxItems(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("TEnum must be an Enumeration type");

            return Enum.GetValues(enumType)
                .Cast<object>()
                .Cast<Enum>()
                .Select(e => new ComboBoxItem
                {
                    Name = Enum.GetName(enumType, e),
                    Value = e,
                    DisplayName = e.DisplayName()
                })
                .ToArray();
        }
    }
}
