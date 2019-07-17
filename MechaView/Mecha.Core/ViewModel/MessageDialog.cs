using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel
{
    public class MessageDialog : IMechaMessage
    {
        public string Message { get; set; }
        public string Title { get; set; }
    }
}
