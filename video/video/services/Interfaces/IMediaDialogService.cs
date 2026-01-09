namespace video.services.Interfaces
{
    public interface IMediaDialogService
    {
        string? OpenMediaFileDialog();
        IEnumerable<string> OpenManyMediaFIleDialog();

    }
}
