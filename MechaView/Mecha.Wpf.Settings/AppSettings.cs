using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.Wpf.Settings
{
    public class AppSettings
    {
        public WindowSettings Window { get; set; }
        public string Title { get; set; }
        public Type Content { get; set; }
    }
}
