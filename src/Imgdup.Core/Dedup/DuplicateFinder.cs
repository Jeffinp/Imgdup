using Imgdup.Core.Models;

namespace Imgdup.Core.Dedup;

/// <summary>
/// Groups hashed images into duplicate clusters. Exact (byte-identical) groups are reported
/// first; near-duplicate groups are then formed over one representative per exact-content set,
/// so a near group never restates copies already listed as exact duplicates.
/// </summary>
public static class DuplicateFinder
{
    public static IReadOnlyList<DuplicateGroup> Find(IReadOnlyList<ImageEntry> entries, ScanOptions options)
    {
        var groups = new List<DuplicateGroup>();
        var representatives = new List<ImageEntry>();

        // --- Exact duplicates: bucket by content hash ---
        foreach (var bucket in entries
                     .Where(e => e.ExactHash is not null)
                     .GroupBy(e => e.ExactHash!.Value))
        {
            var items = bucket.ToList();
            if (items.Count > 1)
            {
                groups.Add(new DuplicateGroup
                {
                    Id = Guid.NewGuid(),
                    Kind = MatchKind.Exact,
                    Items = Rank(items),
                });
            }

            // One representative per distinct content for the near-duplicate pass.
            representatives.Add(items[0]);
        }

        if (!options.DetectNearDuplicates)
            return groups;

        // --- Near duplicates: cluster representatives via BK-tree + union-find ---
        var candidates = representatives.Where(e => e.Perceptual is not null).ToList();
        var tree = new BkTree<int>();
        for (int i = 0; i < candidates.Count; i++)
            tree.Add(candidates[i].Perceptual!.Value, i);

        var uf = new UnionFind(candidates.Count);
        for (int i = 0; i < candidates.Count; i++)
        {
            foreach (int j in tree.Query(candidates[i].Perceptual!.Value, options.NearDuplicateThreshold))
            {
                if (j != i)
                    uf.Union(i, j);
            }
        }

        foreach (var cluster in Enumerable.Range(0, candidates.Count)
                     .GroupBy(uf.Find)
                     .Where(g => g.Count() > 1))
        {
            groups.Add(new DuplicateGroup
            {
                Id = Guid.NewGuid(),
                Kind = MatchKind.Near,
                Items = Rank(cluster.Select(i => candidates[i]).ToList()),
            });
        }

        return groups;
    }

    /// <summary>Orders a group so the suggested "keep" candidate (highest resolution, then oldest) is first.</summary>
    private static List<ImageEntry> Rank(List<ImageEntry> items) =>
        items
            .OrderByDescending(i => i.Pixels)
            .ThenBy(i => i.LastModifiedUtc)
            .ToList();
}
