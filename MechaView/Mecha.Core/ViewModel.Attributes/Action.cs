using System;
using System.Collections.Generic;
using System.Text;

namespace Mecha.ViewModel.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ActionAttribute : Attribute
    {
        public ActionAttribute() { }
        public bool DisableValidation { get; set; }
    }
}
