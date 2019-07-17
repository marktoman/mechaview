using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Mecha.View.Wpf
{
    [MarkupExtensionReturnType(typeof(Style))]
    public class MergedStylesExtension : MarkupExtension
    {
        public Style BasedOn { get; set; }
        public Style Merged { get; set; }
        public Style Merged2 { get; set; }
        public Style Merged3 { get; set; }
        public Style Merged4 { get; set; }
        public Style Merged5 { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (BasedOn == null)
                throw new ArgumentNullException(nameof(BasedOn));
            if (Merged == null)
                throw new ArgumentNullException(nameof(Merged));

            var newStyle = new Style(BasedOn.TargetType, BasedOn);
            
            MergeWithStyle(newStyle, Merged);
            
            if (Merged2 != null)
                MergeWithStyle(newStyle, Merged2);
            if (Merged3 != null)
                MergeWithStyle(newStyle, Merged3);
            if (Merged4 != null)
                MergeWithStyle(newStyle, Merged4);
            if (Merged5 != null)
                MergeWithStyle(newStyle, Merged5);

            return newStyle;
        }

        static void MergeWithStyle(Style style, Style mergeStyle)
        {
            // Recursively merge this style with any style it may be based on.
            if (mergeStyle.BasedOn != null)
                MergeWithStyle(style, mergeStyle.BasedOn);

            foreach (var setter in mergeStyle.Setters)
                style.Setters.Add(setter);

            foreach (var trigger in mergeStyle.Triggers)
                style.Triggers.Add(trigger);
        }
    }
}
