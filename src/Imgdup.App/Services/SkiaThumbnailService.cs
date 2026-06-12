using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SkiaSharp;

namespace Imgdup.App.Services;

/// <summary>
/// Produces thumbnails with SkiaSharp and converts them to frozen WPF BitmapSources.
/// Uses a bounded LRU cache (400 entries ≈ 100 MB) to cap resident memory.
/// Tracks native pixel buffer memory with GC.AddMemoryPressure so the collector
/// runs before the process hits address-space limits.
/// </summary>
public sealed class SkiaThumbnailService : IThumbnailService
{
    // 400 × 256×256×4 bytes ≈ 100 MB upper bound
    private const int CacheCapacity = 400;
    private const int BytesPerPixel = 4;

    private readonly LruCache<(string Path, int Size), BitmapSource> _cache = new(CacheCapacity);

    public Task<ImageSource?> GetThumbnailAsync(string path, int maxSize, CancellationToken ct = default)
    {
        if (_cache.TryGet((path, maxSize), out var cached))
            return Task.FromResult<ImageSource?>(cached);

        return Task.Run<ImageSource?>(() =>
        {
            ct.ThrowIfCancellationRequested();
            var bitmap = Decode(path, maxSize);
            if (bitmap is null)
                return null;

            long bytes = NativeBytes(bitmap);
            GC.AddMemoryPressure(bytes);

            var evicted = _cache.Add((path, maxSize), bitmap);
            if (evicted is not null)
                GC.RemoveMemoryPressure(NativeBytes(evicted));

            return bitmap;
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

            writable.Freeze();
            return writable;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return null;
        }
    }

    private static long NativeBytes(BitmapSource b) =>
        (long)b.PixelWidth * b.PixelHeight * BytesPerPixel;

    private static unsafe Span<byte> GetBackBufferSpan(WriteableBitmap bitmap, int length) =>
        new((void*)bitmap.BackBuffer, length);

    private static (int Width, int Height) FitWithin(int width, int height, int box)
    {
        if (width <= 0 || height <= 0)
            return (box, box);

        double scale = Math.Min((double)box / width, (double)box / height);
        scale = Math.Min(scale, 1.0);
        return (Math.Max(1, (int)(width * scale)), Math.Max(1, (int)(height * scale)));
    }
}
