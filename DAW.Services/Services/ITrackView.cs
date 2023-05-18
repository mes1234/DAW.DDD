using AutoMapper;
using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories;
using DAW.Services.Dto;

namespace DAW.Services.Services;

public interface ITrackView
{
    Task<TrackDto?> GetTrack(Guid trackId);

    Task<List<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> GetTrackPlayable(Guid trackId);

    Task<List<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> GetClipPlayable(Guid trackId, Guid clipId);
}
