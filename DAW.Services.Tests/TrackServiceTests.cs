using AutoMapper;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.Repositories;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using DAW.Services.Dto;
using DAW.Services.Services;
using FluentAssertions;
using NSubstitute;
using System.Reflection.PortableExecutable;

namespace DAW.Services.Tests;

public class TrackServiceTests
{
    private readonly TrackService _trackService;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly IModelStateReader<ClipState> _clipStateProvider;
    private readonly IModelStateReader<TrackState> _trackStateProvider;

    public TrackServiceTests()
    {
        _notificationPublisher = Substitute.For<INotificationPublisher>();
        _clipStateProvider = Substitute.For<IModelStateReader<ClipState>>();
        _trackStateProvider = Substitute.For<IModelStateReader<TrackState>>();

        var mapper = (new MapperConfiguration(cfg => cfg.AddMaps(typeof(Mappings)))).CreateMapper();

        var repository = new ModelRepository(_clipStateProvider, _trackStateProvider, _notificationPublisher);

        _trackService = new TrackService(_notificationPublisher, mapper, repository);
    }

    [Fact]
    public void NewDefinedTrackShouldCreateEventsAndObjects_Test()
    {
        //Arrange
        var sourceId = Guid.NewGuid();

        //Act
        var result = _trackService.CreateTrack(sourceId, "dummy");

        //Assert
        _notificationPublisher.Received(1).Publish(Arg.Any<TrackCreatedNotification>());
        _notificationPublisher.Received(1).Publish(Arg.Any<ChangeTrackSourceIdNotification>());

        result.Id.Should().NotBeEmpty();
        result.SourceId.Should().Be(sourceId);
    }

    [Fact]
    public async Task NewDefinedClipShouldCreateEventsAndObjects_Test()
    {
        // Arrange 
        var trackLength = 1000;

        var dummyTrack = new TrackState
        {
            Clips = new List<EventAtLocationState<Guid>>(),
            Id = Guid.NewGuid(),
            SourceId = Guid.NewGuid(),
        };

        _trackStateProvider.TryGet(Arg.Any<Guid>()).Returns(o => Task.FromResult((TrackState?)dummyTrack));

        //Act

        var clip = await _trackService.CreateClip(Guid.NewGuid(), trackLength);

        //Assert
        _notificationPublisher.Received(1).Publish(Arg.Any<ClipAddedToTrackNotification>());
        _notificationPublisher.Received(1).Publish(Arg.Any<ChangeLengthOfClipNotification>());

        clip.Should().NotBeNull();
        clip!.Id.Should().NotBeEmpty();
        clip.Length_ms.Should().Be(trackLength);

    }
}