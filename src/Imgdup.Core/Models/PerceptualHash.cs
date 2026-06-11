using System.Globalization;
using System.Numerics;

namespace Imgdup.Core.Models;

/// <summary>
/// 64-bit perceptual hash. Similarity is measured by Hamming distance:
/// the number of differing bits. 0 = identical, 64 = fully different.
/// </summary>
public readonly record struct PerceptualHash(ulong Value)
{
    /// <summary>Number of differing bits between two hashes (0..64).</summary>
    public int DistanceTo(PerceptualHash other) =>
        BitOperations.PopCount(Value ^ other.Value);

    /// <summary>Similarity ratio in [0,1], where 1 means identical.</summary>
    public double SimilarityTo(PerceptualHash other) =>
        1.0 - DistanceTo(other) / 64.0;

    public override string ToString() => Value.ToString("x16", CultureInfo.InvariantCulture);
}
