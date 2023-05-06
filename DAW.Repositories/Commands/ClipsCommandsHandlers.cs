using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;

namespace DAW.Repositories.Commands;

/// <summary>
/// Represents simple in memory storage of data
/// </summary>
internal class ClipsCommandsHandlers :
    IDomainNotificationHandler<ClipCreatedNotification>,
    IDomainNotificationHandler<AddSoundToClipNotification>,
    IDomainNotificationHandler<ChangeLengthOfClipNotification>,
    IDomainNotificationHandler<ChangeSourceIdOfClipNotification>
{
    private readonly IModelStateReader<ClipState> _stateReader;
    private readonly IModelStateWriter<ClipState> _stateWriter;

    public ClipsCommandsHandlers(IModelStateReader<ClipState> stateReader, IModelStateWriter<ClipState> stateWriter)
    {
        _stateReader = stateReader;
        _stateWriter = stateWriter;
    }

    public Task Handle(ClipCreatedNotification notification)
    {
        var newClip = new ClipState
        {
            Id = notification.EntityId,
            SourceId = notification.SourceId,
        };

        _stateWriter.TryAdd(notification.EntityId, newClip);

        return Task.CompletedTask;
    }

    public async Task Handle(AddSoundToClipNotification notification)
    {
        var clipState = await _stateReader.TryGet(notification.EntityId);

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
        var clipState = await _stateReader.TryGet(notification.EntityId);

        if (clipState == null) return;

        clipState.Length = notification.Length;
    }

    public async Task Handle(ChangeSourceIdOfClipNotification notification)
    {
        var clipState = await _stateReader.TryGet(notification.EntityId);

        if (clipState == null) return;

        clipState.SourceId = notification.SourceId;
    }
}
