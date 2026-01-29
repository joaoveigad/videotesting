using System;
using System.Windows.Controls;
using video.services.Interfaces;

namespace video.services
{
    internal class MediaPlayerService : IMediaPlayerService
    {
        private readonly MediaElement _player;

        public bool IsPlaying { get; private set; }
        double Volume { get; set; }

        public TimeSpan Position => _player.Position;

        double IMediaPlayerService.Volume { get => Volume; set => Volume = value; }

        public event EventHandler? MediaEnded;

        public MediaPlayerService(MediaElement player)
        {
            _player = player;
            _player.MediaEnded += (s, e) => MediaEnded?.Invoke(this, EventArgs.Empty);
            _player.LoadedBehavior = MediaState.Manual;
            _player.UnloadedBehavior = MediaState.Manual;
        }

        public void Load(string path)
        {
            IsPlaying = false;
            _player.Stop();
            _player.Source = new Uri(path);
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
            IsPlaying = false;
        }
    }
}
