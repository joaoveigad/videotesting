using System.Collections.ObjectModel;
using System.Windows.Input;

public class MenuItemViewModel
{
    public string Title { get; }
    public ICommand? Command { get; }
    public ObservableCollection<MenuItemViewModel>? Children { get; }

    public bool HasChildren => Children != null && Children.Count > 0;

    public MenuItemViewModel(
        string title,
        ICommand? command = null,
        ObservableCollection<MenuItemViewModel>? children = null)
    {
        Title = title;
        Command = command;
        Children = children;
    }
}
