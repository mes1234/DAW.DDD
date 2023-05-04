using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.Entities;

public class Track : IEntity, IPlayable
{
    private readonly ICollection<EventAtLocation<ClipEvent>> _clips = new List<EventAtLocation<ClipEvent>>();
    public IReadOnlyCollection<EventAtLocation<ClipEvent>> Clips => (IReadOnlyCollection<EventAtLocation<ClipEvent>>)_clips;

    public Guid Id { get; private set; }

    private readonly INotificationPublisher _publisher;
    protected Track(Guid id, ICollection<EventAtLocation<ClipEvent>> clips, INotificationPublisher publisher)
    {
        Id = id;
        _publisher = publisher ?? NullNotificationPublisher.Instance;
        this.AddTracks(clips);
    }
    public Track AddClip(EventAtLocation<ClipEvent> clip)
    {
        _clips.Add(clip);
        return this;
    }

    public static Track Create(ICollection<EventAtLocation<ClipEvent>> clips, INotificationPublisher publisher)
    {
        return new(Guid.NewGuid(), clips, publisher);
    }
    public static Track Create(Guid id, ICollection<EventAtLocation<ClipEvent>> clips, INotificationPublisher publisher)
    {
        return new(id, clips, publisher);
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents()
    {
        throw new NotImplementedException();
    }
}

public static class TrackExtensions
{
    public static Track AddTracks(this Track track, ICollection<EventAtLocation<ClipEvent>> clips)
    {
        foreach (var clip in clips)
        {
            track.AddClip(clip);
        }

        return track;
    }
}
