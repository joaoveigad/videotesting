using System;
using System.Windows.Controls;
using video.services;

namespace video.Services
{
    internal class MediaPlayerService : IMediaPlayerService
    {
        private readonly MediaElement _player;

        public bool IsPlaying { get; private set; }

        public MediaPlayerService(MediaElement player)
        {
            _player = player;
        }

        public void Load(string path)
        {
            _player.Source = new Uri(path);
        }

        public void Play()
        {
            _player.Play();
            IsPlaying = true;
        }

        public void Pause()
        {
            _player.Pause();
            IsPlaying = false;
        }

        public void Stop()
        {
            _player.Stop();
            IsPlaying = false;
        }
    }
}
