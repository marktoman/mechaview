using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Mecha.ViewModel.Elements
{    
    internal class Container : Element
    {
        object CreateProxy(Type sourceType)
        {
            return new ProxyGenerator().CreateClassProxy(
                sourceType,
                new[] { typeof(INotifyPropertyChanged) },
                interceptor);
        }
        readonly InpcInterceptor interceptor = new InpcInterceptor();

        public Container(Type sourceType) : base(sourceType?.Name)
        {
            Source = CreateProxy(sourceType);
            Elements = ElementBuilder.CreateElements(Source.GetType(), this);
        }
        public Container(Type sourceType, Container parent, string name) : base(parent, name)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            Source = CreateProxy(sourceType);
            Elements = ElementBuilder.CreateElements(Source.GetType(), this);
        }
        public Container(Type sourceType, string name) : base(name)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            Source = CreateProxy(sourceType);
            Elements = ElementBuilder.CreateElements(Source.GetType(), this);
        }

        public object Source { get; }
        public Element[] Elements { get; }
    }
}
