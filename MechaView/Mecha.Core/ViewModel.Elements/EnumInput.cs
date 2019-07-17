using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mecha.Helpers;

namespace Mecha.ViewModel.Elements
{
    internal class EnumInput : StateElement
    {
        public EnumInput(Container parent, PropertyInfo property, string name) : base(parent, property, name) { }
    }
}
