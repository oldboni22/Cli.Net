using Cli.Net.Core.Parse;
using Microsoft.Extensions.ObjectPool;

namespace Cli.Net.Core.Pipeline;

public sealed class CliContext : IResettable
{
    internal CliContext(char flagStart = '-')
    {
        _flagStart = flagStart;
    }
    
    private readonly char _flagStart; 
    
    public string Input { get; private set; } = string.Empty;
    
    public ReadOnlyMemory<char> Command { get; private set; } = ReadOnlyMemory<char>.Empty;

    public string Arguments { get; private set; } = string.Empty;
    
    public string CommandString
    {
        get
        {
            if(string.IsNullOrEmpty(field)) field = Command.ToString();
            return field;  
        }
        
        private set;
    }
    
    public ArgumentEnumerable GetArgumentStream() => new(Arguments.AsSpan(), _flagStart);
    
    bool IResettable.TryReset()
    {
        Input = string.Empty;
        CommandString = string.Empty;
        Arguments = string.Empty;
        Command = ReadOnlyMemory<char>.Empty;
        
        return true;
    }
}
