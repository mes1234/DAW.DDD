using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.Commands;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using FluentAssertions;

namespace DAW.Repositories.Tests;

public class TracksCommandsTests
{
    [Fact]
    public async Task TrackCommandShouldResultInAddingToRepository_Test()
    {
        // Arrange
        var storage = new InMemoryStorage<TrackState>();

        var track = Track.Create(Enumerable.Empty<EventAtLocation<Clip>>().ToList(), NullNotificationPublisher.Instance);
        var command = track.CreateTrackCreatedNotification();

        var handler = new TracksCommandsHandlers(storage, storage);
        // Act

        await handler.Handle(command);

        // Assert

        var trackRetrieved = await storage.TryGet(track.Id);

        trackRetrieved.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatedCommandAndConfigurationShouldResultInAddingToRepository_Test()
    {
        // Arrange
        var storage = new InMemoryStorage<TrackState>();

        var track = Track.Create(Enumerable.Empty<EventAtLocation<Clip>>().ToList(), NullNotificationPublisher.Instance);
        var command1 = track.CreateTrackCreatedNotification();
        var command2 = track.CreateClipAddedToTrackNotification(EventAtLocation<Clip>.Create(Location.Create(TimeSpan.FromSeconds(1), true), Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromSeconds(1), Guid.NewGuid(), NullNotificationPublisher.Instance)));

        var handler = new TracksCommandsHandlers(storage, storage);
        // Act

        await handler.Handle(command1);
        await handler.Handle(command2);

        // Assert

        var trackRetrieved = await storage.TryGet(track.Id);

        trackRetrieved.Should().NotBeNull();
        trackRetrieved.Clips.Should().HaveCount(1);
    }


}