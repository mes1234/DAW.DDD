using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;


namespace DAW.Repositories.DataAccess;

[ExcludeFromCodeCoverage]
public class LocalMemoryCacheWriter<T> : IModelStateWriter<T> where T : class
{
    private readonly IMemoryCache _memoryCache;

    private readonly IModelStateWriter<T> _innerWriter;
    private readonly CacheOptions _cacheOptions;

    public LocalMemoryCacheWriter(IModelStateWriter<T> innerWriter, IMemoryCache memoryCache, IOptions<CacheOptions> configureOptions)
    {
        _cacheOptions = configureOptions.Value;
        _innerWriter = innerWriter;
        _memoryCache = memoryCache;
    }

    public Task TryAddOrUpdate(Guid id, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheOptions.Ttl);

        _memoryCache.Set(id, value, cacheEntryOptions);

        return _innerWriter.TryAddOrUpdate(id, value);
    }

}

[ExcludeFromCodeCoverage]
public class LocalMemoryCacheReader<T> : IModelStateReader<T> where T : class
{
    private readonly IMemoryCache _memoryCache;

    private readonly IModelStateReader<T> _innerReader;

    private readonly CacheOptions _cacheOptions;

    public LocalMemoryCacheReader(IModelStateReader<T> innerReader, IMemoryCache memoryCache, IOptions<CacheOptions> configureOptions)
    {
        _cacheOptions = configureOptions.Value;
        _innerReader = innerReader;
        _memoryCache = memoryCache;
    }

    public async Task<T?> TryGet(Guid id)
    {
        if (_memoryCache.TryGetValue(id, out var value))
        {
            return (T?)value;
        }

        var found = await _innerReader.TryGet(id);

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_cacheOptions.Ttl);

        _memoryCache.Set(id, found, cacheEntryOptions);

        return found;

    }
}
