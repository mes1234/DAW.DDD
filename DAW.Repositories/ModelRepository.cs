using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using System.Collections.Concurrent;

namespace DAW.Repositories;
public class ModelRepository
{
    private readonly IModelStateProvider<ClipState> _clipStateProvider;
    private readonly INotificationPublisher _notificationPublisher;

    public ModelRepository(IModelStateProvider<ClipState> clipStateProvider, INotificationPublisher notificationPublisher)
    {
        _clipStateProvider = clipStateProvider;
        _notificationPublisher = notificationPublisher;
    }

    // TODO TEST IT!!!
    public async Task<Clip?> Get(Guid id)
    {
        var clipState = await _clipStateProvider.TryGet(id);

        if (clipState == null) return null;

        var clipModel = clipState.ToModel();

        clipModel.Activate(_notificationPublisher);

        return clipModel;

    }
}
