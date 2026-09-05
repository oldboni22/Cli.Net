namespace Cli.Net.Core.Parse;

public readonly ref struct ParsedArgument
{
    public ReadOnlySpan<char> Flag { get; init; }
    
    public ReadOnlySpan<char> Value { get; init; }
    
    public bool HasValue => !Value.IsEmpty;
}
