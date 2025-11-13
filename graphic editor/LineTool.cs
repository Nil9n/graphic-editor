using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace graphic_editor
{
    public class LineTool : ITool
    {
        private Point _startPoint;
        private MyLine _previewLine;
        private readonly EditorContext _context;

        public LineTool(EditorContext context)
        {
            _context = context;
        }

        public void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e)
        {
            _startPoint = canvas.GetWorldPoint(e.GetPosition(canvas));
            _previewLine = new MyLine
            {
                Position = _startPoint,
                EndPoint = _startPoint,
                StrokeColor = SerializableColor.FromColor(_context.CurrentStrokeColor),
                StrokeThickness = 2,
                FillColor = SerializableColor.FromColor(Colors.Transparent)
            };
            canvas.PreviewShape = _previewLine;
        }

        public void OnMouseMove(MyCanvas canvas, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _previewLine != null)
            {
                _previewLine.EndPoint = canvas.GetWorldPoint(e.GetPosition(canvas));
                canvas.Invalidate();
            }
        }

        public void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e)
        {
            if (_previewLine != null)
            {
                var finalLine = new MyLine
                {
                    Position = _previewLine.Position,
                    EndPoint = _previewLine.EndPoint,
                    StrokeColor = _previewLine.StrokeColor,
                    StrokeThickness = _previewLine.StrokeThickness,
                    FillColor = _previewLine.FillColor
                };

                if ((finalLine.EndPoint - finalLine.Position).Length > 1)
                {
                    _context.ExecuteCommand(new AddShapeCommand(canvas.Shapes, finalLine));
                }

                canvas.PreviewShape = null;
                _previewLine = null;
                canvas.Invalidate();
            }
        }

        public void FinishPolygon(MyCanvas canvas)
        {

        }
    }
}