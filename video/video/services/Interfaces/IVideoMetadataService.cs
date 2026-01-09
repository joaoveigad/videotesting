using video.models;
namespace video.services.Interfaces
{
    internal interface IVideoMetadataService
    {
        VideoMetaData Get(string path);
    }
}
