using DAW.DDD.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

public class Location : IComparable<Location>
{
    public DateTimeOffset Start { get; private set; }
    public bool Active { get; private set; }
    protected Location(DateTimeOffset start, bool active)
    {
        ChangeStart(start);

        if (active)
            Activate();
        else
            Deactivate();
    }
    public Location ChangeStart(DateTimeOffset start)
    {
        Start = start;
        return this;
    }
    public Location Activate()
    {
        Active = true;
        return this;
    }
    public Location Deactivate()
    {
        Active = false;
        return this;
    }
    public static Location Create(DateTimeOffset start, bool active)
    {
        return new(start, active);
    }

    public int CompareTo(Location other) => Start.CompareTo(other.Start);

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

    public static bool operator ==(Location left, Location right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(Location left, Location right)
    {
        return !(left == right);
    }

    public static bool operator <(Location left, Location right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Location left, Location right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Location left, Location right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Location left, Location right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }
}
