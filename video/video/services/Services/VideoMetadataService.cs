using TagLib;
using video.models;
using video.services.Interfaces;

namespace video.services.Services
{
    internal class VideoMetadataService : IVideoMetadataService
    {
        public VideoMetaData Get(string path)
        {
            var file = File.Create(path);

            return new VideoMetaData
            {
                Duration = file.Properties.Duration,
                Width = file.Properties.VideoWidth,
                Height = file.Properties.VideoHeight,
                Bitrate = file.Properties.AudioBitrate,
                Title = file.Tag.Title ?? System.IO.Path.GetFileNameWithoutExtension(path)
            };
        }
    }
}
