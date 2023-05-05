using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using System.Collections.Concurrent;

namespace DAW.Repositories;
public class ClipsRepository
{
    private readonly IModelStateProvider<ClipState> _clipStateProvider;
    private readonly INotificationPublisher _notificationPublisher;

    public ClipsRepository(IModelStateProvider<ClipState> clipStateProvider, INotificationPublisher notificationPublisher)
    {
        _clipStateProvider = clipStateProvider;
        _notificationPublisher = notificationPublisher;
    }

    // TODO Clip should have some Null Object?
    // TODO this might be mapper?
    // TODO TEST IT!!!
    public async Task<Clip?> Get(Guid id)
    {
        var clipState = await _clipStateProvider.TryGet(id);

        if (clipState == null) return null;

        var soundsModels = new List<EventAtLocation<SoundEvent>>();
        foreach (var sound in clipState.Sounds)
        {
            var locationModel = Location.Create(sound!.Location!.Start);
            var soundEvent = SoundEvent.Create(sound!.Event!.Velocity, sound.Event.Pitch, sound.Event.Length, sound.Event.Offset);
            soundsModels.Add(EventAtLocation<SoundEvent>.Create(locationModel, soundEvent));
        }

        var clipModel = Clip.Create(soundsModels.ToArray(), clipState.Length, clipState.SourceId, NullNotificationPublisher.Instance);

        clipModel.Activate(_notificationPublisher);

        return clipModel;

    }
}
