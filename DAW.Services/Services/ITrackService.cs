using AutoMapper;
using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories;
using DAW.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.Services.Services;
public interface ITrackService
{
    public TrackDto CreateTrack(Guid? sourceId, string name);

    public Task<ClipDto> CreateClip(Guid trackId, int length_ms);

    public Task<IEnumerable<EventAtLocationDto<IEnumerable<SoundEventDto>>>> CreateSounds(Guid trackId, Guid clipId, IEnumerable<EventAtLocationDto<SoundEventDto>> sounds);
}

public interface ITrackView
{
    Task<TrackDto?> GetTrack(Guid trackId);

    Task<List<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> GetTrackPlayable(Guid trackId);

    Task<List<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> GetClipPlayable(Guid trackId, Guid clipId);
}

public class TrackService : ITrackService, ITrackView
{
    private readonly INotificationPublisher _notificationPublisher;
    private readonly IMapper _mapper;
    private readonly ModelRepository _modelRepository;

    public TrackService(INotificationPublisher notificationPublisher, IMapper mapper, ModelRepository modelRepository)
    {
        _notificationPublisher = notificationPublisher;
        _mapper = mapper;
        _modelRepository = modelRepository;
    }

    public async Task<ClipDto> CreateClip(Guid trackId, int length_ms)
    {
        var track = await _modelRepository.GetTrack(trackId);

        //TODO exception model
        if (track == null) throw new Exception("TODO");

        var clip = Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromMilliseconds(length_ms), _notificationPublisher);

        track.AddClip(EventAtLocation<Clip>.Create(Location.Create(track.Length), clip));

        var mapped = _mapper.Map<ClipDto>(clip);

        return mapped;
    }

    public async Task<IEnumerable<EventAtLocationDto<IEnumerable<SoundEventDto>>>> CreateSounds(Guid trackId, Guid clipId, IEnumerable<EventAtLocationDto<SoundEventDto>> sounds)
    {
        var track = await _modelRepository.GetTrack(trackId);

        if (track == null) throw new Exception("TODO");

        var clip = track.Clips.FirstOrDefault(x => x.Event.Id == clipId);

        if (clip == null) throw new Exception("TODO");

        clip.Event.AddSounds(sounds.MapSounds());

        var soundEvents = clip.GetPlayableEvents(Location.EmptyLocation);

        var soundEventsMapped = soundEvents.MapPlayableEvents(_mapper);

        return soundEventsMapped;
    }

    public TrackDto CreateTrack(Guid? sourceId, string name)
    {
        var track = Track.Create(Enumerable.Empty<EventAtLocation<Clip>>().ToList(), sourceId ?? Guid.Empty, _notificationPublisher);

        var mapped = _mapper.Map<TrackDto>(track);

        return mapped;
    }

    public async Task<List<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> GetClipPlayable(Guid trackId, Guid clipId)
    {
        var clip = await _modelRepository.GetClip(clipId);

        if (clip == null) return null;

        var sounds = clip.GetPlayableEvents(Location.EmptyLocation);

        return sounds.MapPlayableEvents(_mapper);
    }

    public async Task<TrackDto?> GetTrack(Guid trackId)
    {
        var track = await _modelRepository.GetTrack(trackId);

        if (track != null)
            return _mapper.Map<TrackDto>(track);

        else return null;

    }

    public async Task<List<EventAtLocationDto<IEnumerable<SoundEventDto>>>?> GetTrackPlayable(Guid trackId)
    {
        var track = await _modelRepository.GetTrack(trackId);

        if (track == null) return null;

        var sounds = track.GetPlayableEvents(Location.EmptyLocation);

        return sounds.MapPlayableEvents(_mapper);
    }
}

public static class SoundsExtensions
{
    public static List<EventAtLocationDto<IEnumerable<SoundEventDto>>> MapPlayableEvents(this IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> soundEvents, IMapper mapper)
    {
        var soundEventsMapped = new List<EventAtLocationDto<IEnumerable<SoundEventDto>>>();

        foreach (var soundEvent in soundEvents)
        {
            var mappedSoundEvent = mapper.Map<EventAtLocationDto<IEnumerable<SoundEventDto>>>(soundEvent);

            soundEventsMapped.Add(mappedSoundEvent);
        }

        return soundEventsMapped;
    }

    public static ICollection<EventAtLocation<SoundEvent>> MapSounds(this IEnumerable<EventAtLocationDto<SoundEventDto>> sounds)
    {
        var soundsModel = new List<EventAtLocation<SoundEvent>>();

        foreach (var sound in sounds)
        {
            soundsModel.Add(
                EventAtLocation<SoundEvent>.Create(
                    Location.Create(TimeSpan.FromMilliseconds(sound.Location!.Start_ms), true),
                    SoundEvent.Create(sound.Event!.Velocity, sound.Event.Pitch, TimeSpan.FromMilliseconds(sound.Event.Length_ms), TimeSpan.FromMilliseconds(sound.Event.Offset_ms))));
        }

        return soundsModel;
    }
}