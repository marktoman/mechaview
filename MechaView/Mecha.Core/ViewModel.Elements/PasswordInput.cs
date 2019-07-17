using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mecha.ViewModel.Attributes;

namespace Mecha.ViewModel.Elements
{
    internal class PasswordInput : StateElement
    {
        public PasswordInput(Container parent, PropertyInfo property, string name) : base(parent, property, name) { }

        public PasswordAttribute PasswordAttribute
        {
            get { return base.Attribute as PasswordAttribute; }
            set { base.Attribute = value; }
        }
    }
}
