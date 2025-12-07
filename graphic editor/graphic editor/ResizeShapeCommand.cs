using System.Windows;
using System.Linq;

namespace graphic_editor
{
    public class ResizeShapeCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly Vector _sizeDelta;
        private readonly ResizeHandle _handle;

        private readonly double _oldWidth, _oldHeight, _oldRadiusX, _oldRadiusY;
        private readonly Point _oldPosition, _oldEndPoint;
        private readonly System.Collections.Generic.List<Point> _oldPoints;

        public ResizeShapeCommand(Shape shape, Vector sizeDelta, ResizeHandle handle)
        {
            _shape = shape;
            _sizeDelta = sizeDelta;
            _handle = handle;

            if (shape is MyRectangle rect)
            {
                _oldWidth = rect.Width;
                _oldHeight = rect.Height;
                _oldPosition = rect.Position;
            }
            else if (shape is MyEllipse ellipse)
            {
                _oldRadiusX = ellipse.RadiusX;
                _oldRadiusY = ellipse.RadiusY;
                _oldPosition = ellipse.Position;
            }
            else if (shape is MyLine line)
            {
                _oldPosition = line.Position;
                _oldEndPoint = line.EndPoint;
            }
            else if (shape is MyPolygon polygon)
            {
                _oldPoints = new System.Collections.Generic.List<Point>(polygon.Points);
            }
        }

        public void Execute()
        {
            ApplyResize(_sizeDelta);
        }

        public void Undo()
        {
            if (_shape is MyRectangle rect)
            {
                rect.Position = _oldPosition;
                rect.Width = _oldWidth;
                rect.Height = _oldHeight;
            }
            else if (_shape is MyEllipse ellipse)
            {
                ellipse.Position = _oldPosition;
                ellipse.RadiusX = _oldRadiusX;
                ellipse.RadiusY = _oldRadiusY;
            }
            else if (_shape is MyLine line)
            {
                line.Position = _oldPosition;
                line.EndPoint = _oldEndPoint;
            }
            else if (_shape is MyPolygon polygon)
            {
                polygon.Points.Clear();
                polygon.Points.AddRange(_oldPoints);
            }
        }

        private void ApplyResize(Vector delta)
        {
            if (_shape is MyRectangle rect)
            {
                ApplyRectangleResize(rect, delta);
            }
            else if (_shape is MyEllipse ellipse)
            {
                ApplyEllipseResize(ellipse, delta);
            }
            else if (_shape is MyLine line)
            {
                ApplyLineResize(line, delta);
            }
            else if (_shape is MyPolygon polygon)
            {
                ApplyPolygonResize(polygon, delta);
            }
        }

        private void ApplyRectangleResize(MyRectangle rect, Vector delta)
        {
            switch (_handle)
            {
                case ResizeHandle.TopLeft:
                    rect.Position = new Point(rect.Position.X + delta.X, rect.Position.Y + delta.Y);
                    rect.Width = System.Math.Max(1, rect.Width - delta.X);
                    rect.Height = System.Math.Max(1, rect.Height - delta.Y);
                    break;
                case ResizeHandle.TopRight:
                    rect.Position = new Point(rect.Position.X, rect.Position.Y + delta.Y);
                    rect.Width = System.Math.Max(1, rect.Width + delta.X);
                    rect.Height = System.Math.Max(1, rect.Height - delta.Y);
                    break;
                case ResizeHandle.BottomLeft:
                    rect.Position = new Point(rect.Position.X + delta.X, rect.Position.Y);
                    rect.Width = System.Math.Max(1, rect.Width - delta.X);
                    rect.Height = System.Math.Max(1, rect.Height + delta.Y);
                    break;
                case ResizeHandle.BottomRight:
                    rect.Width = System.Math.Max(1, rect.Width + delta.X);
                    rect.Height = System.Math.Max(1, rect.Height + delta.Y);
                    break;
            }
        }

        private void ApplyEllipseResize(MyEllipse ellipse, Vector delta)
        {
            switch (_handle)
            {
                case ResizeHandle.TopLeft:
                    ellipse.Position = new Point(ellipse.Position.X + delta.X, ellipse.Position.Y + delta.Y);
                    ellipse.RadiusX = System.Math.Max(0.5, ellipse.RadiusX - delta.X / 2);
                    ellipse.RadiusY = System.Math.Max(0.5, ellipse.RadiusY - delta.Y / 2);
                    break;
                case ResizeHandle.TopRight:
                    ellipse.Position = new Point(ellipse.Position.X, ellipse.Position.Y + delta.Y);
                    ellipse.RadiusX = System.Math.Max(0.5, ellipse.RadiusX + delta.X / 2);
                    ellipse.RadiusY = System.Math.Max(0.5, ellipse.RadiusY - delta.Y / 2);
                    break;
                case ResizeHandle.BottomLeft:
                    ellipse.Position = new Point(ellipse.Position.X + delta.X, ellipse.Position.Y);
                    ellipse.RadiusX = System.Math.Max(0.5, ellipse.RadiusX - delta.X / 2);
                    ellipse.RadiusY = System.Math.Max(0.5, ellipse.RadiusY + delta.Y / 2);
                    break;
                case ResizeHandle.BottomRight:
                    ellipse.RadiusX = System.Math.Max(0.5, ellipse.RadiusX + delta.X / 2);
                    ellipse.RadiusY = System.Math.Max(0.5, ellipse.RadiusY + delta.Y / 2);
                    break;
            }
        }

        private void ApplyLineResize(MyLine line, Vector delta)
        {
            switch (_handle)
            {
                case ResizeHandle.TopLeft:
                    line.Position += delta;
                    break;
                case ResizeHandle.TopRight:
                    line.EndPoint += delta;
                    break;
                case ResizeHandle.BottomLeft:
                    line.Position += delta;
                    break;
                case ResizeHandle.BottomRight:
                    line.EndPoint += delta;
                    break;
            }
        }

        private void ApplyPolygonResize(MyPolygon polygon, Vector delta)
        {
            if (polygon.Points.Count == 0) return;

            double minX = polygon.Points.Min(p => p.X);
            double minY = polygon.Points.Min(p => p.Y);
            double maxX = polygon.Points.Max(p => p.X);
            double maxY = polygon.Points.Max(p => p.Y);
            Point center = new Point((minX + maxX) / 2, (minY + maxY) / 2);

            double scaleX = 1 + delta.X / (maxX - minX);
            double scaleY = 1 + delta.Y / (maxY - minY);

            for (int i = 0; i < polygon.Points.Count; i++)
            {
                Vector fromCenter = polygon.Points[i] - center;
                Vector scaled = new Vector(fromCenter.X * scaleX, fromCenter.Y * scaleY);
                polygon.Points[i] = center + scaled;
            }
        }
    }

    public enum ResizeHandle
    {
        None,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}