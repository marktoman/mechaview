using System;
using Mecha.ViewModel;
using Mecha.ViewModel.Elements;

namespace Mecha.View.Wpf
{
    internal class ButtonHandle : IMechaAction
    {
        readonly Button button;
        public ButtonHandle(Button button)
        {
            if (button == null)
                throw new ArgumentNullException();

            this.button = button;
        }

        public string Label
        {
            get { return button.DisplayName; }
            set { button.DisplayName = value; }
        }
    }
}