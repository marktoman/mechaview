using Mecha.ViewModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
    public class ElementDecoration
    {
        [Readonly("Status Label")]
        public virtual string Status { get; set; }

        [TextInput(Persistent = true)]
        public virtual string User { get; set; }

        [Password(Persistent = true)]
        public virtual string Pass { get; set; }

        [TextInput(Persistent = true, Multiline = true)]
        public virtual string Description { get; set; }

        [DateInput(Persistent = true)]
        public virtual DateTime Start { get; set; } = DateTime.Now;

        [Path("txt", "md", Type = PathType.Open, Persistent = true)]
        public virtual string File { get; set; }
    }
}
