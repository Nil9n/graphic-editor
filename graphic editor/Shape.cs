using System;
using System.Windows;
using System.Windows.Media;

namespace graphic_editor
{
    [Serializable]
    public abstract class Shape
    {
        public virtual Point Position { get; set; }

        public SerializableColor FillColor { get; set; }
        public SerializableColor StrokeColor { get; set; }

        public double StrokeThickness { get; set; } = 2;
        public int ZIndex { get; set; }

        public abstract void Draw(DrawingContext context);

        public abstract bool Contains(Point point);

        public virtual void Move(Vector delta)
        {
            Position += delta;
        }

        public virtual bool HitTest(Point point)
        {
            return Contains(point);
        }
    }
}