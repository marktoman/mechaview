using MahApps.Metro.Controls.Dialogs;
using Mecha.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mecha.Wpf.Ma
{
    public class ProgressDialog : IMechaProgress
    {
        ProgressDialogController ctrl;
        bool isCancelable;
        string title;
        string message;
        double progress;

        public ProgressDialog(string title, string message, bool isCancelable, ProgressDialogController ctrl)
        {
            if (ctrl == null)
                throw new ArgumentNullException(nameof(ctrl));

            this.ctrl = ctrl;
            this.title = title;
            this.message = message;
            this.isCancelable = isCancelable;
        }

        public bool IsOpen { get { return ctrl.IsOpen; } }

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                ctrl.SetMessage(value);
            }
        }

        public bool IsIndeterminate
        {
            get { return double.IsNaN(progress) || progress < 0; }
            set { progress = double.NaN; }
        }

        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                if (double.IsNaN(value) || value < 0)
                    ctrl.SetIndeterminate();
                else
                    ctrl.SetProgress(value);
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                ctrl.SetTitle(value);
            }
        }

        public bool IsCancelable
        {
            get { return isCancelable; }
            set
            {
                isCancelable = value;
                ctrl.SetCancelable(value);
            }
        }

        public bool IsCanceled
        {
            get { return ctrl.IsCanceled; }
        }

        public Task CloseAsync()
        {
            return ctrl.CloseAsync();
        }
    }
}
