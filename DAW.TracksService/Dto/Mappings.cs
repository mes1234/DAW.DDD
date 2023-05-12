using AutoMapper;
using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories.Dtos;
using System.Collections.Generic;

namespace DAW.TracksService.Dto;

public class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<Clip, ClipDto>().
            ForMember(d => d.Length_ms, o => o.MapFrom(s => s.Length.TotalMilliseconds));

        CreateMap<Track, TrackDto>()
            .ForMember(d => d.Length_ms, o => o.MapFrom(s => s.Length.TotalMilliseconds));

        CreateMap<SoundEvent, SoundEventDto>().
            ForMember(d => d.Length_ms, o => o.MapFrom(s => s.Length.TotalMilliseconds));

        CreateMap<Location, LocationDto>().
        ForMember(d => d.Start_ms, o => o.MapFrom(s => s.Start.TotalMilliseconds));

        CreateMap<EventAtLocation<SoundEvent>, EventAtLocationDto<SoundEventDto>>();

        CreateMap<EventAtLocation<List<SoundEvent>>, EventAtLocationDto<IEnumerable<SoundEventDto>>>();

        CreateMap<EventAtLocation<Clip>, EventAtLocationDto<ClipDto>>();

        CreateMap<EventAtLocation<IReadOnlyCollection<SoundEvent>>, EventAtLocationDto<IEnumerable<SoundEventDto>>>();
    }
}