
namespace video.services.Interfaces
{
    public interface IMediaPlayerService
    {
        void PlayPause();
        void Stop();
        void Load(string path);
        void Seek(TimeSpan position);
        bool IsPlaying { get; }
        TimeSpan Position { get; }

        event EventHandler MediaEnded;
    }
}
