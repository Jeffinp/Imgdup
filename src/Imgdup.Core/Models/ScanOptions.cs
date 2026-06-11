namespace Imgdup.Core.Models;

public sealed record ScanOptions
{
    /// <summary>One or more root folders to scan.</summary>
    public required IReadOnlyList<string> Folders { get; init; }

    public bool Recursive { get; init; } = true;

    /// <summary>Files smaller than this are skipped (e.g. tiny icons). 0 disables the filter.</summary>
    public long MinFileSizeBytes { get; init; } = 1024;

    /// <summary>
    /// Maximum Hamming distance for two images to count as near-duplicates.
    /// Lower = stricter. Typical useful range is 0..10; 0 means perceptual-exact.
    /// </summary>
    public int NearDuplicateThreshold { get; init; } = 6;

    /// <summary>When false, only exact (byte-identical) duplicates are reported.</summary>
    public bool DetectNearDuplicates { get; init; } = true;

    /// <summary>Lower-case file extensions (with dot) treated as images.</summary>
    public IReadOnlySet<string> Extensions { get; init; } = DefaultExtensions;

    public static readonly IReadOnlySet<string> DefaultExtensions =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".webp", ".tiff", ".tif",
        };
}
