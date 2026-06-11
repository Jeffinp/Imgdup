namespace Imgdup.Core.Models;

/// <summary>
/// An image discovered during a scan, together with its computed hashes.
/// Hashes are nullable because population happens in a later pipeline stage.
/// </summary>
public sealed record ImageEntry
{
    public required string Path { get; init; }
    public required long SizeBytes { get; init; }
    public required DateTime LastModifiedUtc { get; init; }

    /// <summary>Fast non-cryptographic content hash (xxHash3) for exact-duplicate grouping.</summary>
    public ulong? ExactHash { get; set; }

    /// <summary>Perceptual hash for near-duplicate grouping. Null if the image failed to decode.</summary>
    public PerceptualHash? Perceptual { get; set; }

    public int? Width { get; set; }
    public int? Height { get; set; }

    /// <summary>True when decoding failed (corrupt/unsupported file). Excluded from near-dup matching.</summary>
    public bool DecodeFailed { get; set; }

    public long Pixels => (long)(Width ?? 0) * (Height ?? 0);
}
