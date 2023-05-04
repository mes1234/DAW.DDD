using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using FluentAssertions;

namespace DAW.DDD.Tests;

public class TrackTests
{
    [Fact]
    public void SerialClipsShouldBePresent_Test()
    {
        //Arrange & Act

        // Clip 1
        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var midiEvent2 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location2 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes2 = EventAtLocation<SoundEvent>.Create(location2, midiEvent2);

        var clip1 = ClipEvent.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1, pairsNotes2 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        // Clip 2
        var midiEvent3 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location3 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes3 = EventAtLocation<SoundEvent>.Create(location3, midiEvent3);

        var midiEvent4 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location4 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes4 = EventAtLocation<SoundEvent>.Create(location4, midiEvent4);

        var clip2 = ClipEvent.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes3, pairsNotes4 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        // Track1

        var clip1Location = Location.Create(TimeSpan.FromSeconds(1));
        var pairsClip1 = EventAtLocation<ClipEvent>.Create(clip1Location, clip1);
        var clip2Location = Location.Create(TimeSpan.FromSeconds(10));
        var pairsClip2 = EventAtLocation<ClipEvent>.Create(clip2Location, clip2);
        var track1 = Track.Create(new List<EventAtLocation<ClipEvent>> { pairsClip1, pairsClip2 }, NullNotificationPublisher.Instance);


        // Act
        var playableEvents = track1.GetPlayableEvents(Location.EmptyLocation);
        //Assert

        playableEvents.Should().HaveCount(4);

        playableEvents.ToList()[0].Event.First().Should().Be(midiEvent1);
        playableEvents.ToList()[1].Event.First().Should().Be(midiEvent2);
        playableEvents.ToList()[2].Event.First().Should().Be(midiEvent3);
        playableEvents.ToList()[3].Event.First().Should().Be(midiEvent4);

        playableEvents.ToList()[0].Location.Should().Be(location1 + clip1Location);
        playableEvents.ToList()[1].Location.Should().Be(location2 + clip1Location);
        playableEvents.ToList()[2].Location.Should().Be(location3 + clip2Location);
        playableEvents.ToList()[3].Location.Should().Be(location4 + clip2Location);
    }

    [Fact]
    public void ParallelClipsShouldBePresent_Test()
    {
        //Arrange & Act

        // Clip 1
        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var midiEvent2 = SoundEvent.Create(2, 2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location2 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes2 = EventAtLocation<SoundEvent>.Create(location2, midiEvent2);

        var clip1 = ClipEvent.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1, pairsNotes2 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        // Clip 2
        var midiEvent3 = SoundEvent.Create(3, 3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location3 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes3 = EventAtLocation<SoundEvent>.Create(location3, midiEvent3);

        var midiEvent4 = SoundEvent.Create(4, 4, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location4 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes4 = EventAtLocation<SoundEvent>.Create(location4, midiEvent4);

        var clip2 = ClipEvent.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes3, pairsNotes4 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        // Track1

        var clip1Location = Location.Create(TimeSpan.FromSeconds(1));
        var pairsClip1 = EventAtLocation<ClipEvent>.Create(clip1Location, clip1);
        var clip2Location = Location.Create(TimeSpan.FromSeconds(1));
        var pairsClip2 = EventAtLocation<ClipEvent>.Create(clip2Location, clip2);
        var track1 = Track.Create(new List<EventAtLocation<ClipEvent>> { pairsClip1, pairsClip2 }, NullNotificationPublisher.Instance);


        // Act
        var playableEvents = track1.GetPlayableEvents(Location.EmptyLocation);
        //Assert

        playableEvents.Should().HaveCount(2);

        playableEvents.ToList()[0].Event.First().Should().Be(midiEvent1);
        playableEvents.ToList()[0].Event.Last().Should().Be(midiEvent3);
        playableEvents.ToList()[1].Event.First().Should().Be(midiEvent2);
        playableEvents.ToList()[1].Event.Last().Should().Be(midiEvent4);

        playableEvents.ToList()[0].Location.Should().Be(location1 + clip1Location);
        playableEvents.ToList()[1].Location.Should().Be(location2 + clip1Location);
    }
}