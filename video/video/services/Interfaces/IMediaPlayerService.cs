namespace video.services.Interfaces
{
    public interface IMediaPlayerService
    {
        void PlayPause();
        void Stop();
        void Load(string path);
        void Seek(TimeSpan position);
        bool IsPlaying { get; }
        bool isLoaded { get; set; }
        TimeSpan Position { get; }
        double Volume { get; set; }

        event EventHandler MediaEnded;
    }
}
