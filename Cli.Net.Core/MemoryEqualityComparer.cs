namespace Cli.Net.Core;

public sealed class MemoryEqualityComparer : IEqualityComparer<ReadOnlyMemory<char>>
{
    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return x.Span.SequenceEqual(y.Span);
    }

    public int GetHashCode(ReadOnlyMemory<char> obj)
    {
        return string.GetHashCode(obj.Span);
    }
}
