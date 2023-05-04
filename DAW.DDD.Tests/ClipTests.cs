using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using FluentAssertions;

namespace DAW.DDD.Tests;

public class ClipTests
{
    [Fact]
    public void SerialClipsShouldBePresent_Test()
    {
        //Arrange & Act

        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(1));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var midiEvent2 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location2 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes2 = EventAtLocation<SoundEvent>.Create(location2, midiEvent2);

        var clip1 = Clip.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1, pairsNotes2 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        // Act
        var playableEvents = clip1.GetPlayableEvents(Location.EmptyLocation);
        //Assert

        playableEvents.Should().HaveCount(2);

        playableEvents.First().Event.First().Should().Be(midiEvent1);
        playableEvents.Last().Event.First().Should().Be(midiEvent2);
    }

    [Fact]
    public void ParallelClipsShouldBePresent_Test()
    {
        //Arrange & Act

        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var midiEvent2 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location2 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes2 = EventAtLocation<SoundEvent>.Create(location2, midiEvent2);

        var clip1 = Clip.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1, pairsNotes2 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        // Act
        var playableEvents = clip1.GetPlayableEvents(Location.EmptyLocation);
        //Assert

        playableEvents.Should().HaveCount(1);

        playableEvents.First().Event.First().Should().Be(midiEvent1);
        playableEvents.First().Event.Last().Should().Be(midiEvent2);
    }

    [Fact]
    public void OffsetShouldBePresent_Test()
    {
        //Arrange & Act

        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var clip1 = Clip.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        var clipOffset = Location.Create(TimeSpan.FromSeconds(1));

        // Act
        var playableEvents = clip1.GetPlayableEvents(clipOffset);
        //Assert

        playableEvents.Should().HaveCount(1);

        playableEvents.First().Event.First().Should().Be(midiEvent1);
        playableEvents.First().Location.Should().Be(location1 + clipOffset);
    }

    [Fact]
    public void EventsAfterEndShouldBeNotPresent_Test()
    {
        //Arrange & Act


        var midiEvent1 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location1 = Location.Create(TimeSpan.FromSeconds(2));

        var pairsNotes1 = EventAtLocation<SoundEvent>.Create(location1, midiEvent1);

        var midiEvent2 = SoundEvent.Create(1, 1, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0));
        var location2 = Location.Create(TimeSpan.FromSeconds(20));

        var pairsNotes2 = EventAtLocation<SoundEvent>.Create(location2, midiEvent2);

        var clip1 = Clip.Create(new List<EventAtLocation<SoundEvent>> { pairsNotes1, pairsNotes2 }, TimeSpan.FromSeconds(10), Guid.Empty, NullNotificationPublisher.Instance);

        var clipOffset = Location.Create(TimeSpan.FromSeconds(1));

        // Act
        var playableEvents = clip1.GetPlayableEvents(clipOffset);
        //Assert

        playableEvents.Should().HaveCount(1);

        playableEvents.First().Event.First().Should().Be(midiEvent1);
        playableEvents.First().Location.Should().Be(location1 + clipOffset);
    }
}