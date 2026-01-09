using System.Windows.Input;
using video.models;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private VideoMetaData videoMetaData;

    public RelayCommand(Action execute)
    {
        _execute = execute;
    }

    public RelayCommand(VideoMetaData videoMetaData)
    {
        this.videoMetaData = videoMetaData;
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter) => _execute();

    public event EventHandler? CanExecuteChanged;
}