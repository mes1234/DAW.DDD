using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using System.Collections;
using System.Collections.Concurrent;

namespace DAW.Repositories;
public class ModelRepository
{
    private readonly IModelStateReader<ClipState> _clipStateProvider;
    private readonly IModelStateReader<TrackState> _trackStateProvider;
    private readonly INotificationPublisher _notificationPublisher;

    public ModelRepository(
        IModelStateReader<ClipState> clipStateProvider,
        IModelStateReader<TrackState> trackStateProvider,
        INotificationPublisher notificationPublisher)
    {
        _clipStateProvider = clipStateProvider;
        _trackStateProvider = trackStateProvider;
        _notificationPublisher = notificationPublisher;
    }

    // TODO TEST IT!!!
    public async Task<Clip?> GetClip(Guid id)
    {
        var state = await _clipStateProvider.TryGet(id);

        if (state == null) return null;

        var model = state.ToModel();

        model.Activate(_notificationPublisher);

        return model;
    }

    // TODO TEST IT!!!
    public async Task<Track?> GetTrack(Guid id)
    {
        // Get state of Track
        var state = await _trackStateProvider.TryGet(id);

        if (state == null) return null;

        var clips = await GetClips(state.Clips.Select(x => x.Event));

        // Assemble model
        var model = state.ToModel(clips);

        model.Activate(_notificationPublisher);

        return model;
    }

    private async Task<IEnumerable<Clip>> GetClips(IEnumerable<Guid> ids)
    {
        var clips = new List<Clip>();

        foreach (var id in ids)
        {
            // Get clip
            var clip = await GetClip(id);

            if (clip == null) continue;

            clips.Add(clip);
        }

        return clips;
    }
}
