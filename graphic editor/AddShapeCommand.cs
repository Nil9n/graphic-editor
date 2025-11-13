using System.Collections.Generic;

namespace graphic_editor
{
    public class AddShapeCommand : ICommand
    {
        private readonly List<Shape> _shapesList;
        private readonly Shape _shapeToAdd;

        public AddShapeCommand(List<Shape> shapesList, Shape shapeToAdd)
        {
            _shapesList = shapesList;
            _shapeToAdd = shapeToAdd;
            _shapeToAdd.ZIndex = shapesList.Count;
        }

        public void Execute()
        {
            _shapesList.Add(_shapeToAdd);
        }

        public void Undo()
        {
            _shapesList.Remove(_shapeToAdd);
        }
    }
}