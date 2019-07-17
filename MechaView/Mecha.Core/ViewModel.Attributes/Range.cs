using System;
using System.Collections.Generic;
using System.Text;

namespace Mecha.ViewModel.Attributes
{
    public class RangeAttribute : StateAttribute
    {
        public RangeAttribute() { }
        public RangeAttribute(string displayName) : base(displayName) { }

        public double Min { get; set; }
        public double Max { get; set; }
    }
}
