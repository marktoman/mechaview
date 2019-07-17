using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Mecha.ViewModel.Attributes;

namespace Mecha.ViewModel.Elements
{
    internal class StateElement : Element
    {
        public PropertyInfo Property { get; }

        public StateElement(Container parent, PropertyInfo property, string name) : base(parent, name)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            this.Property = property;
        }
        
        public object Value
        {
            get { return Property.GetValue(Parent.Source, null); }
            set { Property.SetValue(Parent.Source, value, null); }
        }

        public StateAttribute StateAttribute
        {
            get { return base.Attribute as StateAttribute; }
            set { base.Attribute = value; }
        }

        public bool Validate()
        {
            if (OnValidation == null)
                return true;

            var res = new ValidationResult();

            OnValidation(this, res);

            return !res.HasError;
        }
        public event EventHandler<ValidationResult> OnValidation;
    }

    public class ValidationResult
    {
        public bool HasError { get; set; }
    }
}
