namespace DAW.Repositories.DataAccess;

public interface IModelStateReader<T>
{
    public Task<T?> TryGet(Guid id);
}

public interface IModelStateWriter<in T>
{
    public Task TryAddOrUpdate(Guid id, T value);
}

