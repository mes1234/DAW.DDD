﻿using DAW.DDD.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

/// <summary>
/// Location represents occurrence wrt. some container Clip/Track etc.
/// </summary>
public record Location : IComparable<Location>
{
    public static readonly Location EmptyLocation = new Location(TimeSpan.FromSeconds(0), true);

    /// <summary>
    /// Defines when event occurred assuming T=0 is beginning of container (Clip/Track) 
    /// </summary>
    public TimeSpan Start { get; private set; }

    /// <summary>
    /// Defines if given event should be collected for playback
    /// </summary>
    public bool Active { get; private set; }
    protected Location(TimeSpan start, bool active)
    {
        ChangeStart(start);

        if (active)
            Activate();
        else
            Deactivate();
    }
    public Location ChangeStart(TimeSpan start)
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
    public static Location Create(TimeSpan start, bool active)
    {
        return new(start, active);
    }
    public static Location Create(TimeSpan start)
    {
        return new(start, true);
    }

    public int CompareTo(Location other) => Start.CompareTo(other.Start);

    public static Location operator +(Location lhs, Location rhs)
    {
        return Create(lhs.Start + rhs.Start, lhs.Active || rhs.Active);
    }

}
