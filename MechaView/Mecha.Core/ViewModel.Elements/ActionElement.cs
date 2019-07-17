using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mecha.ViewModel;
using Mecha.ViewModel.Attributes;

namespace Mecha.ViewModel.Elements
{
    internal class ActionElement : Element
    {
        public ActionElement(Container parent, MethodInfo method, string name) : base(parent, name)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            this.Method = method;
        }

        public object Invoke(params object[] args)
        {
            return Method.Invoke(Parent.Source, args);
        }
        
        public MethodInfo Method { get; }
        
        public ActionAttribute[] ActionAttributes { get; set; }
    }
}
