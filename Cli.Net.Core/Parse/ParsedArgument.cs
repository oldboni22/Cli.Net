namespace Cli.Net.Core.Parse;

public readonly ref struct ParsedArgument
{
    public ReadOnlySpan<char> Flag { get; init; }
    
    public ReadOnlySpan<char> Value { get; init; }
    
    public bool HasValue => !Value.IsEmpty;

    public void Deconstruct(out ReadOnlySpan<char> flag, out ReadOnlySpan<char> value)
    {
        flag = Flag;
        value = Value;
    }

    public void Deconstruct(out string flag, out string? value)
    {
        flag = Flag.ToString();
        value = Value.IsEmpty ? null : Value.ToString();
    }
}
