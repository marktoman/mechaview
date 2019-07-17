using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mecha.ViewModel.Attributes;

namespace Mecha.ViewModel.Elements
{
    internal class TextInput : StateElement
    {
        public TextInput(Container parent, PropertyInfo property, string name) : base(parent, property, name) { }
    }
}
