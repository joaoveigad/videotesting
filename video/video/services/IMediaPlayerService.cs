namespace video.services
{
    public interface IMediaPlayerService
    {
        void PlayPause();
        void Stop();
        void Load(string path);
        bool IsPlaying { get; }
    }
}
