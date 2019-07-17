using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Elements
{
    internal class TableView : StateElement
    {
        public TableView(Type itemType, Container parent, PropertyInfo property, string name)
            : base(parent, property, name)
        {
            if (itemType == null)
                throw new ArgumentNullException(nameof(itemType));

            ItemType = itemType;
        }

        public Type ItemType { get; }
    }
}
