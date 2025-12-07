using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace graphic_editor
{
    public class RectangleTool : ITool
    {
        private Point _startPoint;
        private MyRectangle _previewRect;
        private readonly EditorContext _context;

        public RectangleTool(EditorContext context)
        {
            _context = context;
        }

        public void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e)
        {
            _startPoint = canvas.GetWorldPoint(e.GetPosition(canvas));
            _previewRect = new MyRectangle
            {
                Position = _startPoint,
                Width = 0,
                Height = 0,
                FillColor = SerializableColor.FromColor(_context.CurrentFillColor),
                StrokeColor = SerializableColor.FromColor(_context.CurrentStrokeColor),
                StrokeThickness = 2
            };

            canvas.PreviewShape = _previewRect;
        }

        public void OnMouseMove(MyCanvas canvas, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _previewRect != null)
            {
                Point currentPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

                var x = System.Math.Min(_startPoint.X, currentPoint.X);
                var y = System.Math.Min(_startPoint.Y, currentPoint.Y);
                var width = System.Math.Abs(_startPoint.X - currentPoint.X);
                var height = System.Math.Abs(_startPoint.Y - currentPoint.Y);

                _previewRect.Position = new Point(x, y);
                _previewRect.Width = width;
                _previewRect.Height = height;

                canvas.Invalidate();
            }
        }

        public void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e)
        {
            if (_previewRect != null)
            {
                var finalRect = new MyRectangle
                {
                    Position = _previewRect.Position,
                    Width = _previewRect.Width,
                    Height = _previewRect.Height,
                    FillColor = _previewRect.FillColor,
                    StrokeColor = _previewRect.StrokeColor,
                    StrokeThickness = _previewRect.StrokeThickness
                };

                if (finalRect.Width > 1 && finalRect.Height > 1)
                {
                    _context.ExecuteCommand(new AddShapeCommand(canvas.Shapes, finalRect));
                }

                canvas.PreviewShape = null;
                _previewRect = null;
                canvas.Invalidate();
            }
        }

        public void FinishPolygon(MyCanvas canvas)
        {

        }
    }
}