using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace graphic_editor
{
    public class PolygonTool : ITool
    {
        private MyPolygon _previewPolygon;
        private readonly EditorContext _context;

        public PolygonTool(EditorContext context)
        {
            _context = context;
        }

        public void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e)
        {
            Point clickPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

            if (_previewPolygon == null)
            {
                _previewPolygon = new MyPolygon
                {

                    FillColor = SerializableColor.FromColor(_context.CurrentFillColor),
                    StrokeColor = SerializableColor.FromColor(_context.CurrentStrokeColor),
                    StrokeThickness = 2
                };
                _previewPolygon.Points.Add(clickPoint);
                canvas.PreviewShape = _previewPolygon;
            }
            else
            {
                _previewPolygon.Points.Add(clickPoint);
            }

            canvas.Invalidate();
        }

        public void OnMouseMove(MyCanvas canvas, MouseEventArgs e)
        {

        }

        public void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && _previewPolygon != null && _previewPolygon.Points.Count >= 3)
            {
                CompletePolygon(canvas);
            }
        }

        private void CompletePolygon(MyCanvas canvas)
        {
            if (_previewPolygon != null && _previewPolygon.Points.Count >= 3)
            {

                canvas.PreviewShape = null;
                _context.ExecuteCommand(new AddShapeCommand(canvas.Shapes, _previewPolygon));
                _previewPolygon = null;
                canvas.Invalidate();
            }
        }

        public void FinishPolygon(MyCanvas canvas)
        {
            if (_previewPolygon != null && _previewPolygon.Points.Count >= 3)
            {
                CompletePolygon(canvas);
            }
            else if (_previewPolygon != null)
            {
                canvas.PreviewShape = null;
                _previewPolygon = null;
                canvas.Invalidate();
            }
        }
    }
}