using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    public class PasswordAttribute : StateAttribute
    {
        public PasswordAttribute() : base() { }
        public PasswordAttribute(string displayName) : base(displayName) { }
    }
}
