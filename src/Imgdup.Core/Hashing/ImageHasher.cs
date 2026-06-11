using System.IO.Hashing;
using Imgdup.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using DifferenceHash = CoenM.ImageHash.HashAlgorithms.DifferenceHash;

namespace Imgdup.Core.Hashing;

public sealed class ImageHasher : IImageHasher
{
    // DifferenceHash (dHash): fast, robust to scaling/brightness/aspect changes.
    // Stateless and thread-safe, so a single shared instance is reused across parallel work.
    private readonly DifferenceHash _perceptual = new();

    private const int StreamBufferSize = 1 << 20; // 1 MiB

    public async ValueTask<ulong> ComputeExactAsync(string path, CancellationToken ct = default)
    {
        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            StreamBufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);

        var hasher = new XxHash3();
        var buffer = new byte[StreamBufferSize];
        int read;
        while ((read = await stream.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
            hasher.Append(buffer.AsSpan(0, read));

        return hasher.GetCurrentHashAsUInt64();
    }

    public PerceptualResult? ComputePerceptual(string path)
    {
        try
        {
            using var image = Image.Load<Rgba32>(path);
            var hash = new PerceptualHash(_perceptual.Hash(image));
            return new PerceptualResult(hash, image.Width, image.Height);
        }
        catch (Exception ex) when (ex is UnknownImageFormatException or InvalidImageContentException or IOException)
        {
            // Corrupt, truncated, or unsupported encoding: treat as undecodable rather than failing the scan.
            return null;
        }
    }
}
