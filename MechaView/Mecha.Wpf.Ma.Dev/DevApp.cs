using Mecha.ViewModel.Attributes;
using Mecha.Wpf.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MechaApp : IApp
{
    public void Init(AppSettings s)
    {
        s.Title = "MechaView Dev";
        s.Window.Width = 450;
        s.Content = typeof(Mecha.Wpf.Ma.Dev.ElementGrouping);
        s.Window.Accent = Accent.Cobalt;
        //s.Window.ColorMode = ColorMode.Light;
    }
}

namespace Mecha.Wpf.Ma.Dev
{
    public class DevApp
    {
        public virtual string StatusLabel { get; set; }

        public virtual string User { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime Start { get; set; } = DateTime.Now.Date.AddDays(-1);
        public virtual string SaveToPath { get; set; }

        public enum Dimension { ThreeDimensional, TwoDimensional }
        public virtual Dimension PointDimension { get; set; }

        public virtual bool FilterOutNegative { get; set; }
        public virtual Point[] Points { get; set; }

        public async Task GetPoints()
        {
            StatusLabel = $"Getting points since {Start}...";
            await Task.Delay(1500);

            Points = Enumerable.Range(-10, 90)
                .Select(i => new Point { X = i, Y = i + 10, Z = PointDimension == Dimension.ThreeDimensional ? i * 10 : 0 })
                .Where(x => !FilterOutNegative || (x.X >= 0 && x.Y >= 0 && x.Z >= 0))
                .ToArray();

            if (SaveToPath != null)
            {
                StatusLabel = $"Saving points to {SaveToPath}...";
                await Task.Delay(1000);
            }

            StatusLabel = "Success!";
        }
    }

    public class DevApp2
    {
        enum Grp
        {
            Input,
            [Group(invisible: true)] Result
        }

        //
        // User
        //

        [Element(Group = Grp.Input, Position = 0.0)]
        public virtual string User { get; set; }

        [Element(Group = Grp.Input, Position = 0.1)]
        public virtual string Password { get; set; }

        [Element(Group = Grp.Input, Position = 1)]
        public virtual bool FilterOutNegative { get; set; }

        [Element(Group = Grp.Input, Position = 2)]
        public virtual string SaveToPath { get; set; }

        public enum PointDimension { ThreeDimensional, TwoDimensional }
        [Element(Group = Grp.Input, Position = 3.0)]
        public virtual DateTime Start { get; set; } = DateTime.Now.AddDays(-1);

        [Element(Group = Grp.Input, Position = 3.1)]
        public virtual PointDimension Dimension { get; set; }

        [Element(Group = Grp.Input)]
        public void Clear()
        {
            User = Password = SaveToPath = null;
        }

        //
        // Result
        //

        [Element(Group = Grp.Result)]
        public virtual string StatusLabel { get; set; }

        [Element(Group = Grp.Result)]
        public virtual Point[] Points { get; set; }

        public async Task GetPoints()
        {
            StatusLabel = $"Getting points since {Start}...";
            await Task.Delay(1500);

            Points = Enumerable.Range(-10, 90)
                .Select(i => new Point { X = i, Y = i + 10, Z = Dimension == PointDimension.ThreeDimensional ? i * 10 : 0 })
                .Where(x => !FilterOutNegative || (x.X >= 0 && x.Y >= 0 && x.Z >= 0))
                .ToArray();

            StatusLabel = "Success!";
        }
    }


    public class ElementGrouping
    {
        enum Grp
        {
            Input,
            [Group(invisible: true)] Result
        }

        //
        // User
        //

        [Element(Group = Grp.Input, Position = 0.0)]
        public virtual string User { get; set; }

        [Element(Group = Grp.Input, Position = 0.1)]
        public virtual string Password { get; set; }

        [Element(Group = Grp.Input, Position = 1)]
        public virtual bool FilterOutNegative { get; set; }

        [Element(Group = Grp.Input, Position = 2)]
        public virtual string SaveToPath { get; set; }

        public enum PointDimension { ThreeDimensional, TwoDimensional }
        [Element(Group = Grp.Input, Position = 3.0)]
        public virtual DateTime Start { get; set; } = DateTime.Now.AddDays(-1);

        [Element(Group = Grp.Input, Position = 3.1)]
        public virtual PointDimension Dimension { get; set; }

        [Element(Group = Grp.Input)]
        public void Clear()
        {
            User = Password = SaveToPath = null;
        }

        //
        // Result
        //

        [Element(Group = Grp.Result)]
        public virtual string StatusLabel { get; set; }

        [Element(Group = Grp.Result)]
        public virtual Point[] Points { get; set; }

        public async Task GetPoints()
        {
            StatusLabel = $"Getting points since {Start}...";
            await Task.Delay(1500);

            Points = Enumerable.Range(-10, 90)
                .Select(i => new Point { X = i, Y = i + 10, Z = Dimension == PointDimension.ThreeDimensional ? i * 10 : 0 })
                .Where(x => !FilterOutNegative || (x.X >= 0 && x.Y >= 0 && x.Z >= 0))
                .ToArray();

            StatusLabel = "Success!";
        }
    }


    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
