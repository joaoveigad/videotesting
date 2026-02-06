using System.Windows.Controls;
using video.services.Interfaces;

internal class MediaPlayerService : IMediaPlayerService
{
    private readonly MediaElement _player;

    public bool IsPlaying { get; private set; }
    public bool isLoaded { get; set; }

    public TimeSpan Position => _player.Position;

    public double Volume
    {
        get => _player.Volume;
        set => _player.Volume = Math.Clamp(value, 0, 1);
    }

    public event EventHandler? MediaEnded;

    public MediaPlayerService(MediaElement player)
    {
        _player = player;
        _player.MediaEnded += (s, e) => MediaEnded?.Invoke(this, EventArgs.Empty);
        _player.LoadedBehavior = MediaState.Manual;
        _player.UnloadedBehavior = MediaState.Manual;
        _player.Volume = 1.0;
    }

    public void Load(string path)
    {
        IsPlaying = false;
        _player.Stop();
        _player.Source = new Uri(path);
        isLoaded = true;

    }

    public void PlayPause()
    {
        if (IsPlaying)
        {
            _player.Pause();
            IsPlaying = false;
        }
        else
        {
            _player.Play();
            IsPlaying = true;
        }
    }

    public void Seek(TimeSpan position)
    {
        if (_player.Source == null)
            return;

        if (!_player.NaturalDuration.HasTimeSpan)
            return;

        var duration = _player.NaturalDuration.TimeSpan;

        if (position < TimeSpan.Zero)
            position = TimeSpan.Zero;

        if (position > duration)
            position = duration;

        _player.Position = position;
    }

    public void Stop()
    {
        _player.Stop();
        _player.Source = null;
    }

}
