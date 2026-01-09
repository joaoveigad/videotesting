namespace video.models
{
    public class VideoMetaData
    {
        public TimeSpan Duration { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Bitrate { get; set; }
        public required string Title { get; set; }
    }
}
