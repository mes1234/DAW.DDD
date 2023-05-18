using AutoMapper;
using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories;
using DAW.Services.Dto;

namespace DAW.Services.Services;
public interface ITrackService
{
    public TrackDto CreateTrack(Guid? sourceId, string name);

    public Task<ClipDto?> CreateClip(Guid trackId, int length_ms);

    public Task<IEnumerable<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> CreateSounds(Guid trackId, Guid clipId, IEnumerable<EventAtLocationDto<SoundEventDto>> sounds);
}