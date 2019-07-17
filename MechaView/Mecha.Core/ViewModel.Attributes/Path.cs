using System;
using System.Collections.Generic;
using System.Text;

namespace Mecha.ViewModel.Attributes
{
    public enum PathType { Auto, Open, Save, Folder }

    [AttributeUsage(AttributeTargets.Property)]
    public class PathAttribute : StateAttribute
    {
        public PathAttribute(PathType type, params string[] ext)
        {
            Type = type;
        }
        public PathAttribute(params string[] ext)
        {
            Type = PathType.Auto;
            Ext = ext;
        }

        public string[] Ext { get; }

        public bool All { get; set; }
        
        public PathType Type { get; set; }
        public string FileName { get; set; }
    }
}
