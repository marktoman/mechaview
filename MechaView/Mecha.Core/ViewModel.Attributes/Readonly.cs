using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    public class ReadonlyAttribute : StateAttribute
    {
        public ReadonlyAttribute() : base() { }
        public ReadonlyAttribute(string displayName) : base(displayName) { }
    }
}
