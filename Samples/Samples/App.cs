using Mecha.Wpf.Settings;
using System;

public class App : IApp
{
    public void Init(AppSettings s)
    {
        s.Title = "Element Grouping";
        s.Window.Width = 450;

        s.Content = typeof(SampleApp.ElementGrouping);
        //s.Content = typeof(SampleApp.ElementConventions);
        //s.Content = typeof(SampleApp.ElementDecoration);
        //s.Content = typeof(SampleApp.InputValidation);
        //s.Content = typeof(SampleApp.ActionDialogs);

        s.Window.Accent = Accent.Cobalt;
        s.Window.ColorMode = ColorMode.Light;
    }
}
