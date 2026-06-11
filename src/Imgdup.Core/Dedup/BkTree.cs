using Imgdup.Core.Models;

namespace Imgdup.Core.Dedup;

/// <summary>
/// Burkhard-Keller tree over perceptual hashes using Hamming distance.
/// Enables radius queries ("all hashes within distance N") in roughly O(log n)
/// instead of comparing every pair (O(n²)).
/// </summary>
/// <remarks>Not thread-safe. Build on a single thread, then query.</remarks>
public sealed class BkTree<T>
{
    private sealed class Node(PerceptualHash hash, T value)
    {
        public PerceptualHash Hash { get; } = hash;
        public T Value { get; } = value;
        public Dictionary<int, Node> Children { get; } = [];
    }

    private Node? _root;

    public void Add(PerceptualHash hash, T value)
    {
        if (_root is null)
        {
            _root = new Node(hash, value);
            return;
        }

        var node = _root;
        while (true)
        {
            int distance = node.Hash.DistanceTo(hash);
            if (node.Children.TryGetValue(distance, out var child))
            {
                node = child;
                continue;
            }

            node.Children[distance] = new Node(hash, value);
            return;
        }
    }

    /// <summary>Returns every stored value whose hash is within <paramref name="maxDistance"/> of <paramref name="query"/>.</summary>
    public List<T> Query(PerceptualHash query, int maxDistance)
    {
        var results = new List<T>();
        if (_root is null)
            return results;

        var stack = new Stack<Node>();
        stack.Push(_root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            int distance = node.Hash.DistanceTo(query);

            if (distance <= maxDistance)
                results.Add(node.Value);

            // Triangle inequality: only branches within [d-max, d+max] can contain matches.
            int low = distance - maxDistance;
            int high = distance + maxDistance;
            foreach (var (edge, child) in node.Children)
            {
                if (edge >= low && edge <= high)
                    stack.Push(child);
            }
        }

        return results;
    }
}
