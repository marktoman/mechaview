using Mecha.ViewModel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleApp
{
    public class ElementConventions
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
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
