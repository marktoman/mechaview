using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mecha.ViewModel.Elements;

namespace Mecha.ViewModel
{
    internal class Validation
    {
        public IEnumerable<object> ValidateStateChildren(Container container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            var states = container.Elements
                .OfType<StateElement>()
                .Where(x => x.StateAttribute?.Mandatory == true);

            foreach (var se in states)
            {
                var t = se.Property.PropertyType;

                bool isValid =
                    t == typeof(string)
                        ? !string.IsNullOrWhiteSpace((string)se.Value) :
                    t.IsValueType
                        ? se.Value != Activator.CreateInstance(t)
                        : se.Value != null;

                yield return new
                {
                    IsValid = isValid,
                    StateElement = se
                };
            }
        }
    }
}
