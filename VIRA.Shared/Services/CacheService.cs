namespace VIRA.Shared.Services;

/// <summary>
/// Cache entry with expiration
/// </summary>
public class CacheEntry<T>
{
    public T Value { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired => DateTime.Now > ExpiresAt;

    public CacheEntry(T value, TimeSpan ttl)
    {
        Value = value;
        ExpiresAt = DateTime.Now.Add(ttl);
    }
}

/// <summary>
/// Service for caching data with TTL (Time To Live)
/// </summary>
public class CacheService
{
    private readonly Dictionary<string, object> _cache = new();
    private readonly object _lock = new();

    // Default TTL values
    public static readonly TimeSpan WeatherCacheTTL = TimeSpan.FromHours(1);
    public static readonly TimeSpan NewsCacheTTL = TimeSpan.FromMinutes(30);
    public static readonly TimeSpan MessageCacheTTL = TimeSpan.FromMinutes(60);
    public static readonly TimeSpan ContactCacheTTL = TimeSpan.FromHours(24);

    /// <summary>
    /// Get cached value
    /// </summary>
    public T? Get<T>(string key)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var obj) && obj is CacheEntry<T> entry)
            {
                if (!entry.IsExpired)
                {
                    return entry.Value;
                }
                
                // Remove expired entry
                _cache.Remove(key);
            }
            
            return default;
        }
    }

    /// <summary>
    /// Set cached value with TTL
    /// </summary>
    public void Set<T>(string key, T value, TimeSpan ttl)
    {
        lock (_lock)
        {
            _cache[key] = new CacheEntry<T>(value, ttl);
        }
    }

    /// <summary>
    /// Check if key exists and is not expired
    /// </summary>
    public bool Contains(string key)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var obj))
            {
                var entryType = obj.GetType();
                var isExpiredProperty = entryType.GetProperty("IsExpired");
                if (isExpiredProperty != null)
                {
                    var isExpired = (bool)isExpiredProperty.GetValue(obj)!;
                    if (!isExpired)
                    {
                        return true;
                    }
                    
                    // Remove expired entry
                    _cache.Remove(key);
                }
            }
            
            return false;
        }
    }

    /// <summary>
    /// Remove cached value
    /// </summary>
    public void Remove(string key)
    {
        lock (_lock)
        {
            _cache.Remove(key);
        }
    }

    /// <summary>
    /// Clear all cached values
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Clear expired entries
    /// </summary>
    public void ClearExpired()
    {
        lock (_lock)
        {
            var expiredKeys = new List<string>();
            
            foreach (var kvp in _cache)
            {
                var entryType = kvp.Value.GetType();
                var isExpiredProperty = entryType.GetProperty("IsExpired");
                if (isExpiredProperty != null)
                {
                    var isExpired = (bool)isExpiredProperty.GetValue(kvp.Value)!;
                    if (isExpired)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }
            }
            
            foreach (var key in expiredKeys)
            {
                _cache.Remove(key);
            }
        }
    }

    /// <summary>
    /// Get or create cached value
    /// </summary>
    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan ttl)
    {
        var cached = Get<T>(key);
        if (cached != null)
        {
            return cached;
        }

        var value = await factory();
        Set(key, value, ttl);
        return value;
    }

    /// <summary>
    /// Get cache statistics
    /// </summary>
    public CacheStats GetStats()
    {
        lock (_lock)
        {
            var total = _cache.Count;
            var expired = 0;
            
            foreach (var kvp in _cache)
            {
                var entryType = kvp.Value.GetType();
                var isExpiredProperty = entryType.GetProperty("IsExpired");
                if (isExpiredProperty != null)
                {
                    var isExpired = (bool)isExpiredProperty.GetValue(kvp.Value)!;
                    if (isExpired)
                    {
                        expired++;
                    }
                }
            }
            
            return new CacheStats
            {
                TotalEntries = total,
                ExpiredEntries = expired,
                ActiveEntries = total - expired
            };
        }
    }
}

/// <summary>
/// Cache statistics
/// </summary>
public class CacheStats
{
    public int TotalEntries { get; set; }
    public int ExpiredEntries { get; set; }
    public int ActiveEntries { get; set; }
}

/// <summary>
/// Extension methods for common caching patterns
/// </summary>
public static class CacheExtensions
{
    /// <summary>
    /// Cache weather data
    /// </summary>
    public static void CacheWeather(this CacheService cache, string location, object weatherData)
    {
        cache.Set($"weather_{location}", weatherData, CacheService.WeatherCacheTTL);
    }

    /// <summary>
    /// Get cached weather data
    /// </summary>
    public static T? GetCachedWeather<T>(this CacheService cache, string location)
    {
        return cache.Get<T>($"weather_{location}");
    }

    /// <summary>
    /// Cache news articles
    /// </summary>
    public static void CacheNews(this CacheService cache, string category, object newsData)
    {
        cache.Set($"news_{category}", newsData, CacheService.NewsCacheTTL);
    }

    /// <summary>
    /// Get cached news articles
    /// </summary>
    public static T? GetCachedNews<T>(this CacheService cache, string category)
    {
        return cache.Get<T>($"news_{category}");
    }

    /// <summary>
    /// Cache contact list
    /// </summary>
    public static void CacheContacts(this CacheService cache, object contacts)
    {
        cache.Set("contacts_all", contacts, CacheService.ContactCacheTTL);
    }

    /// <summary>
    /// Get cached contact list
    /// </summary>
    public static T? GetCachedContacts<T>(this CacheService cache)
    {
        return cache.Get<T>("contacts_all");
    }
}
