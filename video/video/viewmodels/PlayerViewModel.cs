using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

public class PlayerViewModel
{
    public ObservableCollection<MenuItemViewModel> MenuItems { get; }

    public PlayerViewModel()
    {
        MenuItems = new ObservableCollection<MenuItemViewModel>
        {
            new MenuItemViewModel("Open", new RelayCommand(Open)),
            new MenuItemViewModel("View", new RelayCommand(View)),
            new MenuItemViewModel("Audio", new RelayCommand(Audio)),
            new MenuItemViewModel("Video", new RelayCommand(Video)),
            new MenuItemViewModel("Subtitles", new RelayCommand(Subtitles)),
            new MenuItemViewModel("Help", new RelayCommand(Help))
        };
    }

    private void Open()
    {
        MessageBox.Show("Open clicked!");
    }
    private void View()
    {
        MessageBox.Show("View clicked!");
    }
    private void Audio()
    {
        MessageBox.Show("Audio clicked!");
    }
    private void Video()
    {
        MessageBox.Show("Video clicked!");
    }
    private void Subtitles()
    {
        MessageBox.Show("Subtitles clicked!");
    }
    private void Help()
    {
        MessageBox.Show("Help clicked!");
    }
}


