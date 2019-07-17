using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mecha.ViewModel;

namespace Mecha.View.Wpf
{
    public delegate Task<IMechaProgress> ShowProgressDialog(string title, string message, bool isCancelable);
    public delegate Task ShowErrorDialog(string title, string message);
    public delegate Task ShowInfoDialog(string title, string message);
    public delegate Task<bool> ShowConfirmDialog(string title, string message, string affirmativeText, string dismissiveText);
    //public delegate void CloseDialog();

    public class Dialogs
    {
        public Dialogs(
            ShowProgressDialog showProgress,
            ShowErrorDialog showError,
            ShowInfoDialog showInfo,
            ShowConfirmDialog showConfirm)
        {
            if (showProgress == null)
                throw new ArgumentNullException(nameof(showProgress));
            if (showError == null)
                throw new ArgumentNullException(nameof(showError));
            if (showInfo == null)
                throw new ArgumentNullException(nameof(showInfo));
            if (showConfirm == null)
                throw new ArgumentNullException(nameof(showConfirm));

            ShowProgress = showProgress;
            ShowError = showError;
            ShowInfo = showInfo;
            ShowConfirm = showConfirm;
        }
        public ShowProgressDialog ShowProgress { get; }
        public ShowErrorDialog ShowError { get; }
        public ShowInfoDialog ShowInfo { get; }
        public ShowConfirmDialog ShowConfirm { get; }
    }
}
