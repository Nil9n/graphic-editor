using System.Collections.Generic;

namespace graphic_editor
{
    public class DeleteShapeCommand : ICommand
    {
        private readonly List<Shape> _shapesList;
        private readonly Shape _shapeToDelete;
        private readonly int _originalIndex;

        public DeleteShapeCommand(List<Shape> shapesList, Shape shapeToDelete)
        {
            _shapesList = shapesList;
            _shapeToDelete = shapeToDelete;
            _originalIndex = shapesList.IndexOf(shapeToDelete);
        }

        public void Execute()
        {
            _shapesList.Remove(_shapeToDelete);
        }

        public void Undo()
        {
            if (_originalIndex >= 0 && _originalIndex <= _shapesList.Count)
            {
                _shapesList.Insert(_originalIndex, _shapeToDelete);
            }
            else
            {
                _shapesList.Add(_shapeToDelete);
            }
        }
    }
}