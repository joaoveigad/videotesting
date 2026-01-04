using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using video.services;
using video.Services;
using System.Windows.Input;
using video.Views;

public class PlayerViewModel

{
    private readonly IMediaDialogService _mediaDialogService;
    private readonly IMediaPlayerService _mediaPlayerService;

    public ICommand PlayPauseCommand { get; }
    public ICommand StopCommand { get; }

    public ObservableCollection<MenuItemViewModel> MenuItems { get; }


    public PlayerViewModel(IMediaDialogService mediaDialogService, IMediaPlayerService mediaPlayerService)

    {
        _mediaDialogService = mediaDialogService;
        _mediaPlayerService = mediaPlayerService;

        StopCommand = new RelayCommand(Stop);
        PlayPauseCommand = new RelayCommand(PlayPause);

        MenuItems = new ObservableCollection<MenuItemViewModel>
    {
        new MenuItemViewModel(
            "File",
            children: new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("Open", new RelayCommand(Open))
            }
        ),

        new MenuItemViewModel("View", new RelayCommand(View)),
        new MenuItemViewModel("Audio", new RelayCommand(Audio)),
        new MenuItemViewModel("Video", new RelayCommand(Video)),
        new MenuItemViewModel("Subtitles", new RelayCommand(Subtitles)),
        new MenuItemViewModel("Help", new RelayCommand(Help))
    };
    }



    private void Stop()
    {
        _mediaPlayerService.Stop();
    }
    private void PlayPause()
    {
        _mediaPlayerService.PlayPause();
    }
    private void Open()
    {
        var path = _mediaDialogService.OpenMediaFileDialog();
        _mediaPlayerService.Load(path);
        _mediaPlayerService.PlayPause();
        
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


