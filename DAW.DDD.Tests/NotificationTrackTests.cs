using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using FluentAssertions;

namespace DAW.DDD.Tests;

public class NotificationTrackTests
{
    [Fact]
    public void AllNotificationShouldBePresent_Test()
    {
        //Arrange & Act

        var notificationPublisher = new NotificationPublisher();

        // Clip 1
        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var midiEvent2 = SoundEvent.Create(2, 2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location2 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes2 = EventAtLocation<SoundEvent>.Create(location2, midiEvent2);

        var clip1 = Clip.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1, pairsNotes2 }, TimeSpan.FromSeconds(10), Guid.Empty, notificationPublisher);

        // Clip 2
        var midiEvent3 = SoundEvent.Create(3, 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location3 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes3 = EventAtLocation<SoundEvent>.Create(location3, midiEvent3);

        var midiEvent4 = SoundEvent.Create(4, 4, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location4 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes4 = EventAtLocation<SoundEvent>.Create(location4, midiEvent4);

        var clip2 = Clip.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes3, pairsNotes4 }, TimeSpan.FromSeconds(10), Guid.Empty, notificationPublisher);

        // Track1

        var clip1Location = Location.Create(TimeSpan.FromSeconds(1));
        var pairsClip1 = EventAtLocation<Clip>.Create(clip1Location, clip1);
        var clip2Location = Location.Create(TimeSpan.FromSeconds(10));
        var pairsClip2 = EventAtLocation<Clip>.Create(clip2Location, clip2);
        _ = Track.Create(new List<EventAtLocation<Clip>> { pairsClip1, pairsClip2 }, Guid.NewGuid(), notificationPublisher);


        // Act
        notificationPublisher.NotificationCount.Should().Be(14);
        //Assert

    }
}