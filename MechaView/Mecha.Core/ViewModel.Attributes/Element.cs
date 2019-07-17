using System;
using System.Collections.Generic;
using System.Text;

namespace Mecha.ViewModel.Attributes
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class ElementAttribute : Attribute
    {
        public ElementAttribute() { }
        public ElementAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// A value of a user defined enum 
        /// </summary>
        public object Group { get; set; }
        public double Position { get; set; } = -1;
        public int Width { get; set; } = -1;
    }
}
