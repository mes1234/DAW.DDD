using AutoMapper;
using DAW.DDD.Domain.Notifications;
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
    [Fact]
    public void NewDefinedTrackServiceShouldBeRetrieved_Test()
    {
        //Arrange
        var notificationProvider = Substitute.For<INotificationPublisher>();
        var clipStateProvider = Substitute.For<IModelStateReader<ClipState>>();
        var trackStateProvider = Substitute.For<IModelStateReader<TrackState>>();

        var mapper = (new MapperConfiguration(cfg => cfg.AddMaps(typeof(Mappings)))).CreateMapper();

        var repository = new ModelRepository(clipStateProvider, trackStateProvider, notificationProvider);

        var sut = new TrackService(notificationProvider, mapper, repository);

        var sourceId = (Guid.NewGuid());
        //Act

        var result = sut.CreateTrack(sourceId, "dummy");

        //Assert

        notificationProvider.Received(1).Publish(Arg.Any<TrackCreatedNotification>());
        notificationProvider.Received(1).Publish(Arg.Any<ChangeTrackSourceIdNotification>());

        result.Id.Should().NotBeEmpty();
        result.SourceId.Should().Be(sourceId);
    }
}