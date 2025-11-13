using System;
using System.Windows;
using System.Windows.Media;

namespace graphic_editor
{
    [Serializable]
    public class MyEllipse : Shape
    {
        public double RadiusX { get; set; }
        public double RadiusY { get; set; }

        public override void Draw(DrawingContext context)
        {
            Brush fill = new SolidColorBrush(FillColor.ToColor());
            Pen stroke = new Pen(new SolidColorBrush(StrokeColor.ToColor()), StrokeThickness);

            context.DrawEllipse(fill, stroke, new Point(Position.X + RadiusX, Position.Y + RadiusY), RadiusX, RadiusY);
        }

        public override bool Contains(Point point)
        {
            var center = new Point(Position.X + RadiusX, Position.Y + RadiusY);
            var geometry = new EllipseGeometry(center, RadiusX, RadiusY);
            return geometry.FillContains(point);
        }
    }
}