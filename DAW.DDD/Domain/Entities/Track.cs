﻿using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.DDD.Domain.Primitives;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace DAW.DDD.Domain.Entities;

/// <summary>
/// Track represents container for Clips
/// </summary>
public class Track : IEntity, IPlayable
{
    private readonly ICollection<EventAtLocation<Clip>> _clips = new List<EventAtLocation<Clip>>();
    public IReadOnlyCollection<EventAtLocation<Clip>> Clips => (IReadOnlyCollection<EventAtLocation<Clip>>)_clips;

    public Guid Id { get; private set; }

    public Guid SourceId { get; private set; } = Guid.Empty;

    //TODO TEST IT!
    public TimeSpan Length => TimeSpan.FromMilliseconds(_clips.Sum(x => x.Event.Length.TotalMilliseconds));

    private INotificationPublisher _publisher;
    protected Track(Guid id, INotificationPublisher publisher)
    {
        Id = id;
        _publisher = publisher ?? NullNotificationPublisher.Instance;

        _publisher.Publish(this.CreateTrackCreatedNotification());
    }
    public Track AddClip(EventAtLocation<Clip> clip)
    {
        _clips.Add(clip);

        _publisher.Publish(this.CreateClipAddedToTrackNotification(clip));
        return this;
    }
    public Track ChangeSourceId(Guid sourceId)
    {
        SourceId = sourceId;

        _publisher.Publish(this.CreateChangeTrackSourceIdNotification(sourceId));
        return this;
    }

    public static Track Create(ICollection<EventAtLocation<Clip>> clips, Guid sourceId, INotificationPublisher publisher) => Create(Guid.NewGuid(), clips, sourceId, publisher);

    public static Track Create(Guid id, ICollection<EventAtLocation<Clip>> clips, Guid sourceId, INotificationPublisher publisher)
    {
        var newTrack = new Track(id, publisher);

        newTrack.AddClips(clips);
        newTrack.ChangeSourceId(sourceId);

        return newTrack;
    }

    public IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> GetPlayableEvents(Location offset)
    {
        var result = new List<EventAtLocation<IReadOnlyCollection<SoundEvent>>>();

        var allEvents = _clips.SelectMany(x => x.GetPlayableEvents(offset));

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

public static class TrackExtensions
{
    public static Track AddClips(this Track track, ICollection<EventAtLocation<Clip>> clips)
    {
        foreach (var clip in clips)
        {
            track.AddClip(clip);
        }

        return track;
    }
    public static TrackCreatedNotification CreateTrackCreatedNotification(this Track track) => new(track.Id, track.Id.ToString(), track);
    public static ClipAddedToTrackNotification CreateClipAddedToTrackNotification(this Track track, EventAtLocation<Clip> clip) => new(track.Id, track.Id.ToString(), clip);
    public static ChangeTrackSourceIdNotification CreateChangeTrackSourceIdNotification(this Track track, Guid sourceId) => new(track.Id, track.Id.ToString(), sourceId);
}
