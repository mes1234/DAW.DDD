namespace DAW.Repositories.DataAccess;
public interface IModelStateProvider<T>
{
    public Task<T?> TryGet(Guid id);
}
