using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VM = Mecha.ViewModel.Elements;

namespace Mecha.View.Wpf.Controls
{
    internal class MandatoryInputRule : ValidationRule
    {
        readonly VM.StateElement stateVM;
        public MandatoryInputRule(VM.StateElement stateVM)
        {
            if (stateVM == null)
                throw new ArgumentNullException(nameof(stateVM));
            this.stateVM = stateVM;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (stateVM.StateAttribute?.Mandatory == true)
            {
                if (string.IsNullOrWhiteSpace(value as string))
                    return new ValidationResult(false, "Required");
            }

            return new ValidationResult(true, value);
        }
    }
}
