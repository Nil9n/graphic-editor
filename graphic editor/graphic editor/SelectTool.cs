using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace graphic_editor
{
    public class SelectTool : ITool
    {
        private readonly EditorContext _context;
        private Point _lastMouseWorldPoint;
        private bool _isDragging = false;
        private bool _isResizing = false;
        private ResizeHandle _activeHandle;
        private Point _resizeStartPoint;

        public SelectTool(EditorContext context)
        {
            _context = context;
        }

        public void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e)
        {
            Point worldPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

            if (canvas.SelectedShape != null)
            {
                _activeHandle = GetResizeHandleAt(canvas, worldPoint, canvas.SelectedShape);
                if (_activeHandle != ResizeHandle.None)
                {
                    _isResizing = true;
                    _resizeStartPoint = worldPoint;
                    return;
                }
            }

            canvas.SelectedShape = null;
            _isDragging = false;
            _isResizing = false;

            for (int i = canvas.Shapes.Count - 1; i >= 0; i--)
            {
                Shape shape = canvas.Shapes[i];
                if (shape.HitTest(worldPoint))
                {
                    canvas.SelectedShape = shape;
                    _lastMouseWorldPoint = worldPoint;
                    _isDragging = true;
                    break;
                }
            }

            canvas.Invalidate();
        }

        public void OnMouseMove(MyCanvas canvas, MouseEventArgs e)
        {
            Point currentWorldPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_isResizing && canvas.SelectedShape != null)
                {
                    Vector delta = currentWorldPoint - _resizeStartPoint;
                    ApplyTempResize(canvas.SelectedShape, delta, _activeHandle);
                    _resizeStartPoint = currentWorldPoint;
                    canvas.Invalidate();
                }
                else if (_isDragging && canvas.SelectedShape != null)
                {
                    Vector delta = currentWorldPoint - _lastMouseWorldPoint;
                    canvas.SelectedShape.Move(delta);
                    _lastMouseWorldPoint = currentWorldPoint;
                    canvas.Invalidate();
                }
            }
        }

        public void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e)
        {
            if (_isResizing && canvas.SelectedShape != null)
            {
                Point finalWorldPoint = canvas.GetWorldPoint(e.GetPosition(canvas));
                Vector totalDelta = finalWorldPoint - _resizeStartPoint;

                if (totalDelta.Length > 0.1)
                {
                    var command = new ResizeShapeCommand(canvas.SelectedShape, totalDelta, _activeHandle);
                    _context.ExecuteCommand(command);
                }
            }
            else if (_isDragging && canvas.SelectedShape != null)
            {
                Point finalWorldPoint = canvas.GetWorldPoint(e.GetPosition(canvas));
                Vector totalDelta = finalWorldPoint - _lastMouseWorldPoint;

                if (totalDelta.Length > 0.1)
                {
                    var command = new MoveShapeCommand(canvas.SelectedShape, totalDelta);
                    _context.ExecuteCommand(command);
                }
            }

            _isDragging = false;
            _isResizing = false;
            _activeHandle = ResizeHandle.None;
        }

        public void FinishPolygon(MyCanvas canvas)
        {

        }

        private ResizeHandle GetResizeHandleAt(MyCanvas canvas, Point worldPoint, Shape shape)
        {
            var bounds = canvas.GetShapeBounds(shape);
            if (!bounds.HasValue) return ResizeHandle.None;

            double handleSize = 10 / canvas.Scale; 

            var rect = bounds.Value;
            if (IsPointInHandle(worldPoint, new Point(rect.Left, rect.Top), handleSize)) return ResizeHandle.TopLeft;
            if (IsPointInHandle(worldPoint, new Point(rect.Right, rect.Top), handleSize)) return ResizeHandle.TopRight;
            if (IsPointInHandle(worldPoint, new Point(rect.Left, rect.Bottom), handleSize)) return ResizeHandle.BottomLeft;
            if (IsPointInHandle(worldPoint, new Point(rect.Right, rect.Bottom), handleSize)) return ResizeHandle.BottomRight;

            return ResizeHandle.None;
        }

        private bool IsPointInHandle(Point point, Point handleCenter, double handleSize)
        {
            return (point - handleCenter).Length <= handleSize;
        }

        private void ApplyTempResize(Shape shape, Vector delta, ResizeHandle handle)
        {
            if (shape is MyRectangle rect)
            {
                ApplyRectangleResize(rect, delta, handle);
            }
            else if (shape is MyEllipse ellipse)
            {
                ApplyEllipseResize(ellipse, delta, handle);
            }
            else if (shape is MyLine line)
            {
                ApplyLineResize(line, delta, handle);
            }
            else if (shape is MyPolygon polygon)
            {
                ApplyPolygonResize(polygon, delta, handle);
            }
        }

        private void ApplyRectangleResize(MyRectangle rect, Vector delta, ResizeHandle handle)
        {
            switch (handle)
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

        private void ApplyEllipseResize(MyEllipse ellipse, Vector delta, ResizeHandle handle)
        {
            switch (handle)
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

        private void ApplyLineResize(MyLine line, Vector delta, ResizeHandle handle)
        {
            switch (handle)
            {
                case ResizeHandle.TopLeft:
                case ResizeHandle.BottomLeft:
                    line.Position += delta;
                    break;
                case ResizeHandle.TopRight:
                case ResizeHandle.BottomRight:
                    line.EndPoint += delta;
                    break;
            }
        }

        private void ApplyPolygonResize(MyPolygon polygon, Vector delta, ResizeHandle handle)
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
}