using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.Commands;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using FluentAssertions;

namespace DAW.Repositories.Tests;

public class ClipsCommandsTests
{
    [Fact]
    public async Task CreatedCommandShouldResultInAddingToRepository_Test()
    {
        // Arrange
        var storage = new InMemoryStorage<ClipState>();

        var clip = Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromSeconds(0), NullNotificationPublisher.Instance);
        var command = clip.CreateClipCreatedNotification();

        var handler = new ClipsCommandsHandlers(storage, storage);
        // Act

        await handler.Handle(command);

        // Assert

        var clipRetrieved = await storage.TryGet(clip.Id);

        clipRetrieved.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatedCommandAndConfigurationShouldResultInAddingToRepository_Test()
    {
        // Arrange
        var storage = new InMemoryStorage<ClipState>();

        var clip = Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromSeconds(0), NullNotificationPublisher.Instance);
        var command1 = clip.CreateClipCreatedNotification();
        var command2 = clip.CreateAddSoundToClipNotification(EventAtLocation<SoundEvent>.Create(Location.Create(TimeSpan.FromSeconds(1), true), SoundEvent.Create(2, 2, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))));
        var command3 = clip.CreateChangeLengthOfClipNotification(TimeSpan.FromSeconds(5));


        var handler = new ClipsCommandsHandlers(storage, storage);
        // Act

        await handler.Handle(command1);
        await handler.Handle(command2);
        await handler.Handle(command3);

        // Assert

        var clipRetrieved = await storage.TryGet(clip.Id);

        clipRetrieved.Should().NotBeNull();
        clipRetrieved!.Length.Should().Be(command3.Length);
        clipRetrieved!.Sounds.Should().HaveCount(1);
        clipRetrieved!.Sounds.First().Event!.Velocity.Should().Be(command2.Sound.Velocity);
        clipRetrieved!.Sounds.First().Event!.Pitch.Should().Be(command2.Sound.Pitch);
        clipRetrieved!.Sounds.First().Event!.Length.Should().Be(command2.Sound.Length);
        clipRetrieved!.Sounds.First().Location!.Start.Should().Be(command2.Location.Start);
    }


}