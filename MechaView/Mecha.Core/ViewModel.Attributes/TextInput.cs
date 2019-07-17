using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    public class TextInputAttribute : StateAttribute
    {
        public TextInputAttribute() : base() { }
        public TextInputAttribute(string displayName) : base(displayName) { }

        public bool Multiline { get; set; }
    }
}
