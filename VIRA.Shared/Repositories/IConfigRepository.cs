using VIRA.Shared.Models.Entities;

namespace VIRA.Shared.Repositories;

/// <summary>
/// Repository interface for configuration persistence
/// </summary>
public interface IConfigRepository
{
    /// <summary>
    /// Save a configuration value
    /// </summary>
    Task<bool> SaveConfigAsync(string key, string value, string? category = null);

    /// <summary>
    /// Get a configuration value
    /// </summary>
    Task<string?> GetConfigAsync(string key);

    /// <summary>
    /// Get all configurations in a category
    /// </summary>
    Task<Dictionary<string, string>> GetConfigsByCategoryAsync(string category);

    /// <summary>
    /// Delete a configuration
    /// </summary>
    Task<bool> DeleteConfigAsync(string key);

    /// <summary>
    /// Clear all configurations
    /// </summary>
    Task<bool> ClearAllConfigsAsync();
}

/// <summary>
/// In-memory implementation of config repository
/// </summary>
public class InMemoryConfigRepository : IConfigRepository
{
    private readonly List<ConfigEntity> _configs = new();

    public Task<bool> SaveConfigAsync(string key, string value, string? category = null)
    {
        var existing = _configs.FirstOrDefault(c => c.Key == key);
        if (existing != null)
        {
            existing.Value = value;
            existing.UpdatedAt = DateTime.Now;
            if (category != null)
                existing.Category = category;
        }
        else
        {
            _configs.Add(new ConfigEntity
            {
                Key = key,
                Value = value,
                Category = category,
                UpdatedAt = DateTime.Now
            });
        }
        return Task.FromResult(true);
    }

    public Task<string?> GetConfigAsync(string key)
    {
        var config = _configs.FirstOrDefault(c => c.Key == key);
        return Task.FromResult(config?.Value);
    }

    public Task<Dictionary<string, string>> GetConfigsByCategoryAsync(string category)
    {
        var configs = _configs
            .Where(c => c.Category == category)
            .ToDictionary(c => c.Key, c => c.Value);
        return Task.FromResult(configs);
    }

    public Task<bool> DeleteConfigAsync(string key)
    {
        var config = _configs.FirstOrDefault(c => c.Key == key);
        if (config != null)
        {
            _configs.Remove(config);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> ClearAllConfigsAsync()
    {
        _configs.Clear();
        return Task.FromResult(true);
    }
}
