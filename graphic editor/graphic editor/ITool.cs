using System.Windows.Input;

namespace graphic_editor
{
    public interface ITool
    {
        void OnMouseDown(MyCanvas canvas, MouseButtonEventArgs e);
        void OnMouseMove(MyCanvas canvas, MouseEventArgs e);
        void OnMouseUp(MyCanvas canvas, MouseButtonEventArgs e);
        void FinishPolygon(MyCanvas canvas);
    }
}