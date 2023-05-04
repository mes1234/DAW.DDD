using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DAW.DDD.Domain.Entities;

public class ClipEvent : IEntity, IPlayable
{
    private readonly ICollection<EventAtLocation<SoundEvent>> _sounds = new List<EventAtLocation<SoundEvent>>();
    public IReadOnlyCollection<EventAtLocation<SoundEvent>> Sounds => (IReadOnlyCollection<EventAtLocation<SoundEvent>>)_sounds;
    public Location Location { get; private set; }
    public DateTimeOffset Length { get; private set; }
    public Guid SourceId { get; private set; }
    public Guid Id { get; private set; }

    private readonly INotificationPublisher _publisher;
    protected ClipEvent(Guid id, ICollection<EventAtLocation<SoundEvent>> sounds, Location location, DateTimeOffset length, Guid sourceId, INotificationPublisher publisher)
    {
        Id = id;
        _publisher = publisher ?? NullNotificationPublisher.Instance;
        this.AddSounds(sounds);
        ChangeLocation(location);
        ChangeLength(length);
        ChangeSourceId(sourceId);
    }
    public ClipEvent AddSound(EventAtLocation<SoundEvent> sound)
    {
        _sounds.Add(sound);
        return this;
    }
    public ClipEvent ChangeLocation(Location location)
    {
        Location = location;
        return this;
    }
    public ClipEvent ChangeLength(DateTimeOffset length)
    {
        Length = length;
        return this;
    }
    public ClipEvent ChangeSourceId(Guid sourceId)
    {
        SourceId = sourceId;
        return this;
    }
    public static ClipEvent Create(ICollection<EventAtLocation<SoundEvent>> sounds, Location location, DateTimeOffset length, Guid sourceId, INotificationPublisher publisher)
    {
        return new(Guid.NewGuid(), sounds, location, length, sourceId, publisher);
    }
    public static ClipEvent Create(Guid id, ICollection<EventAtLocation<SoundEvent>> sounds, Location location, DateTimeOffset length, Guid sourceId, INotificationPublisher publisher)
    {
        return new(id, sounds, location, length, sourceId, publisher);
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents()
    {
        throw new NotImplementedException();
    }
}

public static class ClipEventExtensions
{
    public static ClipEvent AddSounds(this ClipEvent clipEvent, ICollection<EventAtLocation<SoundEvent>> sounds)
    {
        foreach (var sound in sounds)
        {
            clipEvent.AddSound(sound);
        }
        return clipEvent;
    }
}
