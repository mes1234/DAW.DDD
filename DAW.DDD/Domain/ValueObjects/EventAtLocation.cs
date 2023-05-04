using DAW.DDD.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

public class EventAtLocation<T> : IComparable<T>
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

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }

    public static bool operator ==(EventAtLocation<T> left, EventAtLocation<T> right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(EventAtLocation<T> left, EventAtLocation<T> right)
    {
        return !(left == right);
    }

    public static bool operator <(EventAtLocation<T> left, EventAtLocation<T> right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(EventAtLocation<T> left, EventAtLocation<T> right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(EventAtLocation<T> left, EventAtLocation<T> right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(EventAtLocation<T> left, EventAtLocation<T> right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}
