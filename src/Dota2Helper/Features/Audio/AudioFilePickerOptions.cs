using Avalonia.Platform.Storage;

namespace Dota2Helper.Features.Audio;

public class AudioFilePickerOptions : FilePickerOpenOptions
{
    public AudioFilePickerOptions()
    {
        Title = "Select audio file";
        AllowMultiple = false;
        FileTypeFilter =
        [
            new FilePickerFileType("Audio files")
            {
                Patterns = ["*.mp3", "*.wav"],
                MimeTypes = ["audio/*"]
            },
        ];
    }
}