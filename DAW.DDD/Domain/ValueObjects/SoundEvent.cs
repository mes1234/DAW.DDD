using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.ValueObjects;

/// <summary>
/// SoundEvent represents single midi note/audio waveform
/// </summary>
public record SoundEvent : IPlayable
{

    /// <summary>
    /// Defines MIDI velocity (1-127)
    /// </summary>
    public int Velocity { get; private set; }

    /// <summary>
    /// Defines MIDI pitch (1-127)
    /// For Audio this is base pitch
    /// </summary>
    public int Pitch { get; private set; }

    /// <summary>
    /// Defines how long events is occurring
    /// </summary>
    public TimeSpan Length { get; private set; }

    /// <summary>
    /// Defines artificial when to assume note was playing eg. crop of audio beginning
    /// </summary>
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

    /// <summary>
    /// SoundEvent doesn't have its own Location only offset is applied
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
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
