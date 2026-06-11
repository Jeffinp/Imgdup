using System.Windows.Media;

namespace Imgdup.App.Services;

public interface IThumbnailService
{
    /// <summary>
    /// Decodes and downscales an image to fit within a <paramref name="maxSize"/>×<paramref name="maxSize"/> box.
    /// Returns a frozen, UI-thread-safe <see cref="ImageSource"/>, or null if decoding fails.
    /// </summary>
    Task<ImageSource?> GetThumbnailAsync(string path, int maxSize, CancellationToken ct = default);
}
