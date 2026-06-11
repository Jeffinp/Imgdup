namespace Imgdup.Core.Models;

public enum MatchKind
{
    /// <summary>Byte-identical content (same exact hash).</summary>
    Exact,
    /// <summary>Visually similar within the configured Hamming threshold.</summary>
    Near,
}

/// <summary>A cluster of two or more images considered duplicates of each other.</summary>
public sealed record DuplicateGroup
{
    public required Guid Id { get; init; }
    public required MatchKind Kind { get; init; }
    public required IReadOnlyList<ImageEntry> Items { get; init; }

    /// <summary>Total bytes that could be reclaimed by keeping a single copy.</summary>
    public long ReclaimableBytes => Items.Skip(1).Sum(i => i.SizeBytes);

    public int Count => Items.Count;
}
