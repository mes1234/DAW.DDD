using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

public record SoundEvent : IPlayable
{
    public int Velocity { get; private set; }
    public int Pitch { get; private set; }
    public TimeSpan Length { get; private set; }
    public TimeSpan Offset { get; private set; }
    protected SoundEvent(int velocity, int pitch, TimeSpan length, TimeSpan offset)
    {
        ChangeVelocity(velocity);
        ChangePitch(pitch);
        ChangeLength(length);
        ChangeOffset(offset);
    }
    public SoundEvent ChangeVelocity(int velocity)
    {
        Velocity = velocity;
        return this;
    }
    public SoundEvent ChangePitch(int pitch)
    {
        Pitch = pitch;
        return this;
    }
    public SoundEvent ChangeLength(TimeSpan length)
    {
        Length = length;
        return this;
    }
    public SoundEvent ChangeOffset(TimeSpan offset)
    {
        Offset = offset;
        return this;
    }
    public static SoundEvent Create(int velocity, int pitch, TimeSpan length, TimeSpan offset)
    {
        return new(velocity, pitch, length, offset);
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset)
    {
        return new List<EventAtLocation<IReadOnlyCollection<SoundEvent>>>
        {
            EventAtLocation<IReadOnlyCollection<SoundEvent>>.Create(offset, new List<SoundEvent>
            {
                this
            })
        };
    }
}
