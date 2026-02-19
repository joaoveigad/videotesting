using System.Collections.ObjectModel;
using System.IO;
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
    public RelayCommand ToggleMuteCommand { get; }
    public ObservableCollection<MenuItemViewModel> MenuItems { get; }
    public ObservableCollection<string> Playlist { get; } = new();


    //  Variáveis de estado
    private VideoMetaData? _currentMetadata;
    private int _currentIndex = -1;
    private string _currentFile = "";
    public bool IsUserDraggingSlider { get; set; } // Variáveis para controlar o slider no code-behind do xaml
    public bool IsPlaying => _mediaPlayerService.IsPlaying;

    public bool isLoaded = false; // Variável para controlar se um arquivo foi carregado, evitando erros de acesso a arquivos não carregados

    private double _volume = 1.0;

    public double Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0, 1);
            _mediaPlayerService.Volume = _volume;
            OnPropertyChanged(nameof(Volume));
        }
    }

    private double _lastVolume = 1.0;

    private bool _isMuted;

    public bool isMuted
    {
        get => _isMuted;
        set
        {
            _isMuted = value;
            _mediaPlayerService.Volume = _isMuted ? 0 : Volume;
            OnPropertyChanged(nameof(isMuted));
        }
    }

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
        ToggleMuteCommand = new RelayCommand(ToggleMute);

        MenuItems = new ObservableCollection<MenuItemViewModel>
        {
            new MenuItemViewModel(
                "File",
                children: new ObservableCollection<MenuItemViewModel>
                {
                    new MenuItemViewModel("Open", new RelayCommand(Open)),
                    new MenuItemViewModel("Open many", new RelayCommand(OpenMany)),
                    new MenuItemViewModel("Add to list", new RelayCommand(addToPlaylist))
                }
            ),
            new MenuItemViewModel("View",
            children: new ObservableCollection<MenuItemViewModel>
            {
                new MenuItemViewModel("File details", new RelayCommand(ViewFileData)),
                new MenuItemViewModel("Playlist details", new RelayCommand(ViewPlaylist))
            }
            ),
            new MenuItemViewModel("Audio", new RelayCommand(Audio)),
            new MenuItemViewModel("Video", new RelayCommand(Video)),
            new MenuItemViewModel("Subtitles", new RelayCommand(Subtitles)),
            new MenuItemViewModel("Help", new RelayCommand(Help))
        };
    }

    // Timer

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (!_mediaPlayerService.IsPlaying || IsUserDraggingSlider)
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
        if (Playlist.Count > 0) //evitar interações estranhas e playlists indesejadas
            Playlist.Clear();

        var path = _mediaDialogService.OpenMediaFileDialog();
        if (string.IsNullOrEmpty(path))
            return;

        Playlist.Add(path);

        _currentIndex = 0;
        _currentFile = Playlist[_currentIndex];

        _currentMetadata = _metadataService.Get(_currentFile);

        Duration = _currentMetadata.Duration;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Load(_currentFile);
        _mediaPlayerService.PlayPause();

        _timer.Start();
        isLoaded = true;
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

        _currentMetadata = _metadataService.Get(_currentFile);
        Duration = _currentMetadata.Duration;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Load(_currentFile);
        _mediaPlayerService.PlayPause();
        _timer.Start();
    }

    private void addToPlaylist()
    {
        if (_currentIndex == -1 && !string.IsNullOrEmpty(_currentFile))
        {
            Playlist.Clear();
            Playlist.Add(_currentFile);
            _currentIndex = 0;
        }

        var path = _mediaDialogService.OpenMediaFileDialog();
        if (string.IsNullOrEmpty(path))
            return;

        Playlist.Add(path);
    }

    private VideoMetaData? ShowMetadata(string path)
    {
        return _currentMetadata;
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
        Playlist.Clear();
        _currentIndex = -1;
        _currentFile = "";
        _currentMetadata = null;
        isLoaded = false;

        Duration = TimeSpan.Zero;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Stop();
        _timer.Stop();
    }

    private void NextInPlaylist()
    {
        if (Playlist.Count == 0)
            return;

        _currentIndex = (_currentIndex + 1) % Playlist.Count;
        LoadFromPlaylist();

        //Lógica para fim da playlist
        if (_currentIndex == 0)
        {
            _mediaPlayerService.Stop();
            _timer.Stop();
            CurrentPosition = TimeSpan.Zero;
        }
    }

    public void Seek(TimeSpan position)
    {
        _mediaPlayerService.Seek(position);
    }

    public void ToggleMute()
    {
        if (!isMuted)
        {
            _lastVolume = Volume == 0 ? 0.5 : Volume;
            isMuted = true;
        }
        else
        {
            isMuted = false;
            Volume = _lastVolume;
        }
    }

    private void PreviousInPlaylist()
    {
        if (_mediaPlayerService.Position > TimeSpan.FromSeconds(3))
        {
            _mediaPlayerService.Seek(TimeSpan.Zero);
            return;
        }

        if (Playlist.Count == 0)
            return;

        if (_currentIndex == 0)
        {
            LoadFromPlaylist();
            return;
        }

        _currentIndex--;
        LoadFromPlaylist();
    }

    private void LoadFromPlaylist()
    {
        _currentFile = Playlist[_currentIndex];

        _currentMetadata = _metadataService.Get(_currentFile);
        Duration = _currentMetadata.Duration;
        CurrentPosition = TimeSpan.Zero;

        _mediaPlayerService.Load(_currentFile);
        _mediaPlayerService.PlayPause();
        _timer.Start();
    }

    private void OnMediaEnded(object? sender, EventArgs e)
    {
        if (Playlist.Count == 0)
        {
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

    // Menu Item Actions
    private void ViewFileData()
    {
        if (!_mediaPlayerService.isLoaded || _currentMetadata == null)
        {
            MessageBox.Show("The o file loaded.");
            return;
        }

        var meta = _currentMetadata;
        MessageBox.Show(
            $"Title: {meta.Title}\n" +
            $"Duration: {meta.Duration:hh\\:mm\\:ss}"
        );
    }

    private void ViewPlaylist()
    {
        if (!_mediaPlayerService.isLoaded || Playlist.Count <= 0)
        {
            MessageBox.Show("There is no playlist loaded.");
            return;
        }

        // Converte a playlist para string
        string playlistText = string.Join(
            Environment.NewLine,
            Playlist.Select(p => Path.GetFileNameWithoutExtension(p))
        );

        MessageBox.Show(playlistText);
    }

    private void Audio() => MessageBox.Show("Audio clicked!");
    private void Video() => MessageBox.Show("Video clicked!");
    private void Subtitles() => MessageBox.Show("Subtitles clicked!");
    private void Help() => MessageBox.Show("Help clicked!");
}
