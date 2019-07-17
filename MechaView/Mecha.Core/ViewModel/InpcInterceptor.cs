using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace Mecha.ViewModel
{
    internal class InpcInterceptor : IInterceptor
    {
        PropertyChangedEventHandler eventHandler;

        public void Intercept(IInvocation invocation)
        {
            string methodName = invocation.Method.Name;
            object[] args = invocation.Arguments;
            object proxy = invocation.Proxy;

            if (invocation.Method.DeclaringType.Equals(typeof(INotifyPropertyChanged)))
            {
                if (methodName == "add_PropertyChanged")
                    eventHandler = (PropertyChangedEventHandler)Delegate.Combine(eventHandler, (Delegate)args[0]);
                else if (methodName == "remove_PropertyChanged")
                    eventHandler = (PropertyChangedEventHandler)Delegate.Remove(eventHandler, (Delegate)args[0]);
            }

            if (!ShouldProceedWithInvocation(methodName))
                return;

            invocation.Proceed();

            NotifyPropertyChanged(methodName, proxy);
        }

        protected void OnPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (eventHandler != null)
                eventHandler(sender, e);
        }

        protected void NotifyPropertyChanged(string methodName, object proxy)
        {
            if (methodName.StartsWith("set_"))
            {
                var propName = methodName.Substring(4);
                OnPropertyChanged(proxy, new PropertyChangedEventArgs(propName));
            }
        }

        protected bool ShouldProceedWithInvocation(string methodName)
        {
            return
                methodName != "add_PropertyChanged" &&
                methodName != "remove_PropertyChanged";
        }
    }
}
