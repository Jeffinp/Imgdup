using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Imgdup.App.Services;

/// <summary>
/// Deletes files to the Recycle Bin via <see cref="FileSystem.DeleteFile(string, UIOption, RecycleOption)"/>.
/// This is reversible by design: the user can restore from the Recycle Bin. Permanent deletion is never performed here.
/// </summary>
public sealed class RecycleBinService : IRecycleBinService
{
    public IReadOnlyList<DeleteResult> SendToRecycleBin(IEnumerable<string> paths)
    {
        ArgumentNullException.ThrowIfNull(paths);

        var results = new List<DeleteResult>();
        foreach (var path in paths)
        {
            try
            {
                FileSystem.DeleteFile(
                    path,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin,
                    UICancelOption.ThrowException);
                results.Add(new DeleteResult(path, Success: true, Error: null));
            }
            catch (Exception ex) when (ex is OperationCanceledException or IOException or UnauthorizedAccessException or FileNotFoundException)
            {
                results.Add(new DeleteResult(path, Success: false, Error: ex.Message));
            }
        }

        return results;
    }
}
