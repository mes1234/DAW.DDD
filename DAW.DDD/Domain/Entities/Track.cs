using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAW.DDD.Domain.Entities;

/// <summary>
/// Track represents container for Clips
/// </summary>
public class Track : IEntity, IPlayable
{
    private readonly ICollection<EventAtLocation<Clip>> _clips = new List<EventAtLocation<Clip>>();
    public IReadOnlyCollection<EventAtLocation<Clip>> Clips => (IReadOnlyCollection<EventAtLocation<Clip>>)_clips;

    public Guid Id { get; private set; }

    private readonly INotificationPublisher _publisher;
    protected Track(Guid id, ICollection<EventAtLocation<Clip>> clips, INotificationPublisher publisher)
    {
        Id = id;
        _publisher = publisher ?? NullNotificationPublisher.Instance;
        this.AddTracks(clips);
    }
    public Track AddClip(EventAtLocation<Clip> clip)
    {
        _clips.Add(clip);
        return this;
    }

    public static Track Create(ICollection<EventAtLocation<Clip>> clips, INotificationPublisher publisher)
    {
        return new(Guid.NewGuid(), clips, publisher);
    }
    public static Track Create(Guid id, ICollection<EventAtLocation<Clip>> clips, INotificationPublisher publisher)
    {
        return new(id, clips, publisher);
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset)
    {
        var result = new List<EventAtLocation<IReadOnlyCollection<SoundEvent>>>();

        var allEvents = _clips.SelectMany(x => x.GetPlayableEvents(offset));

        var groupedByLocation = allEvents.GroupBy(x => x.Location);

        foreach (var group in groupedByLocation)
        {
            var events = group.SelectMany(x => x.Event);
            var location = group.First().Location;

            result.Add(EventAtLocation<IReadOnlyCollection<SoundEvent>>.Create(location, events.ToList()));
        }

        return result;
    }
}

public static class TrackExtensions
{
    public static Track AddTracks(this Track track, ICollection<EventAtLocation<Clip>> clips)
    {
        foreach (var clip in clips)
        {
            track.AddClip(clip);
        }

        return track;
    }
}
