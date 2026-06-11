namespace Imgdup.App.ViewModels;

/// <summary>Thumbnail size presets. The integer value is the box size in device-independent pixels.</summary>
public enum ThumbnailSize
{
    Small = 96,
    Medium = 160,
    Large = 256,
}

public enum SortMode
{
    DateDescending,
    DateAscending,
    SizeDescending,
    SizeAscending,
    NameAscending,
}
