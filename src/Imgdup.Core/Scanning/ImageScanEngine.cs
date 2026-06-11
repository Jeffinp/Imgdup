using Imgdup.Core.Caching;
using Imgdup.Core.Dedup;
using Imgdup.Core.Hashing;
using Imgdup.Core.Models;

namespace Imgdup.Core.Scanning;

/// <summary>
/// Orchestrates a full duplicate scan: enumerate → hash (parallel) → cluster.
/// Exact hashing is computed only for files that share a byte size with another file,
/// since differing sizes can never be byte-identical.
/// </summary>
public sealed class ImageScanEngine(IImageHasher hasher, IHashCache? cache = null)
{
    public async Task<IReadOnlyList<DuplicateGroup>> ScanAsync(
        ScanOptions options,
        IProgress<ScanProgress>? progress = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        progress?.Report(new ScanProgress(ScanPhase.Enumerating, 0, 0, null));
        var entries = FileScanner.Enumerate(options).ToList();

        // Only files whose size collides with another can be exact duplicates.
        var sizeNeedsExact = entries
            .GroupBy(e => e.SizeBytes)
            .Where(g => g.Skip(1).Any())
            .SelectMany(g => g)
            .Select(e => e.Path)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        int processed = 0;
        int total = entries.Count;
        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = ct,
        };

        await Parallel.ForEachAsync(entries, parallelOptions, async (entry, token) =>
        {
            var cached = cache?.TryGet(entry.Path, entry.LastModifiedUtc, entry.SizeBytes);

            if (sizeNeedsExact.Contains(entry.Path))
                entry.ExactHash = cached?.ExactHash ?? await hasher.ComputeExactAsync(entry.Path, token).ConfigureAwait(false);

            if (options.DetectNearDuplicates)
            {
                if (cached?.Perceptual is { } cp)
                {
                    entry.Perceptual = cp;
                    entry.Width = cached.Width;
                    entry.Height = cached.Height;
                }
                else
                {
                    var result = hasher.ComputePerceptual(entry.Path);
                    if (result is { } r)
                    {
                        entry.Perceptual = r.Hash;
                        entry.Width = r.Width;
                        entry.Height = r.Height;
                    }
                    else
                    {
                        entry.DecodeFailed = true;
                    }
                }
            }

            cache?.Store(entry);

            int done = Interlocked.Increment(ref processed);
            progress?.Report(new ScanProgress(ScanPhase.Hashing, done, total, entry.Path));
        }).ConfigureAwait(false);

        progress?.Report(new ScanProgress(ScanPhase.Clustering, total, total, null));
        var groups = DuplicateFinder.Find(entries, options);

        progress?.Report(new ScanProgress(ScanPhase.Completed, total, total, null));
        return groups;
    }
}
