using video.models;
namespace video.services.Interfaces
{
    public interface IVideoMetadataService
    {
        VideoMetaData Get(string path);
    }
}
