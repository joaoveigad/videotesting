namespace video.services
{
    public interface IMediaPlayerService
    {
        void Play();
        void Pause();
        void Stop();
        void Load(string path);
        bool IsPlaying { get; }
    }
}
