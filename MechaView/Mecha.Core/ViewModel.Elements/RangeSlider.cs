using System.Reflection;
using Mecha.ViewModel.Attributes;

namespace Mecha.ViewModel.Elements
{
    internal class RangeSlider : SlideInput
    {
        public RangeSlider(double min, double max, Container parent, PropertyInfo property, string name) : base(min, max, parent, property, name)
        {

        }

        public RangeAttribute RangeAttribute
        {
            get { return base.StateAttribute as RangeAttribute; }
            set { base.StateAttribute = value; }
        }
    }
}
