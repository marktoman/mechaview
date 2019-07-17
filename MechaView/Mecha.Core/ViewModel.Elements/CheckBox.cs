using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mecha.ViewModel.Elements
{
    internal class CheckBox : StateElement
    {
        public CheckBox(Container parent, PropertyInfo property, string name) : base(parent, property, name) { }
    }
}
