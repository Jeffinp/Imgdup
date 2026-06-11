namespace Imgdup.Core.Models;

public enum ScanPhase
{
    Enumerating,
    Hashing,
    Clustering,
    Completed,
}

/// <summary>Progress snapshot reported during a scan. Immutable; safe to marshal to the UI thread.</summary>
public readonly record struct ScanProgress(
    ScanPhase Phase,
    int Processed,
    int Total,
    string? CurrentFile)
{
    public double Fraction => Total > 0 ? (double)Processed / Total : 0;
}
