
namespace video.services.Interfaces
{
    public interface IMediaPlayerService
    {
        void PlayPause();
        void Stop();
        void Load(string path);
        bool IsPlaying { get; }
        TimeSpan Position { get; }

        event EventHandler MediaEnded;
    }
}
