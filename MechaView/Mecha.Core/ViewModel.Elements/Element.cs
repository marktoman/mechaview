using Mecha.ViewModel.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mecha.ViewModel.Elements
{
    internal class Element : INotifyPropertyChanged
    {
        public Element(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            IsRootElement = true;
        }
        public Element(Container parent, string name) : this(name)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            
            Parent = parent;
        }

        public bool IsRootElement { get; }
        public Container Parent { get; }
        public string Name { get; }

        string displayName;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                displayName = value;
                OnPropertyChanged();
            }
        }

        public string Description { get; set; }

        public ElementAttribute Attribute { get; set; }

        public string Path
        {
            get
            {
                return Parent != null
                    ? nameof(Parent) +"."+ nameof(Parent.Source) +"."+ Name
                    : null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
