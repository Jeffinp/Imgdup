using System.Collections.ObjectModel;
using Imgdup.App.Services;
using Imgdup.Core.Models;

namespace Imgdup.App.ViewModels;

/// <summary>A duplicate cluster shown as a section in the gallery.</summary>
public sealed class DuplicateGroupViewModel
{
    public DuplicateGroupViewModel(DuplicateGroup group, IThumbnailService thumbnails)
    {
        Kind = group.Kind;
        ReclaimableBytes = group.ReclaimableBytes;

        var items = group.Items
            .Select((e, index) => new ImageItemViewModel(e, thumbnails) { IsKeepCandidate = index == 0 })
            .ToList();
        Items = new ObservableCollection<ImageItemViewModel>(items);
        foreach (var item in Items)
            item.Group = this;
    }

    public MatchKind Kind { get; }
    public long ReclaimableBytes { get; }
    public ObservableCollection<ImageItemViewModel> Items { get; }

    public string Header =>
        $"{(Kind == MatchKind.Exact ? "Idênticas" : "Similares")} · {Items.Count} arquivos · recuperável {FormatBytes(ReclaimableBytes)}";

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
