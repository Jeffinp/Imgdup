namespace Imgdup.App.Services;

public interface IFolderPickerService
{
    /// <summary>Shows a native multi-select folder dialog. Returns the chosen folders, or empty if cancelled.</summary>
    IReadOnlyList<string> PickFolders();
}
