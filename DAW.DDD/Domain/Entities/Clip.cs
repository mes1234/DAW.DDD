using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DAW.DDD.Domain.Entities;

/// <summary>
/// Clip is container for sound events which share the same SourceId (eg. midi channel audio file etc.)
/// </summary>
public class Clip : IEntity, IPlayable
{
    private readonly ICollection<EventAtLocation<SoundEvent>> _sounds = new List<EventAtLocation<SoundEvent>>();
    public IReadOnlyCollection<EventAtLocation<SoundEvent>> Sounds => (IReadOnlyCollection<EventAtLocation<SoundEvent>>)_sounds;
    public TimeSpan Length { get; private set; }
    public Guid SourceId { get; private set; }
    public Guid Id { get; private set; }

    private readonly INotificationPublisher _publisher;
    protected Clip(Guid id, ICollection<EventAtLocation<SoundEvent>> sounds, TimeSpan length, Guid sourceId, INotificationPublisher publisher)
    {
        Id = id;
        _publisher = publisher ?? NullNotificationPublisher.Instance;
        this.AddSounds(sounds);
        ChangeLength(length);
        ChangeSourceId(sourceId);
    }
    public Clip AddSound(EventAtLocation<SoundEvent> sound)
    {
        _sounds.Add(sound);
        return this;
    }

    public Clip ChangeLength(TimeSpan length)
    {
        Length = length;
        return this;
    }
    public Clip ChangeSourceId(Guid sourceId)
    {
        SourceId = sourceId;
        return this;
    }
    public static Clip Create(ICollection<EventAtLocation<SoundEvent>> sounds, TimeSpan length, Guid sourceId, INotificationPublisher publisher)
    {
        return new(Guid.NewGuid(), sounds, length, sourceId, publisher);
    }
    public static Clip Create(Guid id, ICollection<EventAtLocation<SoundEvent>> sounds, TimeSpan length, Guid sourceId, INotificationPublisher publisher)
    {
        return new(id, sounds, length, sourceId, publisher);
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset)
    {
        var result = new List<EventAtLocation<IReadOnlyCollection<SoundEvent>>>();

        var allEvents = _sounds
            .Where(x => x.Location.Start < Length)
            .SelectMany(x => x.GetPlayableEvents(offset));

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

public static class ClipEventExtensions
{
    public static Clip AddSounds(this Clip clipEvent, ICollection<EventAtLocation<SoundEvent>> sounds)
    {
        foreach (var sound in sounds)
        {
            clipEvent.AddSound(sound);
        }
        return clipEvent;
    }
}
