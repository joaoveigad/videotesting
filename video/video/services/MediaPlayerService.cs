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

        public void PlayPause()
        {   
            if(IsPlaying)
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
