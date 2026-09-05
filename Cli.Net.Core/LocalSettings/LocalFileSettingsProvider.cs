using System.Collections.Concurrent;
using System.Text.Json;

namespace Cli.Net.Core.LocalSettings;

file record EntryData(string Key, string Json, string TypeName);

public sealed class LocalFileSettingsProvider(string filePath) : ILocalSettingsProvider
{
    private readonly ConcurrentDictionary<string, object> _settings = [];
    
    public ValueTask LoadAsync()
    {
        if(!File.Exists(filePath)) return ValueTask.CompletedTask;
        
        using var stream = File.OpenRead(filePath);
        
        var entries = JsonSerializer.Deserialize<EntryData[]>(stream);
        if (entries is null) return ValueTask.CompletedTask;

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.TypeName)) continue;
            
            var type = Type.GetType(entry.TypeName);
            if (type is null) continue;
            
            var value = JsonSerializer.Deserialize(entry.Json, type);
            if (value is not null)
            {
                _settings[entry.Key] = value;
            }
        }
        
        return ValueTask.CompletedTask;
    }

    public ValueTask SaveAsync()
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);
        
        var entries = _settings.Select(
            pair => new EntryData(
                Key: pair.Key,
                Json: JsonSerializer.Serialize(pair.Value),
                pair.Value.GetType().AssemblyQualifiedName ?? pair.Value.GetType().FullName!)
            ).ToArray();
        
        using var stream = File.OpenWrite(filePath);
        JsonSerializer.Serialize(stream, entries, new JsonSerializerOptions { WriteIndented = true });
        return ValueTask.CompletedTask;
    }

    public ValueTask ResetAsync()
    {
        _settings.Clear();
        return ValueTask.CompletedTask;
    }

    public T? Get<T>(string key) => _settings.TryGetValue(key, out var value) ? (T?)value : default;

    public void AddOrUpdate<T>(string key, T value) => _settings[key] = value ?? throw new ArgumentNullException(nameof(value));

    public void Remove(string key) => _settings.TryRemove(key, out _);
    
}
