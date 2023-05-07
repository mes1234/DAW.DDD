using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;

namespace DAW.Repositories.Commands;

/// <summary>
/// Represents simple in memory storage of data
/// </summary>
internal class TracksCommandsHandlers :
    IDomainNotificationHandler<TrackCreatedNotification>,
    IDomainNotificationHandler<ClipAddedToTrackNotification>
{
    private readonly IModelStateReader<TrackState> _stateReader;
    private readonly IModelStateWriter<TrackState> _stateWriter;

    public TracksCommandsHandlers(IModelStateReader<TrackState> stateReader, IModelStateWriter<TrackState> stateWriter)
    {
        _stateReader = stateReader;
        _stateWriter = stateWriter;
    }

    public async Task Handle(TrackCreatedNotification notification)
    {
        var newTrack = new TrackState
        {
            Id = notification.EntityId,
            SourceId = notification.SourceId,
        };

        await _stateWriter.TryAddOrUpdate(notification.EntityId, newTrack);
    }

    public async Task Handle(ClipAddedToTrackNotification notification)
    {
        var trackState = await _stateReader.TryGet(notification.EntityId);

        if (trackState == null) return;

        var newClip = new EventAtLocationState<Guid>
        {
            Location = new LocationState
            {
                Active = true,
                Start = notification.Start
            },
            Event = notification.ClipId
        };

        trackState.Clips.Add(newClip);

        await _stateWriter.TryAddOrUpdate(notification.EntityId, trackState);
    }
}
