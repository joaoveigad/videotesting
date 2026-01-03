using System.Windows.Input;

public class MenuItemViewModel
{
    public string Title { get; }
    public ICommand Command { get; }

    public MenuItemViewModel(string title, ICommand command)
    {
        Title = title;
        Command = command;
    }
}
