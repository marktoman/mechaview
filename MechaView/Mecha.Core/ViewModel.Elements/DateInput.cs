using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Elements
{
    internal class DateInput : StateElement
    {
        public DateInput(Container parent, PropertyInfo property, string name) : base(parent, property, name) { }
    }
}
