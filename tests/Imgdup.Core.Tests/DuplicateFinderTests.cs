using Imgdup.Core.Dedup;
using Imgdup.Core.Models;

namespace Imgdup.Core.Tests;

public class DuplicateFinderTests
{
    private static ImageEntry Entry(string path, ulong? exact = null, ulong? phash = null, int w = 100, int h = 100) =>
        new()
        {
            Path = path,
            SizeBytes = 1000,
            LastModifiedUtc = DateTime.UnixEpoch,
            ExactHash = exact,
            Perceptual = phash is { } p ? new PerceptualHash(p) : null,
            Width = w,
            Height = h,
        };

    private static ScanOptions Options(bool near = true, int threshold = 2) => new()
    {
        Folders = [],
        DetectNearDuplicates = near,
        NearDuplicateThreshold = threshold,
    };

    [Fact]
    public void Find_GroupsExactDuplicatesByContentHash()
    {
        var entries = new[]
        {
            Entry("a.jpg", exact: 111),
            Entry("b.jpg", exact: 111),
            Entry("c.jpg", exact: 222),
        };

        var groups = DuplicateFinder.Find(entries, Options(near: false));

        var group = Assert.Single(groups);
        Assert.Equal(MatchKind.Exact, group.Kind);
        Assert.Equal(2, group.Count);
    }

    [Fact]
    public void Find_ClustersNearDuplicatesWithinThreshold()
    {
        var entries = new[]
        {
            Entry("a.jpg", exact: 1, phash: 0b0000),
            Entry("b.jpg", exact: 2, phash: 0b0001), // distance 1 -> near of a
            Entry("c.jpg", exact: 3, phash: ulong.MaxValue),
        };

        var groups = DuplicateFinder.Find(entries, Options(threshold: 2));

        var near = Assert.Single(groups, g => g.Kind == MatchKind.Near);
        Assert.Equal(2, near.Count);
    }

    [Fact]
    public void Find_RanksHighestResolutionFirst()
    {
        var entries = new[]
        {
            Entry("small.jpg", exact: 5, w: 50, h: 50),
            Entry("large.jpg", exact: 5, w: 500, h: 500),
        };

        var group = Assert.Single(DuplicateFinder.Find(entries, Options(near: false)));
        Assert.Equal("large.jpg", group.Items[0].Path); // suggested keep = biggest
    }

    [Fact]
    public void Find_NoDuplicates_ReturnsEmpty()
    {
        var entries = new[] { Entry("a.jpg", exact: 1, phash: 0), Entry("b.jpg", exact: 2, phash: ulong.MaxValue) };
        Assert.Empty(DuplicateFinder.Find(entries, Options(threshold: 1)));
    }
}
