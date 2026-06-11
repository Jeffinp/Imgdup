namespace Imgdup.App.Services;

public interface IRecycleBinService
{
    /// <summary>
    /// Sends the given files to the Windows Recycle Bin. Returns per-file results;
    /// failures (e.g. files >2GB, network paths, locked files) are reported, never silently swallowed.
    /// </summary>
    IReadOnlyList<DeleteResult> SendToRecycleBin(IEnumerable<string> paths);
}

public readonly record struct DeleteResult(string Path, bool Success, string? Error);
