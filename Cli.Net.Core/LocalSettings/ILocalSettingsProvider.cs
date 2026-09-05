namespace Cli.Net.Core.LocalSettings;

public interface ILocalSettingsProvider
{
    ValueTask LoadAsync();
    
    ValueTask SaveAsync();

    ValueTask ResetAsync();
    
    T? Get<T>(string key);
    
    void AddOrUpdate<T>(string key, T value);
    
    void Remove(string key);
}
