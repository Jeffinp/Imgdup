using Imgdup.Core.Models;

namespace Imgdup.Core.Tests;

public class PerceptualHashTests
{
    [Fact]
    public void DistanceTo_IdenticalHashes_IsZero()
    {
        var a = new PerceptualHash(0xDEADBEEFCAFEF00D);
        Assert.Equal(0, a.DistanceTo(a));
        Assert.Equal(1.0, a.SimilarityTo(a));
    }

    [Fact]
    public void DistanceTo_CountsDifferingBits()
    {
        var a = new PerceptualHash(0b0000);
        var b = new PerceptualHash(0b1011);
        Assert.Equal(3, a.DistanceTo(b));
    }

    [Fact]
    public void DistanceTo_AllBitsDiffer_Is64()
    {
        var a = new PerceptualHash(0UL);
        var b = new PerceptualHash(ulong.MaxValue);
        Assert.Equal(64, a.DistanceTo(b));
        Assert.Equal(0.0, a.SimilarityTo(b));
    }
}
