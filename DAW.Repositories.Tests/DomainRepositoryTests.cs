using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Repositories.Tests;
public class DomainRepositoryTests
{
    private readonly IModelStateReader<ClipState> _clipStateProvider;
    private readonly IModelStateReader<TrackState> _trackStateProvider;
    private readonly INotificationPublisher _notificationPublisher;

    private readonly Guid _trackId = Guid.NewGuid();
    private readonly Guid _trackSourceId = Guid.NewGuid();
    private readonly Guid _clipId = Guid.NewGuid();

    public DomainRepositoryTests()
    {
        _clipStateProvider = NSubstitute.Substitute.For<IModelStateReader<ClipState>>();
        _trackStateProvider = NSubstitute.Substitute.For<IModelStateReader<TrackState>>();
        _notificationPublisher = NSubstitute.Substitute.For<INotificationPublisher>();

        MockClips();
        MockTracks();
    }

    [Fact]
    public async Task RepositoryWithStateShouldeRecreateModel_Test()
    {
        //Arrange
        var repository = new ModelRepository(_clipStateProvider, _trackStateProvider, _notificationPublisher);
        //Act
        var trackModel = await repository.GetTrack(_trackId);

        //Assert

        trackModel.Id.Should().Be(_trackId);
        trackModel.SourceId.Should().Be(_trackSourceId);
        trackModel.Clips.Should().HaveCount(1);
        trackModel.Clips.First().Event.Id.Should().Be(_clipId);
    }

    private void MockClips()
    {
        _clipStateProvider!.TryGet(Arg.Any<Guid>()).Returns(x => new ClipState
        {
            Id = (Guid)x[0],
            Length = TimeSpan.FromSeconds(1),
            Sounds = new[]
            {
              new EventAtLocationState<SoundEventState>
              {
                  Event = new SoundEventState
                  {
                      Length = TimeSpan.FromSeconds(1),
                      Offset = TimeSpan.FromSeconds(2),
                      Pitch = 2,
                      Velocity =2,
                  },
                  Location = new LocationState
                  {
                      Active = true,
                      Start = TimeSpan.FromSeconds(1),
                  }
              },
              new EventAtLocationState<SoundEventState>
              {
                  Event = new SoundEventState
                  {
                      Length = TimeSpan.FromSeconds(1),
                      Offset = TimeSpan.FromSeconds(2),
                      Pitch = 2,
                      Velocity =2,
                  },
                  Location = new LocationState
                  {
                      Active = true,
                      Start = TimeSpan.FromSeconds(1),
                  }
              }
            },
            SourceId = Guid.NewGuid(),
        });
    }
    private void MockTracks()
    {
        _trackStateProvider!.TryGet(Arg.Any<Guid>()).Returns(x => new TrackState
        {
            Id = (Guid)x[0],

            Clips = new[]
            {
              new EventAtLocationState<Guid>
              {
                  Event = _clipId,
                  Location = new LocationState
                  {
                      Active = true,
                      Start = TimeSpan.FromSeconds(1),
                  }
              }
            },
            SourceId = _trackSourceId,
        });
    }
}
