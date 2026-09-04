using System.Collections.Frozen;

namespace Cli.Net.Core;

public static class ArgumentsParser
{
    public static IReadOnlyDictionary<ReadOnlyMemory<char>, IReadOnlyCollection<ReadOnlyMemory<char>>>? Parse(
        ReadOnlyMemory<char> input, char flagStart = '-')
    {
        var dict = new Dictionary<ReadOnlyMemory<char>, IReadOnlyCollection<ReadOnlyMemory<char>>>(new MemoryEqualityComparer());

        ReadOnlyMemory<char> currentFlag = ReadOnlyMemory<char>.Empty;
        
        while (TryGetNextToken(input, out var token, out var skip, out var escaped))
        {
            if (!escaped && token.Span[0] == flagStart)
            {
                if (!dict.TryGetValue(token, out var value))
                {
                    value = new List<ReadOnlyMemory<char>>();
                    dict[token] = value;
                }
                
                currentFlag = token;
            }
            else
            {
                if (currentFlag.Equals(ReadOnlyMemory<char>.Empty)) return null;

                ((List<ReadOnlyMemory<char>>)dict[currentFlag]).Add(token);
            }
            
            input = input[skip..];
        }

        return dict;
    }

    private static bool TryGetNextToken(
        ReadOnlyMemory<char> input,
        out ReadOnlyMemory<char> token,
        out int skip,
        out bool escaped)
    {
        var length = input.Length;
        input = input.TrimStart();
        
        skip = length - input.Length;
        
        escaped = false;
        
        if (input.IsEmpty)
        {
            token = Memory<char>.Empty;
            return false;
        }

        if (input.Span[0] == '"')
        {
            input = input[1..];
            var closingIndex = input.Span.IndexOfUnescapedQuote();
            token = input[..closingIndex];
            skip += closingIndex + 2;
            escaped = true;
            return true;
        }
        
        var spaceIndex = input.Span.IndexOf(' ');
        if (spaceIndex == -1)
        {
            token = input;
            skip += input.Length;
            return true;
        }
        
        token = input[..spaceIndex];
        skip += spaceIndex + 1;
        return true;
    }
}

file static class Helpers
{
    extension(ReadOnlySpan<char> span)
    {
        public int IndexOfUnescapedQuote()
        {
            var offset = 0;
            for (;;)
            {
                var index = span.IndexOf('"');
                if (index == -1) return -1;

                if (index != 0 && span[index - 1] == '\\')
                {
                    offset += index + 1;
                    span = span[(index + 1)..];
                }
                else
                {
                    return offset + index;
                }
            }
        }
    }
}
