using Microsoft.Win32;

namespace Imgdup.App.Services;

/// <summary>Native folder picker using <see cref="OpenFolderDialog"/> (WPF, .NET 8+), with multi-select enabled.</summary>
public sealed class FolderPickerService : IFolderPickerService
{
    public IReadOnlyList<string> PickFolders()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Selecione uma ou mais pastas para verificar",
            Multiselect = true,
        };

        return dialog.ShowDialog() == true ? dialog.FolderNames : [];
    }
}
