using System;
using System.Collections.Generic;
using System.Text;

namespace Mecha.Exceptions
{
    public class ControlNotFoundException : Exception
    {
        public ControlNotFoundException(string message) : base(message) { }
    }
}
