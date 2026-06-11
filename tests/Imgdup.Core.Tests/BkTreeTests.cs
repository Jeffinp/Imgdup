using Imgdup.Core.Dedup;
using Imgdup.Core.Models;

namespace Imgdup.Core.Tests;

public class BkTreeTests
{
    [Fact]
    public void Query_ReturnsOnlyHashesWithinRadius()
    {
        var tree = new BkTree<string>();
        tree.Add(new PerceptualHash(0b0000), "a");
        tree.Add(new PerceptualHash(0b0001), "b"); // distance 1 from a
        tree.Add(new PerceptualHash(0b0111), "c"); // distance 3 from a
        tree.Add(new PerceptualHash(ulong.MaxValue), "far");

        var hits = tree.Query(new PerceptualHash(0b0000), maxDistance: 1);

        Assert.Equal(["a", "b"], hits.Order());
    }

    [Fact]
    public void Query_EmptyTree_ReturnsEmpty()
    {
        var tree = new BkTree<int>();
        Assert.Empty(tree.Query(new PerceptualHash(123), 5));
    }

    [Fact]
    public void Query_ZeroRadius_ReturnsExactMatchesOnly()
    {
        var tree = new BkTree<string>();
        tree.Add(new PerceptualHash(42), "x");
        tree.Add(new PerceptualHash(42), "y");
        tree.Add(new PerceptualHash(43), "z");

        var hits = tree.Query(new PerceptualHash(42), maxDistance: 0);

        Assert.Equal(["x", "y"], hits.Order());
    }
}
