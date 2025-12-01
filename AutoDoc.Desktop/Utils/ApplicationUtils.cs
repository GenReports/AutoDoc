using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace AutoDoc.UI.Utils
{
    public static class ApplicationUtils
    {
        public static Window GetCurrentWindow()
        {
            return (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
                .MainWindow!;
        }

        //TODO: Add notification method

        public static async Task<string> PickFolderAsync(Window window)
        {
            var topLevel = TopLevel.GetTopLevel(window)!;

            var result = await topLevel.StorageProvider.OpenFolderPickerAsync(
                new FolderPickerOpenOptions
                {
                    Title = "Select a folder",
                    AllowMultiple = false
                });

            if (result.Count > 0)
            {
                return result[0].Path.LocalPath;
            }

            return string.Empty;
        }
    }
}
