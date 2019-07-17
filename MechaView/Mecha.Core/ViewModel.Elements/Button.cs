using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mecha.ViewModel.Elements
{
    internal class Button : ActionElement
    {
        public Button(Container parent, MethodInfo method, string name) : base(parent, method, name) { }
    }
}
