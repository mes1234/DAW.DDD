namespace DAW.Services.Dto;

/// <summary>
/// Represents Clip in storage only POCO should exist here
/// </summary>
public class ClipDto
{
    public ICollection<EventAtLocationDto<SoundEventDto>> Sounds { get; set; } = new List<EventAtLocationDto<SoundEventDto>>();

    /// <summary>
    /// Defines active length of clip, no events started after Length will be played
    /// <remarks>Expressed in milliseconds</remarks> 
    /// </summary>
    public int Length_ms { get; set; }
    public Guid Id { get; set; }
}

public record EventAtLocationDto<T>
{
    public LocationDto? Location { get; set; }
    public T? Event { get; set; }

}

public record LocationDto
{

    /// <summary>
    /// Defines when event occurred assuming T=0 is beginning of container (Clip/Track) 
    /// <remarks>Expressed in milliseconds</remarks> 
    /// </summary>
    public int Start_ms { get; set; }

    /// <summary>
    /// Defines if given event should be collected for playback
    /// </summary>
    public bool Active { get; set; }
}

/// <summary>
/// SoundEvent represents single midi note/audio waveform
/// </summary>
public record SoundEventDto
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
    /// <remarks>Expressed in milliseconds</remarks> 
    /// </summary>
    public int Length_ms { get; set; }

    /// <summary>
    /// Defines artificial when to assume note was playing eg. crop of audio beginning
    /// <remarks>Expressed in milliseconds</remarks> 
    /// </summary>
    public int Offset_ms { get; set; }
}

public class TrackDto
{
    public ICollection<EventAtLocationDto<ClipDto>> Clips { get; set; } = new List<EventAtLocationDto<ClipDto>>();
    public Guid Id { get; set; }
    public Guid SourceId { get; set; } = Guid.Empty;

    /// <summary>
    /// Defines how long events is occurring
    /// <remarks>Expressed in milliseconds</remarks> 
    /// </summary>
    public int Length_ms { get; set; }

}
