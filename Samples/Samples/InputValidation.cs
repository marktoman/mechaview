using Mecha.ViewModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
    public class InputValidation
    {
        [TextInput(Mandatory = true, Description = "User name")]
        public virtual string User { get; set; }

        public void BoundAction() { }

        [Action(DisableValidation = true)]
        public void UnboundAction() { }
    }
}
