using Imgdup.Core.Models;
using Microsoft.Data.Sqlite;

namespace Imgdup.Core.Caching;

/// <summary>
/// SQLite-backed hash cache. A single connection is guarded by a lock: scan workers
/// hit the cache in short bursts, so serialized access is cheaper than the I/O it avoids.
/// </summary>
public sealed class SqliteHashCache : IHashCache, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly Lock _gate = new();

    public SqliteHashCache(string databasePath)
    {
        _connection = new SqliteConnection($"Data Source={databasePath}");
        _connection.Open();

        using var pragma = _connection.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
        pragma.ExecuteNonQuery();

        using var create = _connection.CreateCommand();
        create.CommandText =
            """
            CREATE TABLE IF NOT EXISTS hashes (
                path   TEXT PRIMARY KEY,
                mtime  INTEGER NOT NULL,
                size   INTEGER NOT NULL,
                exact  INTEGER NULL,
                phash  INTEGER NULL,
                width  INTEGER NULL,
                height INTEGER NULL
            );
            """;
        create.ExecuteNonQuery();
    }

    public CachedHashes? TryGet(string path, DateTime lastModifiedUtc, long sizeBytes)
    {
        lock (_gate)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText =
                "SELECT exact, phash, width, height FROM hashes WHERE path = $p AND mtime = $m AND size = $s;";
            cmd.Parameters.AddWithValue("$p", path);
            cmd.Parameters.AddWithValue("$m", lastModifiedUtc.Ticks);
            cmd.Parameters.AddWithValue("$s", sizeBytes);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new CachedHashes(
                ExactHash: reader.IsDBNull(0) ? null : unchecked((ulong)reader.GetInt64(0)),
                Perceptual: reader.IsDBNull(1) ? null : new PerceptualHash(unchecked((ulong)reader.GetInt64(1))),
                Width: reader.IsDBNull(2) ? null : reader.GetInt32(2),
                Height: reader.IsDBNull(3) ? null : reader.GetInt32(3));
        }
    }

    public void Store(ImageEntry entry)
    {
        lock (_gate)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText =
                """
                INSERT INTO hashes (path, mtime, size, exact, phash, width, height)
                VALUES ($p, $m, $s, $e, $h, $w, $ht)
                ON CONFLICT(path) DO UPDATE SET
                    mtime=$m, size=$s, exact=$e, phash=$h, width=$w, height=$ht;
                """;
            cmd.Parameters.AddWithValue("$p", entry.Path);
            cmd.Parameters.AddWithValue("$m", entry.LastModifiedUtc.Ticks);
            cmd.Parameters.AddWithValue("$s", entry.SizeBytes);
            cmd.Parameters.AddWithValue("$e", (object?)(entry.ExactHash is { } ex ? unchecked((long)ex) : null) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$h", (object?)(entry.Perceptual is { } ph ? unchecked((long)ph.Value) : null) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$w", (object?)entry.Width ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$ht", (object?)entry.Height ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }
    }

    public void Dispose() => _connection.Dispose();
}
