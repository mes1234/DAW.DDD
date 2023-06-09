﻿using DAW.Repositories.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.DataAccess;
public class InMemoryStorage<T> : IModelStateReader<T>, IModelStateWriter<T> where T : class
{
    private const int DbDelay = 100;

    private static readonly ConcurrentDictionary<Guid, T> _storage = new ConcurrentDictionary<Guid, T>();
    public async Task TryAddOrUpdate(Guid id, T value)
    {
        await Task.Delay(DbDelay);

        _storage.AddOrUpdate(id, value, (key, oldValue) => value);
    }

    public async Task<T?> TryGet(Guid id)
    {
        // Simulate some delay
        await Task.Delay(DbDelay);

        if (_storage.TryGetValue(id, out var clip))
        {
            return clip;
        }

        // TODO is it ok?
        return null;
    }
}
