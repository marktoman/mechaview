using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GroupAttribute : Attribute
    {
        public GroupAttribute(bool invisible)
        {
            Invisible = invisible;
        }
        public GroupAttribute(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentNullException();

            DisplayName = displayName;
            Invisible = false;
        }
        public string DisplayName { get; }

        public bool Invisible { get; }
    }
}
