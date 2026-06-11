using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Imgdup.App.Services;
using Imgdup.Core.Models;

namespace Imgdup.App.ViewModels;

/// <summary>One image inside a duplicate group. Thumbnails load lazily and asynchronously.</summary>
public sealed partial class ImageItemViewModel(ImageEntry entry, IThumbnailService thumbnails) : ObservableObject
{
    private readonly IThumbnailService _thumbnails = thumbnails;

    public ImageEntry Entry { get; } = entry;

    /// <summary>The duplicate group this item belongs to. Used for grouping in the gallery view.</summary>
    public DuplicateGroupViewModel? Group { get; set; }

    public string Path => Entry.Path;
    public string FileName => System.IO.Path.GetFileName(Entry.Path);
    public string Folder => System.IO.Path.GetDirectoryName(Entry.Path) ?? string.Empty;
    public long SizeBytes => Entry.SizeBytes;
    public DateTime LastModified => Entry.LastModifiedUtc.ToLocalTime();
    public string Dimensions => Entry is { Width: { } w, Height: { } h } ? $"{w}×{h}" : "—";
    public string SizeDisplay => FormatBytes(Entry.SizeBytes);

    /// <summary>True when this is the group's suggested file to keep (ranked first).</summary>
    [ObservableProperty]
    private bool _isKeepCandidate;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private ImageSource? _thumbnail;

    private bool _thumbnailRequested;

    /// <summary>Triggers thumbnail decode on first request for the given size. Idempotent per size change.</summary>
    public async Task EnsureThumbnailAsync(int maxSize, CancellationToken ct = default)
    {
        if (_thumbnailRequested && Thumbnail is not null)
            return;

        _thumbnailRequested = true;
        try
        {
            Thumbnail = await _thumbnails.GetThumbnailAsync(Path, maxSize, ct).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            _thumbnailRequested = false;
        }
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB"];
        double value = bytes;
        int unit = 0;
        while (value >= 1024 && unit < units.Length - 1)
        {
            value /= 1024;
            unit++;
        }
        return $"{value:0.#} {units[unit]}";
    }
}
