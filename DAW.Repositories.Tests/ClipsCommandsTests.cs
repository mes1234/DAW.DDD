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

        var clip = Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromSeconds(0), Guid.NewGuid(), NullNotificationPublisher.Instance);
        var command = clip.CreateClipCreatedNotification();

        var handler = new ClipsCommandsHandlers(storage, storage);
        // Act

        await handler.Handle(command);

        // Assert

        var clipRetrieved = await storage.TryGet(clip.Id);

        clipRetrieved.Should().NotBeNull();
    }
}