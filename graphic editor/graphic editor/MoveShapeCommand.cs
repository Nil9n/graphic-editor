using System.Windows;

namespace graphic_editor
{
    public class MoveShapeCommand : ICommand
    {
        private readonly Shape _shape;
        private readonly Vector _delta;

        public MoveShapeCommand(Shape shape, Vector delta)
        {
            _shape = shape;
            _delta = delta;
        }

        public void Execute()
        {
            _shape.Move(_delta);
        }

        public void Undo()
        {
            _shape.Move(-_delta);
        }
    }
}