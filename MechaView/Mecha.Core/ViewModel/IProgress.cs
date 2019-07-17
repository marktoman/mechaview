using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel
{
    public interface IMechaProgress
    {
        string Message { get; set; }
        string Title { get; set; }
        double Progress { get; set; }
        bool IsIndeterminate { get; set; }
        bool IsCancelable { get; set; }
        bool IsCanceled { get; }

        bool IsOpen { get; }

        Task CloseAsync();
    }
}
