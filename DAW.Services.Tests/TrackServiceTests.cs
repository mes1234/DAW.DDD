using AutoMapper;
using DAW.DDD.Domain.Entities;
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

    [Fact]
    public async Task NewDefinedSoundShouldCreateEventsAndObjects_Test()
    {
        // Arrange 
        var trackLength = 1000;
        var clipId = Guid.NewGuid();
        var trackId = Guid.NewGuid();

        var dummyTrack = new TrackState
        {
            Clips = new List<EventAtLocationState<Guid>>
            {
                new EventAtLocationState<Guid>
                {
                    Event = clipId,
                    Location = new LocationState
                    {
                        Active =true,
                        Start =TimeSpan.FromSeconds(1),
                    }
                }
            },
            Id = trackId,
            SourceId = Guid.NewGuid(),
        };

        var dummyClip = new ClipState
        {
            Id = clipId,
            Length = TimeSpan.FromMilliseconds(1000),
            Sounds = new List<EventAtLocationState<SoundEventState>>(),
        };

        _trackStateProvider.TryGet(Arg.Any<Guid>()).Returns(o => Task.FromResult((TrackState?)dummyTrack));

        _clipStateProvider.TryGet(Arg.Any<Guid>()).Returns(o => Task.FromResult((ClipState?)dummyClip));

        var sounds = new List<EventAtLocationDto<SoundEventDto>>
        {
            new EventAtLocationDto<SoundEventDto>
            {
                Event = new SoundEventDto
                {
                    Length_ms =1000,
                    Offset_ms =100,
                    Pitch =122,
                    Velocity = 111
                },
                Location = new LocationDto
                {
                    Active = true,
                    Start_ms = 123,
                }
            }
        };

        //Act
        var soundsRetrieved = await _trackService.CreateSounds(trackId, clipId, sounds);

        //Assert
        _notificationPublisher.Received(1).Publish(Arg.Any<AddSoundToClipNotification>());

        soundsRetrieved.Should().NotBeEmpty();
        soundsRetrieved.First().Event!.First().Pitch.Should().Be(122);
        soundsRetrieved.First().Event!.First().Velocity.Should().Be(111);
        soundsRetrieved.First().Event!.First().Length_ms.Should().Be(1000);
        soundsRetrieved.First().Event!.First().Offset_ms.Should().Be(0); // TODO why?
        soundsRetrieved.First().Location!.Start_ms.Should().Be(1000 + 123);
    }

    /// <summary>
    /// This test is weak while most of data is mocked. 
    /// Full logic test is <see cref="TrackTests"/>
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetTrackPlayableReturnSounds_Test()
    {
        // Arrange 
        var trackLength = 1000;
        var clipId = Guid.NewGuid();
        var trackId = Guid.NewGuid();

        var dummyTrack = new TrackState
        {
            Clips = new List<EventAtLocationState<Guid>>
            {
                new EventAtLocationState<Guid>
                {
                    Event = clipId,
                    Location = new LocationState
                    {
                        Active =true,
                        Start =TimeSpan.FromSeconds(1),
                    }
                }
            },
            Id = trackId,
            SourceId = Guid.NewGuid(),
        };



        _trackStateProvider.TryGet(Arg.Any<Guid>()).Returns(o => Task.FromResult((TrackState?)dummyTrack));

        var sounds = new List<EventAtLocationDto<SoundEventDto>>
        {
            new EventAtLocationDto<SoundEventDto>
            {
                Event = new SoundEventDto
                {
                    Length_ms =1000,
                    Offset_ms =100,
                    Pitch =122,
                    Velocity = 111
                },
                Location = new LocationDto
                {
                    Active = true,
                    Start_ms = 123,
                }
            }
        };

        var dummyClip = new ClipState
        {
            Id = clipId,
            Length = TimeSpan.FromMilliseconds(10000),
            Sounds = new List<EventAtLocationState<SoundEventState>>
        {
            new EventAtLocationState<SoundEventState>
            {
                Event = new SoundEventState
                {
                    Length = TimeSpan.FromMilliseconds(1000),
                    Offset = TimeSpan.FromMilliseconds(100),
                    Pitch =122,
                    Velocity = 111
                },
                Location = new LocationState
                {
                    Active = true,
                    Start =TimeSpan.FromMilliseconds(123),
                }
            }
        },
        };

        _clipStateProvider.TryGet(Arg.Any<Guid>()).Returns(o => Task.FromResult((ClipState?)dummyClip));

        //Act
        await _trackService.CreateSounds(trackId, clipId, sounds);

        var playableTrack = await _trackService.GetTrackPlayable(trackId);
        var playableClip = await _trackService.GetClipPlayable(trackId, clipId);

        //Assert

        playableTrack.Should().HaveCount(1);
        playableClip.Should().HaveCount(1);
    }
}