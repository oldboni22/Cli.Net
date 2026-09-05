namespace Cli.Net.Core.Parse;

public ref struct ArgumentEnumerable(ReadOnlySpan<char> input, char flagStart = '-')
{
    private ReadOnlySpan<char> _input = input;

    private ReadOnlySpan<char> _currentFlag = ReadOnlySpan<char>.Empty;

    public ArgumentEnumerable GetEnumerator() => this;
    
    public ParsedArgument Current { get; private set; }

    public bool MoveNext()
    {
        for (;;)
        {
            var hasNext = _input.TryGetNextToken(out var token, out var skip, out var escaped);
            if (!hasNext)
            {
                if(_currentFlag.IsEmpty) return false;

                Current = new ParsedArgument
                {
                    Flag = _currentFlag,
                    Value = ReadOnlySpan<char>.Empty,
                };
                
                _currentFlag = ReadOnlySpan<char>.Empty;
                
                return true;
            }
            
            if (_currentFlag.IsEmpty)
            {
                if (token[0] == flagStart && !escaped) _currentFlag = token;
                _input = _input[skip..];
            }
            else
            {
                if (token[0] == flagStart && !escaped)
                {
                    Current = new ParsedArgument
                    {
                        Flag = _currentFlag,
                        Value = ReadOnlySpan<char>.Empty
                    };

                    _currentFlag = token;
                }
                else
                {
                    Current = new ParsedArgument
                    {
                        Flag = _currentFlag,
                        Value = token
                    };
                    
                    _currentFlag = ReadOnlySpan<char>.Empty;
                }
                
                _input = _input[skip..];
                return true;
            }
        }
    }
}

file static class Helpers
{
    extension(ReadOnlySpan<char> span)
    {
        public bool TryGetNextToken(
            out ReadOnlySpan<char> token,
            out int skip,
            out bool escaped)
        {
            var length = span.Length;
            span = span.TrimStart();

            skip = length - span.Length;
            token = ReadOnlySpan<char>.Empty;
            escaped = false;

            if (span.IsEmpty) return false;

            if (span[0] == '"')
            {
                span = span[1..];
                var closingIndex = span.IndexOfUnescapedQuote();

                if (closingIndex == -1) return false;

                token = span[..closingIndex];
                skip += closingIndex + 2;
                escaped = true;
                return true;
            }

            var spaceIndex = span.IndexOf(' ');
            if (spaceIndex == -1)
            {
                token = span;
                skip += span.Length;
                return true;
            }

            token = span[..spaceIndex];
            skip += spaceIndex + 1;
            return true;
        }
        
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
