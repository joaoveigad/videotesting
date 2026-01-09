using System;
using System.Windows.Controls;
using video.services.Interfaces;

namespace video.services
{
    internal class MediaPlayerService : IMediaPlayerService
    {
        private readonly MediaElement _player;

        public bool IsPlaying { get; private set; }

        public TimeSpan Position => _player.Position;

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

        public void Stop()
        {
            _player.Stop();
            IsPlaying = false;
        }
    }
}
