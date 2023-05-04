using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

public class SoundEvent : IPlayable
{
    public int Velocity { get; private set; }
    public int Pitch { get; private set; }
    public DateTimeOffset Length { get; private set; }
    public DateTimeOffset Offset { get; private set; }
    protected SoundEvent(int velocity, int pitch, DateTimeOffset length, DateTimeOffset offset)
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
    public SoundEvent ChangeLength(DateTimeOffset length)
    {
        Length = length;
        return this;
    }
    public SoundEvent ChangeOffset(DateTimeOffset offset)
    {
        Offset = offset;
        return this;
    }
    public static SoundEvent Create(int velocity, int pitch, DateTimeOffset length, DateTimeOffset offset)
    {
        return new(velocity, pitch, length, offset);
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents()
    {
        throw new NotImplementedException();
    }
}
