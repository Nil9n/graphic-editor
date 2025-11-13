using System;
using System.Windows;
using System.Windows.Media;

namespace graphic_editor
{
    [Serializable]
    public class MyLine : Shape
    {
        public Point EndPoint { get; set; }

        public override void Move(Vector delta)
        {
            base.Move(delta);
            EndPoint += delta;
        }

        public override void Draw(DrawingContext context)
        {
            Pen stroke = new Pen(new SolidColorBrush(StrokeColor.ToColor()), StrokeThickness);
            context.DrawLine(stroke, Position, EndPoint);
        }

        public override bool Contains(Point point)
        {
            var geometry = new LineGeometry(Position, EndPoint);
            return geometry.StrokeContains(new Pen(Brushes.Black, StrokeThickness + 5), point);
        }
    }
}