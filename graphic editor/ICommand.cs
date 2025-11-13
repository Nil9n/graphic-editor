namespace graphic_editor
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}