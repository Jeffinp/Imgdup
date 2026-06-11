using Imgdup.Core.Models;

namespace Imgdup.Core.Scanning;

/// <summary>Enumerates candidate image files under the configured folders, applying extension and size filters.</summary>
public static class FileScanner
{
    public static IEnumerable<ImageEntry> Enumerate(ScanOptions options)
    {
        var searchOption = options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var folder in options.Folders)
        {
            if (!Directory.Exists(folder))
                continue;

            var enumOptions = new EnumerationOptions
            {
                RecurseSubdirectories = options.Recursive,
                IgnoreInaccessible = true,
                AttributesToSkip = FileAttributes.System | FileAttributes.ReparsePoint,
            };

            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(folder, "*", enumOptions);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or DirectoryNotFoundException or IOException)
            {
                continue;
            }

            foreach (var path in files)
            {
                if (!options.Extensions.Contains(Path.GetExtension(path)))
                    continue;

                FileInfo info;
                try
                {
                    info = new FileInfo(path);
                    if (info.Length < options.MinFileSizeBytes)
                        continue;
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
                {
                    continue;
                }

                // Folders may overlap; emit each physical file once.
                if (!seen.Add(info.FullName))
                    continue;

                yield return new ImageEntry
                {
                    Path = info.FullName,
                    SizeBytes = info.Length,
                    LastModifiedUtc = info.LastWriteTimeUtc,
                };
            }
        }
    }
}
