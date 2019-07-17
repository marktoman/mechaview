using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    public class DateInputAttribute : StateAttribute
    {
        public DateInputAttribute() { }
        public DateInputAttribute(string displayName) : base(displayName) { }
    }
}
