using Mecha.ViewModel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
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
}
