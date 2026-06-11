using Imgdup.Core.Models;

namespace Imgdup.Core.Caching;

/// <summary>
/// Persists computed hashes keyed by path + last-modified + size, so unchanged files
/// are not re-decoded on subsequent scans.
/// </summary>
public interface IHashCache
{
    CachedHashes? TryGet(string path, DateTime lastModifiedUtc, long sizeBytes);
    void Store(ImageEntry entry);
}

public sealed record CachedHashes(
    ulong? ExactHash,
    PerceptualHash? Perceptual,
    int? Width,
    int? Height);
