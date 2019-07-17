using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mecha.ViewModel.Elements
{
    internal class SlideInput : StateElement
    {
        public SlideInput(double min, double max, Container parent, PropertyInfo property, string name) : base(parent, property, name)
        {
            Min = min;
            Max = max;
        }

        public double Min { get; }
        public double Max { get; }
    }
}
