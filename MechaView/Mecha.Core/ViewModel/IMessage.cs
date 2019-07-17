using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel
{
    public interface IMechaMessage
    {
        string Title { get; set; }
        string Message { get; set; }
    }
}
