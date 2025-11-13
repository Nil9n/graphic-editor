using System.Windows;
using System.Windows.Input;

namespace graphic_editor
{
    public class SelectTool : ITool
    {
        private readonly EditorContext _context;
        private Point _lastMouseWorldPoint;
        private bool _isDragging = false;

        public SelectTool(EditorContext context)
        {
            _context = context;
        }

        public void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e)
        {
            Point worldPoint = canvas.GetWorldPoint(e.GetPosition(canvas));

            canvas.SelectedShape = null;
            _isDragging = false;

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
            if (e.LeftButton == MouseButtonState.Pressed && _isDragging && canvas.SelectedShape != null)
            {
                Point currentWorldPoint = canvas.GetWorldPoint(e.GetPosition(canvas));
                Vector delta = currentWorldPoint - _lastMouseWorldPoint;

                canvas.SelectedShape.Move(delta);
                _lastMouseWorldPoint = currentWorldPoint;

                canvas.Invalidate();
            }
        }

        public void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e)
        {
            if (_isDragging && canvas.SelectedShape != null)
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
        }

        public void FinishPolygon(MyCanvas canvas)
        {

        }
    }
}