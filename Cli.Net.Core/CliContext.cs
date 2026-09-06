using Cli.Net.Core.Parse;

namespace Cli.Net.Core;

public ref struct CliContext()
{
    public char FlagStart { get; init; } 
    
    public ReadOnlySpan<char> Input { get; init; }

    public ReadOnlySpan<char> Command { get; init; } 

    public ReadOnlySpan<char> Arguments { get; init; } 
    
    public string CommandString
    {
        get
        {
            if(string.IsNullOrEmpty(field)) field = Command.ToString();
            return field;  
        }
    }

    public string ArgumentsString
    {
        get
        {
            if(string.IsNullOrEmpty(field)) field = Arguments.ToString();
            return field;
        }
    }

    public string InputString
    {
        get
        {
            if(string.IsNullOrEmpty(field)) field = Input.ToString();
            return field;
        }
    }
    
    public ArgumentEnumerable GetArgumentStream() => new(Arguments, FlagStart);
}
