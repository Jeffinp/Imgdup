namespace Imgdup.Core.Dedup;

/// <summary>Disjoint-set with path compression and union by rank. Used to merge transitive near-duplicate links.</summary>
internal sealed class UnionFind
{
    private readonly int[] _parent;
    private readonly int[] _rank;

    public UnionFind(int size)
    {
        _parent = new int[size];
        _rank = new int[size];
        for (int i = 0; i < size; i++)
            _parent[i] = i;
    }

    public int Find(int x)
    {
        while (_parent[x] != x)
        {
            _parent[x] = _parent[_parent[x]]; // path halving
            x = _parent[x];
        }
        return x;
    }

    public void Union(int a, int b)
    {
        int ra = Find(a), rb = Find(b);
        if (ra == rb)
            return;

        if (_rank[ra] < _rank[rb])
            (ra, rb) = (rb, ra);

        _parent[rb] = ra;
        if (_rank[ra] == _rank[rb])
            _rank[ra]++;
    }
}
