using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;

namespace DAW.Repositories.States;

/// <summary>
/// Represents Clip in storage only POCO should exist here
/// </summary>
public class ClipState
{
    public ICollection<EventAtLocationState<SoundEventState>> Sounds { get; set; } = new List<EventAtLocationState<SoundEventState>>();

    /// <summary>
    /// Defines active length of clip, no events started after Length will be played
    /// </summary>
    public TimeSpan Length { get; set; }
    public Guid Id { get; set; }
}

public record EventAtLocationState<T>
{
    public LocationState? Location { get; set; }
    public T? Event { get; set; }

}

public record LocationState
{

    /// <summary>
    /// Defines when event occurred assuming T=0 is beginning of container (Clip/Track) 
    /// </summary>
    public TimeSpan Start { get; set; }

    /// <summary>
    /// Defines if given event should be collected for playback
    /// </summary>
    public bool Active { get; set; }
}

/// <summary>
/// SoundEvent represents single midi note/audio waveform
/// </summary>
public record SoundEventState
{

    /// <summary>
    /// Defines MIDI velocity (1-127)
    /// </summary>
    public int Velocity { get; set; }

    /// <summary>
    /// Defines MIDI pitch (1-127)
    /// For Audio this is base pitch
    /// </summary>
    public int Pitch { get; set; }

    /// <summary>
    /// Defines how long events is occurring
    /// </summary>
    public TimeSpan Length { get; set; }

    /// <summary>
    /// Defines artificial when to assume note was playing eg. crop of audio beginning
    /// </summary>
    public TimeSpan Offset { get; set; }
}

public class TrackState
{
    public ICollection<EventAtLocationState<Guid>> Clips { get; set; } = new List<EventAtLocationState<Guid>>();
    public Guid Id { get; set; }
    public Guid SourceId { get; set; } = Guid.Empty;

}
