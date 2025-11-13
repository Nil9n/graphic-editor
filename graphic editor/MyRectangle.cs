using System;
using System.Windows;
using System.Windows.Media;

namespace graphic_editor
{
    [Serializable]
    public class MyRectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public override void Draw(DrawingContext context)
        {
            Brush fill = new SolidColorBrush(FillColor.ToColor());
            Pen stroke = new Pen(new SolidColorBrush(StrokeColor.ToColor()), StrokeThickness);
            context.DrawRectangle(fill, stroke, new Rect(Position, new Size(Width, Height)));
        }

        public override bool Contains(Point point)
        {
            Rect rect = new Rect(Position, new Size(Width, Height));
            return rect.Contains(point);
        }
    }
}