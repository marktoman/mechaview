using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.ViewModel.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageAttribute : ActionAttribute
    {
        public MessageAttribute(string title, string message)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            Title = title;
            Message = message;
        }

        public string Title { get; }
        public string Message { get; }
    }
}
