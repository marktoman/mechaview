using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    public class StateAttribute : ElementAttribute
    {
        public StateAttribute() { }
        public StateAttribute(string displayName) : base(displayName) { }

        public bool Mandatory { get; set; }
        public bool Persistent { get; set; }
        public double Height { get; set; }
    }
}
