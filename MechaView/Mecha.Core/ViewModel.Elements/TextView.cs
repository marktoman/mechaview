using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mecha.ViewModel.Elements
{
    internal class TextView : StateElement
    {
        public TextView(Container parent, PropertyInfo property, string name) : base(parent, property, name) { }
    }
}
