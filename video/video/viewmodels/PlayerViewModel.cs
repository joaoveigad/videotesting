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
    public ObservableCollection<string> Playlist { get; } = new();
    private int _currentIndex = -1;


    public PlayerViewModel(IMediaDialogService mediaDialogService, IMediaPlayerService mediaPlayerService)

    {

        // Dependency Injection
        _mediaDialogService = mediaDialogService;
        _mediaPlayerService = mediaPlayerService;

        StopCommand = new RelayCommand(Stop);
        PlayPauseCommand = new RelayCommand(PlayPause);

        // Header Menu
        MenuItems = new ObservableCollection<MenuItemViewModel>
    {
        new MenuItemViewModel(
            "File",
            children: new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("Open", new RelayCommand(Open)),
                new MenuItemViewModel("Open Many", new RelayCommand(openMany))
            }
        ),

        new MenuItemViewModel("View", new RelayCommand(View)),
        new MenuItemViewModel("Audio", new RelayCommand(Audio)),
        new MenuItemViewModel("Video", new RelayCommand(Video)),
        new MenuItemViewModel("Subtitles", new RelayCommand(Subtitles)),
        new MenuItemViewModel("Help", new RelayCommand(Help))
    };
    }

    // File selection methods
    private void Open()
    {
        var path = _mediaDialogService.OpenMediaFileDialog();
        if (!string.IsNullOrEmpty(path))
        {
            _mediaPlayerService.Load(path);
            _mediaPlayerService.PlayPause();
        }
    }

    private void openMany()
    {
        var paths = _mediaDialogService.OpenManyMediaFIleDialog();
        Playlist.Clear();

        foreach (var path in paths)
        {
            Playlist.Add(path);
        }

        if (Playlist.Count == 0)
        {
            return;
        }

        _currentIndex = 0;
        _mediaPlayerService.Load(Playlist[_currentIndex]); 
        _mediaPlayerService.PlayPause();

    }

    // Playback methods

    private void Stop()
    {
        _mediaPlayerService.Stop();
    }


    private void PlayPause()
    {
        _mediaPlayerService.PlayPause();
    }

    // Test Menu Item Methods

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


