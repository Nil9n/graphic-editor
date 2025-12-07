using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace graphic_editor
{
    public class EllipseTool : ITool
    {
        private Point _startPoint;
        private MyEllipse _previewEllipse;
        private readonly EditorContext _context;

        public EllipseTool(EditorContext context)
        {
            _context = context;
        }

        public void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e)
        {
            _startPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

            _previewEllipse = new MyEllipse
            {
                Position = _startPoint,
                RadiusX = 0,
                RadiusY = 0,
                FillColor = SerializableColor.FromColor(_context.CurrentFillColor),
                StrokeColor = SerializableColor.FromColor(_context.CurrentStrokeColor),
                StrokeThickness = 2
            };
            canvas.PreviewShape = _previewEllipse;
        }

        public void OnMouseMove(MyCanvas canvas, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _previewEllipse != null)
            {
                Point currentPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

                var x = System.Math.Min(_startPoint.X, currentPoint.X);
                var y = System.Math.Min(_startPoint.Y, currentPoint.Y);
                var width = System.Math.Abs(_startPoint.X - currentPoint.X);
                var height = System.Math.Abs(_startPoint.Y - currentPoint.Y);

                _previewEllipse.Position = new Point(x, y);
                _previewEllipse.RadiusX = width / 2;
                _previewEllipse.RadiusY = height / 2;

                canvas.Invalidate();
            }
        }

        public void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e)
        {
            if (_previewEllipse != null)
            {
                var finalEllipse = new MyEllipse
                {
                    Position = _previewEllipse.Position,
                    RadiusX = _previewEllipse.RadiusX,
                    RadiusY = _previewEllipse.RadiusY,
                    FillColor = _previewEllipse.FillColor,
                    StrokeColor = _previewEllipse.StrokeColor,
                    StrokeThickness = _previewEllipse.StrokeThickness
                };

                if (finalEllipse.RadiusX > 1 && finalEllipse.RadiusY > 1)
                {
                    _context.ExecuteCommand(new AddShapeCommand(canvas.Shapes, finalEllipse));
                }

                canvas.PreviewShape = null;
                _previewEllipse = null;
                canvas.Invalidate();
            }
        }

        public void FinishPolygon(MyCanvas canvas)
        {

        }
    }
}