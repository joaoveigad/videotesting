using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.Win32;

namespace video.Services
{
    public class MediaDialogService : IMediaDialogService
    {
        private readonly OpenFileDialog _dialog = new()
        {
            Title = "Open a media file",
            Filter = "Media Files (*.mp3,*.mp4)|*.mp3;*.mp4"
        };

        public string? OpenMediaFile()
        {
            return _dialog.ShowDialog() == true
                ? _dialog.FileName
                : null;
        }

        public string? OpenMediaFileDialog()
        {
            return OpenMediaFile();
        }

        public IEnumerable<string> OpenManyMediaFIleDialog()
        {
            _dialog.Multiselect = true;
            return _dialog.ShowDialog() == true
                ? _dialog.FileNames
                : Array.Empty<string>();
        }
    }
}
