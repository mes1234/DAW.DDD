using DAW.Repositories.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.DataAccess;
internal class InMemoryStorage<T> : IModelStateReader<T>, IModelStateWriter<T> where T : class
{
    private const int DbDelay = 100;

    private readonly ConcurrentDictionary<Guid, T> _storage = new ConcurrentDictionary<Guid, T>();
    public async Task TryAdd(Guid id, T value)
    {
        await Task.Delay(DbDelay);

        _storage.TryAdd(id, value);
    }

    public async Task<T?> TryGet(Guid id)
    {
        await Task.Delay(DbDelay);

        if (_storage.TryGetValue(id, out var clip))
        {
            return clip;
        }

        // TODO is it ok?
        return null;
    }
}
