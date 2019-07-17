using Mecha.ViewModel;
using Mecha.ViewModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MV = Mecha.ViewModel;

namespace SampleApp
{
    public class ActionDialogs
    {
        public virtual string StatusLabel { get; set; }

        [Confirm("Confirmation", "Continue?")]
        [Progress("Static Progress", "Processing...")]
        [Message("Static Message", "Done")]
        public async Task StaticAll()
        {
            await Task.Delay(1500);
        }

        public void DynamicMessage(IMechaMessage msg)
        {
            msg.Title = "Dynamic Message";
            msg.Message = "Done at " + DateTime.Now.ToShortTimeString();
        }

        public async Task DynamicProgress(IMechaProgress prog)
        {
            prog.Title = "Dynamic Progress";
            prog.IsCancelable = true;

            for (int i = 0; !prog.IsCanceled && i < 100; i++)
            {
                prog.Progress = .01 * i;
                prog.Message = $"Processing... ({i}%)";
                await Task.Delay(10);
            }
        }

        public void ThrowError()
        {
            throw new Exception("Invalid something");
        }
    }
}
