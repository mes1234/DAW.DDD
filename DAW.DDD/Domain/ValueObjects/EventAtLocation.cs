using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

public record EventAtLocation<T> : IComparable<T>, IPlayable
{
    public Location Location { get; private set; }
    public T Event { get; private set; }
    protected EventAtLocation(Location location, T soundEvent)
    {
        ChangeEvent(soundEvent);
        ChangeLocation(location);
    }
    public EventAtLocation<T> ChangeLocation(Location location)
    {
        Location = location;
        return this;
    }
    public EventAtLocation<T> ChangeEvent(T soundEvent)
    {
        Event = soundEvent;
        return this;
    }
    public static EventAtLocation<T> Create(Location location, T soundEvent)
    {
        return new(location, soundEvent);
    }

    public int CompareTo(EventAtLocation<T> other)
    {
        return Location.CompareTo(other.Location);
    }

#nullable enable
    // Assume they ara the same, how to handle it?
    public int CompareTo(T? other) => 0;
#nullable disable


    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset)
    {
        if (Event is IPlayable playable)
        {
            return playable.GetPlayableEvents(Location + offset);
        }

        return new List<EventAtLocation<IReadOnlyCollection<SoundEvent>>>();
    }
}
