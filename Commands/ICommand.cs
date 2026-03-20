namespace ProcessKiller.Commands
{
    public interface ICommand
    {
        int Execute();
        bool CanExecute(string[] args);
    }
}
