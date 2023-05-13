using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DAW.DDD.Domain.Entities;

/// <summary>
/// Clip is container for sound events which share the same SourceId (eg. midi channel audio file etc.)
/// </summary>
public class Clip : IEntity, IPlayable
{
    private readonly ICollection<EventAtLocation<SoundEvent>> _sounds = new List<EventAtLocation<SoundEvent>>();
    public IReadOnlyCollection<EventAtLocation<SoundEvent>> Sounds => (IReadOnlyCollection<EventAtLocation<SoundEvent>>)_sounds;

    /// <summary>
    /// Defines active length of clip, no events started after Length will be played
    /// </summary>
    public TimeSpan Length { get; private set; }

    public Guid Id { get; private set; }

    private INotificationPublisher _publisher;
    protected Clip(Guid id, INotificationPublisher publisher)
    {
        Id = id;
        _publisher = publisher ?? NullNotificationPublisher.Instance;

        _publisher.Publish(this.CreateClipCreatedNotification());


    }
    public Clip AddSound(EventAtLocation<SoundEvent> sound)
    {
        _sounds.Add(sound);
        _publisher.Publish(this.CreateAddSoundToClipNotification(sound));
        return this;
    }
    public Clip ChangeLength(TimeSpan length)
    {
        Length = length;
        _publisher.Publish(this.CreateChangeLengthOfClipNotification(length));
        return this;
    }

    public static Clip Create(ICollection<EventAtLocation<SoundEvent>> sounds, TimeSpan length, INotificationPublisher publisher) => Create(Guid.NewGuid(), sounds, length, publisher);

    public static Clip Create(Guid id, ICollection<EventAtLocation<SoundEvent>> sounds, TimeSpan length, INotificationPublisher publisher)
    {
        var newClip = new Clip(id, publisher);

        newClip.AddSounds(sounds);
        newClip.ChangeLength(length);

        return newClip;
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset)
    {
        var result = new List<EventAtLocation<IReadOnlyCollection<SoundEvent>>>();

        var allEvents = _sounds
            .Where(x => x.Location.Start < Length)
            .SelectMany(x => x.GetPlayableEvents(offset));

        var groupedByLocation = allEvents.OrderBy(x => x.Location).GroupBy(x => x.Location);

        foreach (var group in groupedByLocation)
        {
            var events = group.SelectMany(x => x.Event);
            var location = group.First().Location;

            result.Add(EventAtLocation<IReadOnlyCollection<SoundEvent>>.Create(location, events.ToList()));
        }

        return result;
    }

    public void Activate(INotificationPublisher notificationPublisher)
    {
        _publisher = notificationPublisher;
    }
}

public static class ClipEventExtensions
{
    public static Clip AddSounds(this Clip clipEvent, ICollection<EventAtLocation<SoundEvent>> sounds)
    {
        foreach (var sound in sounds)
        {
            clipEvent.AddSound(sound);
        }
        return clipEvent;
    }
    public static ClipCreatedNotification CreateClipCreatedNotification(this Clip clip) => new(clip.Id, clip.Id.ToString(), clip);
    public static AddSoundToClipNotification CreateAddSoundToClipNotification(this Clip clip, EventAtLocation<SoundEvent> sound) => new(clip.Id, clip.Id.ToString(), sound);
    public static ChangeLengthOfClipNotification CreateChangeLengthOfClipNotification(this Clip clip, TimeSpan length) => new(clip.Id, clip.Id.ToString(), length);
}
