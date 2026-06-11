using Imgdup.Core.Models;

namespace Imgdup.Core.Hashing;

/// <summary>Computes content and perceptual hashes for a single file.</summary>
public interface IImageHasher
{
    /// <summary>Streams the file and returns a fast non-cryptographic content hash (xxHash3).</summary>
    ValueTask<ulong> ComputeExactAsync(string path, CancellationToken ct = default);

    /// <summary>
    /// Decodes the image once and returns its perceptual hash plus dimensions.
    /// Returns <c>null</c> when the file cannot be decoded.
    /// </summary>
    PerceptualResult? ComputePerceptual(string path);
}

public readonly record struct PerceptualResult(PerceptualHash Hash, int Width, int Height);
