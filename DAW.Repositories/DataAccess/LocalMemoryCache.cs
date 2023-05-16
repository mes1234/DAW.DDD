using DAW.Repositories.States;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.DataAccess;
public class LocalMemoryCacheWriter<T> : IModelStateWriter<T> where T : class
{
    private static readonly TimeSpan _ttl = TimeSpan.FromSeconds(60);
    private readonly IMemoryCache _memoryCache;

    private readonly IModelStateWriter<T> _innerWriter;

    public LocalMemoryCacheWriter(IModelStateWriter<T> innerWriter, IMemoryCache memoryCache)
    {
        _innerWriter = innerWriter;
        _memoryCache = memoryCache;
    }

    public Task TryAddOrUpdate(Guid id, T value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_ttl);

        _memoryCache.Set(id, value, cacheEntryOptions);

        return _innerWriter.TryAddOrUpdate(id, value);
    }

}
public class LocalMemoryCacheReader<T> : IModelStateReader<T> where T : class
{
    private static readonly TimeSpan _ttl = TimeSpan.FromSeconds(60);

    private readonly IMemoryCache _memoryCache;

    private readonly IModelStateReader<T> _innerReader;

    public LocalMemoryCacheReader(IModelStateReader<T> innerReader, IMemoryCache memoryCache)
    {
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
            .SetAbsoluteExpiration(_ttl);

        _memoryCache.Set(id, found, cacheEntryOptions);

        return found;

    }
}
