using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mecha.ViewModel.Attributes;

namespace Mecha.ViewModel.Elements
{
    internal class PathInput : TextInput
    {
        public PathInput(Container parent, PropertyInfo property, string name, PathType pathType) : base(parent, property, name)
        {
            PathType = pathType;
        }

        public PathAttribute PathAttribute
        {
            get { return base.StateAttribute as PathAttribute; }
            set { base.StateAttribute = value; }
        }

        public PathType PathType { get; }
    }
}
