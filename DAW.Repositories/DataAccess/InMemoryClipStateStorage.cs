using DAW.DDD.Domain.Notifications.Clips;
using DAW.Repositories.States;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.DataAccess;

/// <summary>
/// Represents simple in memory storage of data
/// </summary>
internal class InMemoryClipStateStorage :
    IModelStateProvider<ClipState>,
    IDomainNotificationHandler<ClipCreatedNotification>,
    IDomainNotificationHandler<AddSoundToClipNotification>,
    IDomainNotificationHandler<ChangeLengthOfClipNotification>,
    IDomainNotificationHandler<ChangeSourceIdOfClipNotification>
{
    private readonly ConcurrentDictionary<Guid, ClipState> _storage = new ConcurrentDictionary<Guid, ClipState>();

    public Task Handle(ClipCreatedNotification notification)
    {
        var newClip = new ClipState
        {
            Id = notification.EntityId,
            SourceId = notification.SourceId,
        };

        _storage.TryAdd(notification.EntityId, newClip);

        return Task.CompletedTask;
    }

    public async Task Handle(AddSoundToClipNotification notification)
    {
        var clipState = await TryGet(notification.EntityId);

        if (clipState == null) return;

        var newSound = new EventAtLocationState<SoundEventState>
        {
            Location = new LocationState
            {
                Active = true,
                Start = notification.Location.Start
            },
            Event = new SoundEventState
            {
                Length = notification.Sound.Length,
                Offset = notification.Sound.Offset,
                Pitch = notification.Sound.Pitch,
                Velocity = notification.Sound.Velocity,
            }
        };

        clipState.Sounds.Add(newSound);
    }

    public async Task Handle(ChangeLengthOfClipNotification notification)
    {
        var clipState = await TryGet(notification.EntityId);

        if (clipState == null) return;

        clipState.Length = notification.Length;
    }

    public async Task Handle(ChangeSourceIdOfClipNotification notification)
    {
        var clipState = await TryGet(notification.EntityId);

        if (clipState == null) return;

        clipState.SourceId = notification.SourceId;
    }

    public Task<ClipState?> TryGet(Guid id)
    {
        if (_storage.TryGetValue(id, out var clip))
        {
            return Task.FromResult((ClipState?)clip);
        }

        return Task.FromResult((ClipState?)null);
    }
}
