using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace graphic_editor
{
    [Serializable]
    public class MyPolygon : Shape
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public int Sides { get; set; } = 5;

        public override Point Position
        {
            get => Points.Count > 0 ? new Point(Points.Min(p => p.X), Points.Min(p => p.Y)) : new Point(0, 0);
            set
            {
                if (Points.Count == 0) return;

                Vector delta = value - Position;
                Move(delta); 
            }
        }

        public override void Move(Vector delta)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] += delta;
            }
        }

        public override void Draw(DrawingContext context)
        {
            if (Points.Count < 2) return;

            Brush fill = new SolidColorBrush(FillColor.ToColor());
            Pen stroke = new Pen(new SolidColorBrush(StrokeColor.ToColor()), StrokeThickness);

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(Points[0], true, true);
                for (int i = 1; i < Points.Count; i++)
                {
                    ctx.LineTo(Points[i], true, false);
                }
            }

            context.DrawGeometry(fill, stroke, geometry);
        }

        public override bool Contains(Point point)
        {
            if (Points.Count < 2) return false;

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(Points[0], true, true);
                for (int i = 1; i < Points.Count; i++)
                {
                    ctx.LineTo(Points[i], true, false);
                }
                ctx.LineTo(Points[0], true, false);
            }
            geometry.Freeze();

            return geometry.FillContains(point) || geometry.StrokeContains(new Pen(Brushes.Black, StrokeThickness + 2), point);
        }
    }
}