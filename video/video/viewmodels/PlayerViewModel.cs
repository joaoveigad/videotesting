using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using video.models;
using video.services.Interfaces;

public class PlayerViewModel : ViewModelBase
{
    private readonly IMediaDialogService _mediaDialogService;
    private readonly IMediaPlayerService _mediaPlayerService;
    private readonly IVideoMetadataService _metadataService;
    private readonly DispatcherTimer _timer;

    public ICommand PlayPauseCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand PreviousCommand { get; }
    public ICommand FileMetadata { get; }

    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    public ObservableCollection<string> Playlist { get; } = new();

    private int _currentIndex = -1;
    private string _currentFile = "";

    private TimeSpan _duration;
    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(TimeText));
        }
    }

    private TimeSpan _currentPosition;
    public TimeSpan CurrentPosition
    {
        get => _currentPosition;
        set
        {
            _currentPosition = value;
            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(TimeText));
        }
    }

    public string TimeText =>
        $"{CurrentPosition:mm\\:ss} / {Duration:mm\\:ss}";

    // Construtor

    public PlayerViewModel(
        IMediaDialogService mediaDialogService,
        IMediaPlayerService mediaPlayerService,
        IVideoMetadataService videoMetadataService)

    {
        _mediaDialogService = mediaDialogService;
        _mediaPlayerService = mediaPlayerService;
        _mediaPlayerService.MediaEnded += OnMediaEnded;
        _metadataService = videoMetadataService;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _timer.Tick += Timer_Tick;

        PlayPauseCommand = new RelayCommand(PlayPause);
        StopCommand = new RelayCommand(Stop);
        NextCommand = new RelayCommand(NextInPlaylist);
        PreviousCommand = new RelayCommand(PreviousInPlaylist);
        FileMetadata = new RelayCommand(() => ShowMetadata(_currentFile));

        MenuItems = new ObservableCollection<MenuItemViewModel>
        {
            new MenuItemViewModel(
                "File",
                children: new ObservableCollection<MenuItemViewModel>
                {
                    new MenuItemViewModel("Open", new RelayCommand(Open)),
                    new MenuItemViewModel("Open many", new RelayCommand(OpenMany))
                }
            ),
            new MenuItemViewModel("View", new RelayCommand(View)),
            new MenuItemViewModel("Audio", new RelayCommand(Audio)),
            new MenuItemViewModel("Video", new RelayCommand(Video)),
            new MenuItemViewModel("Subtitles", new RelayCommand(Subtitles)),
            new MenuItemViewModel("Help", new RelayCommand(Help))
        };
    }

    // Timer

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (!_mediaPlayerService.IsPlaying)
            return;

        CurrentPosition = _mediaPlayerService.Position;

        // lógica para fim do vídeo
        if (_mediaPlayerService.Position >= Duration)
        {
            OnMediaEnded(this, EventArgs.Empty);
            NextCommand.Execute(null);
            _mediaPlayerService.Stop();
        }

    }

    // File Operations

    private void Open()
    {
        var path = _mediaDialogService.OpenMediaFileDialog();
        if (string.IsNullOrEmpty(path))
            return;

        var metadata = _metadataService.Get(path);

        Duration = metadata.Duration;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Load(path);
        _mediaPlayerService.PlayPause();

        _timer.Start();
        _currentFile = path;
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {   
        if(Playlist.Count == 0) { 
            _mediaPlayerService.Stop();
            _timer.Stop();
            CurrentPosition = TimeSpan.Zero;
            return;
        }
        else
        {
            NextInPlaylist();
        }

    }

    private void OpenMany()
    {
        var paths = _mediaDialogService.OpenManyMediaFIleDialog();
        Playlist.Clear();

        foreach (var path in paths)
            Playlist.Add(path);

        if (Playlist.Count == 0)
            return;

        _currentIndex = 0;
        _currentFile = Playlist[_currentIndex];

        var metadata = _metadataService.Get(_currentFile);
        Duration = metadata.Duration;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Load(_currentFile);
        _mediaPlayerService.PlayPause();
        _timer.Start();
    }

    private VideoMetaData ShowMetadata(string path)
    {
        return _metadataService.Get(path);
    }


    // Playback / File Controls

    private void PlayPause()
    {
        _mediaPlayerService.PlayPause();

        if (_mediaPlayerService.IsPlaying)
            _timer.Start();
        else
            _timer.Stop();
    }

    private void Stop()
    {
        _mediaPlayerService.Stop();
        _timer.Stop();
        CurrentPosition = TimeSpan.Zero;
    }

    private void NextInPlaylist()
    {
        if (Playlist.Count == 0)
            return;

        _currentIndex = (_currentIndex + 1) % Playlist.Count;
        LoadFromPlaylist();

        //Lógica para fim da playlist
        if(_currentIndex == 0)
        {
            _mediaPlayerService.Stop();
            _timer.Stop();
            CurrentPosition = TimeSpan.Zero;
        }
    }

    private void PreviousInPlaylist()
    {
        if (Playlist.Count == 0)
            return;

        _currentIndex = (_currentIndex - 1 + Playlist.Count) % Playlist.Count;
        LoadFromPlaylist();
    }

    private void LoadFromPlaylist()
    {
        _currentFile = Playlist[_currentIndex];

        var metadata = _metadataService.Get(_currentFile);
        Duration = metadata.Duration;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Load(_currentFile);
        _mediaPlayerService.PlayPause();
        _timer.Start();
    }

    // Menu Item Actions
    private void View()
    {
        if (!_mediaPlayerService.IsPlaying)
        {
            MessageBox.Show("The media is currently paused.");
            return;
        }

        var meta = ShowMetadata(_currentFile);
        MessageBox.Show(
            $"Title: {meta.Title}\n" +
            $"Duration: {meta.Duration:hh\\:mm\\:ss}"
        );
    }

    private void Audio() => MessageBox.Show("Audio clicked!");
    private void Video() => MessageBox.Show("Video clicked!");
    private void Subtitles() => MessageBox.Show("Subtitles clicked!");
    private void Help() => MessageBox.Show("Help clicked!");
}
