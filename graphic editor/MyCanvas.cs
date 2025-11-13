using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace graphic_editor
{
    public class MyCanvas : Canvas
    {
        public List<Shape> Shapes { get; } = new List<Shape>();
        public Shape PreviewShape { get; set; }
        public Shape SelectedShape { get; set; }

        public double Scale { get; set; } = 1.0;
        public Point Offset { get; set; } = new Point(0, 0);

        private Point _lastMousePos;
        private bool _isPanning = false;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.PushTransform(new TranslateTransform(Offset.X, Offset.Y));
            dc.PushTransform(new ScaleTransform(Scale, Scale));

            foreach (var shape in Shapes.OrderBy(s => s.ZIndex))
            {
                shape.Draw(dc);
            }

            PreviewShape?.Draw(dc);

            if (SelectedShape != null)
            {
                DrawSelection(dc, SelectedShape);
            }

            dc.Pop();
            dc.Pop();
        }

        public Point GetWorldPoint(Point mousePoint)
        {
            return new Point(
                (mousePoint.X - Offset.X) / Scale,
                (mousePoint.Y - Offset.Y) / Scale
            );
        }

        public void StartPan(Point mousePos)
        {
            _isPanning = true;
            _lastMousePos = mousePos;
            this.Cursor = Cursors.SizeAll;
            CaptureMouse();
        }

        public void DoPan(Point currentMousePos)
        {
            if (!_isPanning) return;

            double deltaX = currentMousePos.X - _lastMousePos.X;
            double deltaY = currentMousePos.Y - _lastMousePos.Y;

            Offset = new Point(Offset.X + deltaX, Offset.Y + deltaY);
            _lastMousePos = currentMousePos;

            Invalidate();
        }

        public void StopPan()
        {
            _isPanning = false;
            this.Cursor = Cursors.Arrow;
            ReleaseMouseCapture();
        }

        public void ZoomToPoint(double zoomFactor, Point mousePos)
        {
            Point worldPosBefore = GetWorldPoint(mousePos);

            double newScale = Scale * zoomFactor;
            newScale = System.Math.Max(0.1, System.Math.Min(5.0, newScale));

            Point worldPosAfter = new Point(
                (mousePos.X - Offset.X) / newScale,
                (mousePos.Y - Offset.Y) / newScale
            );

            double deltaX = (worldPosAfter.X - worldPosBefore.X) * newScale;
            double deltaY = (worldPosAfter.Y - worldPosBefore.Y) * newScale;

            Scale = newScale;
            Offset = new Point(Offset.X + deltaX, Offset.Y + deltaY);

            Invalidate();
        }

        public void ResetView()
        {
            Scale = 1.0;
            Offset = new Point(0, 0);
            Invalidate();
        }

        private void DrawSelection(DrawingContext dc, Shape shape)
        {
            var bounds = GetShapeBounds(shape);
            if (bounds.HasValue)
            {
                var dashPen = new Pen(Brushes.Blue, 1.0 / Scale)
                {
                    DashStyle = DashStyles.Dash
                };
                dc.DrawRectangle(null, dashPen, bounds.Value);
            }
        }

        private Rect? GetShapeBounds(Shape shape)
        {
            switch (shape)
            {
                case MyRectangle rect:
                    return new Rect(rect.Position, new Size(rect.Width, rect.Height));
                case MyEllipse ellipse:
                    return new Rect(ellipse.Position, new Size(ellipse.RadiusX * 2, ellipse.RadiusY * 2));
                case MyLine line:
                    return new Rect(
                        new Point(System.Math.Min(line.Position.X, line.EndPoint.X), System.Math.Min(line.Position.Y, line.EndPoint.Y)),
                        new Point(System.Math.Max(line.Position.X, line.EndPoint.X), System.Math.Max(line.Position.Y, line.EndPoint.Y))
                    );
                case MyPolygon polygon when polygon.Points.Count > 0:
                    double minX = polygon.Points.Min(p => p.X);
                    double minY = polygon.Points.Min(p => p.Y);
                    double maxX = polygon.Points.Max(p => p.X);
                    double maxY = polygon.Points.Max(p => p.Y);
                    return new Rect(minX, minY, maxX - minX, maxY - minY);
                default:
                    return null;
            }
        }

        public void Invalidate()
        {
            this.InvalidateVisual();
        }
    }
}