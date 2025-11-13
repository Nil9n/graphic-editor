using System.Windows.Input;
using System.Windows.Media;

namespace graphic_editor
{
    public class EditorContext
    {
        public System.Action<ICommand> ExecuteCommand { get; }

        public Color CurrentFillColor { get; set; } = Colors.LightGray;
        public Color CurrentStrokeColor { get; set; } = Colors.Black;

        public EditorContext(System.Action<ICommand> executeCommand)
        {
            ExecuteCommand = executeCommand;
        }
    }
}