using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using video.services.Interfaces;
using video.models;
using video.services.Services;
using TagLib.Flac;


public class PlayerViewModel

{
    private readonly IMediaDialogService _mediaDialogService;
    private readonly IMediaPlayerService _mediaPlayerService;
    private readonly IVideoMetadataService _metadataService;

    public ICommand PlayPauseCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand PreviousCommand { get; }
    public RelayCommand FileMetadata { get; }

    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    public ObservableCollection<string> Playlist { get; } = new();
    private int _currentIndex = -1;
    private string _currentFile = "";
    
  


    public PlayerViewModel(IMediaDialogService mediaDialogService, IMediaPlayerService mediaPlayerService, IVideoMetadataService videoMetadataService)

    {

        // Dependency Injection
        _mediaDialogService = mediaDialogService;
        _mediaPlayerService = mediaPlayerService;
        _metadataService = videoMetadataService;

        StopCommand = new RelayCommand(Stop);
        PlayPauseCommand = new RelayCommand(PlayPause);
        NextCommand = new RelayCommand(NextInPlaylist);
        PreviousCommand = new RelayCommand(PreviousInPlaylist);
        FileMetadata = new RelayCommand(() => { });

        // Header Menu
        MenuItems = new ObservableCollection<MenuItemViewModel>
    {
        new MenuItemViewModel(
            "File",
            children: new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("Open", new RelayCommand(Open)),
                new MenuItemViewModel("Open many", new RelayCommand(openMany))
            }
        ),

        new MenuItemViewModel("View", new RelayCommand(View)),
        new MenuItemViewModel("Audio", new RelayCommand(Audio)),
        new MenuItemViewModel("Video", new RelayCommand(Video)),
        new MenuItemViewModel("Subtitles", new RelayCommand(Subtitles)),
        new MenuItemViewModel("Help", new RelayCommand(Help))
    };
    }

    // File management methods
    private void Open()
    {
        var path = _mediaDialogService.OpenMediaFileDialog();
        if (!string.IsNullOrEmpty(path))
        {
            _metadataService.Get(path);
            _mediaPlayerService.Load(path);
            _mediaPlayerService.PlayPause();
            _metadataService.Get(path);
            _currentFile = path;
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
        _metadataService.Get(Playlist[_currentIndex]);
        _mediaPlayerService.Load(Playlist[_currentIndex]); 
        _currentFile = Playlist[_currentIndex];
        _mediaPlayerService.PlayPause();
    }

private VideoMetaData showMetadata(string path)
{
    var metadata = _metadataService.Get(path);

    var duration = metadata.Duration.ToString(@"hh\:mm\:ss");

    return metadata;
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

    private void NextInPlaylist()
    {
        if (Playlist.Count == 0)
        {
            return;
        }
        _currentIndex = (_currentIndex + 1) % Playlist.Count;
        _mediaPlayerService.Load(Playlist[_currentIndex]);
    }

    private void PreviousInPlaylist()
    {
        if (Playlist.Count == 0)
        {
            return;
        }
        _currentIndex = (_currentIndex - 1 + Playlist.Count) % Playlist.Count;
        _mediaPlayerService.Load(Playlist[_currentIndex]);
    }

    // Test Menu Item Methods

    private void View()
    {
        if (_mediaPlayerService.IsPlaying)
        {
            var meta = showMetadata(_currentFile);
            MessageBox.Show(
                $"Title: {meta.Title}\n" +
                $"Duration: {meta.Duration:hh\\:mm\\:ss}"
            );
        }
        else
        {
            MessageBox.Show("The media is currently paused.");
        }
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


