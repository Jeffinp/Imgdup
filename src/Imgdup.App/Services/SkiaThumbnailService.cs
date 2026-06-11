using System.Collections.Concurrent;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace Imgdup.App.Services;

/// <summary>
/// Produces thumbnails with SkiaSharp (fast hardware-friendly decode/resize) and converts them to
/// frozen WPF <see cref="BitmapSource"/>s. Decoded results are memoized per (path, size).
/// </summary>
public sealed class SkiaThumbnailService : IThumbnailService
{
    private readonly ConcurrentDictionary<(string Path, int Size), ImageSource> _cache = new();

    public Task<ImageSource?> GetThumbnailAsync(string path, int maxSize, CancellationToken ct = default)
    {
        if (_cache.TryGetValue((path, maxSize), out var cached))
            return Task.FromResult<ImageSource?>(cached);

        return Task.Run<ImageSource?>(() =>
        {
            ct.ThrowIfCancellationRequested();
            var source = Decode(path, maxSize);
            if (source is not null)
                _cache.TryAdd((path, maxSize), source);
            return source;
        }, ct);
    }

    private static WriteableBitmap? Decode(string path, int maxSize)
    {
        try
        {
            using var original = SKBitmap.Decode(path);
            if (original is null)
                return null;

            var (w, h) = FitWithin(original.Width, original.Height, maxSize);
            var info = new SKImageInfo(w, h, SKColorType.Bgra8888, SKAlphaType.Premul);
            using var resized = original.Resize(info, new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
            if (resized is null)
                return null;

            var writable = new WriteableBitmap(w, h, 96, 96, PixelFormats.Pbgra32, palette: null);
            writable.Lock();
            try
            {
                resized.GetPixelSpan().CopyTo(GetBackBufferSpan(writable, resized.ByteCount));
                writable.AddDirtyRect(new System.Windows.Int32Rect(0, 0, w, h));
            }
            finally
            {
                writable.Unlock();
            }

            writable.Freeze(); // cross-thread safe once frozen
            return writable;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return null;
        }
    }

    private static unsafe Span<byte> GetBackBufferSpan(WriteableBitmap bitmap, int length) =>
        new((void*)bitmap.BackBuffer, length);

    private static (int Width, int Height) FitWithin(int width, int height, int box)
    {
        if (width <= 0 || height <= 0)
            return (box, box);

        double scale = Math.Min((double)box / width, (double)box / height);
        scale = Math.Min(scale, 1.0); // never upscale
        return (Math.Max(1, (int)(width * scale)), Math.Max(1, (int)(height * scale)));
    }
}
