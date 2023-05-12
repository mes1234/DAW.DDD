using AutoMapper;
using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories;
using DAW.Repositories.Commands;
using DAW.Repositories.DataAccess;
using DAW.Repositories.Dtos;
using DAW.Repositories.States;
using DAW.TracksService.BackgroundWorkers;
using DAW.TracksService.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<INotificationPublisher, NotificationPublisher>();
builder.Services.AddTransient<ModelRepository>();
builder.Services.AddTransient<IModelStateReader<ClipState>, InMemoryStorage<ClipState>>();
builder.Services.AddTransient<IModelStateReader<TrackState>, InMemoryStorage<TrackState>>();
builder.Services.AddTransient<IModelStateWriter<ClipState>, InMemoryStorage<ClipState>>();
builder.Services.AddTransient<IModelStateWriter<TrackState>, InMemoryStorage<TrackState>>();
builder.Services.AddTransient<INotificationPublisher, NotificationPublisher>();
builder.Services.AddTransient<INotificationDispatcher, NotificationPublisher>();

builder.Services.AddTransient<IDomainNotificationHandler<ClipCreatedNotification>, ClipsCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<AddSoundToClipNotification>, ClipsCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ChangeLengthOfClipNotification>, ClipsCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ChangeSourceIdOfClipNotification>, ClipsCommandsHandlers>();

builder.Services.AddTransient<IDomainNotificationHandler<TrackCreatedNotification>, TracksCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ClipAddedToTrackNotification>, TracksCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ChangeTrackSourceIdNotification>, TracksCommandsHandlers>();

builder.Services.AddHostedService<DomainEventsDispatcher>();
builder.Services.AddAutoMapper(typeof(Mappings));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPut("tracks", ([FromQuery] Guid? sourceId, [FromQuery] string name, [FromServices] INotificationPublisher notificationPublisher) =>
{
    var track = Track.Create(Enumerable.Empty<EventAtLocation<Clip>>().ToList(), sourceId ?? Guid.Empty, notificationPublisher);

    var response = new { trackId = track.Id };

    return Results.Created(new Uri($"https://localhost:7267/tracks/{track.Id}"), response);

}).WithName("PutNewTrack");

app.MapPut("tracks/{trackId}/clips", async ([FromRoute] Guid trackId, [FromQuery] int lengthSec, [FromServices] ModelRepository modelRepository, [FromServices] INotificationPublisher notificationPublisher) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track == null) return Results.BadRequest($"Track {trackId} doesn't exist");

    var clip = Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromSeconds(lengthSec), Guid.Empty, notificationPublisher);

    track.AddClip(EventAtLocation<Clip>.Create(Location.Create(track.Length), clip));

    var response = new { clipId = clip.Id };

    return Results.Created(new Uri($"https://localhost:7267/tracks/{trackId}/clips/{clip.Id}"), response);

}).WithName("PutNewClip");

app.MapPut("tracks/{trackId}/clips/{clipId}/sounds", async ([FromRoute] Guid trackId, [FromRoute] Guid clipId, [FromBody] IEnumerable<EventAtLocationDto<SoundEventDto>> sounds, [FromServices] ModelRepository modelRepository, [FromServices] INotificationPublisher notificationPublisher, [FromServices] IMapper mapper) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track == null) return Results.BadRequest($"Track {trackId} doesn't exist");

    var clip = track.Clips.FirstOrDefault(x => x.Event.Id == clipId);

    if (clip == null) return Results.BadRequest($"Clip {clipId} doesn't exist");

    AddSounds(sounds, clip);

    var soundEvents = clip.GetPlayableEvents(Location.EmptyLocation);

    var soundEventsMapped = MapPlayableEvents(mapper, soundEvents);

    return Results.Created(new Uri($"https://localhost:7267/playable/{trackId}/clips/{clipId}"), (soundEventsMapped));

}).WithName("PutSounds");


app.MapGet("tracks/{id}", async ([FromRoute] Guid id, [FromServices] ModelRepository modelRepository, [FromServices] IMapper mapper) =>
{
    var track = await modelRepository.GetTrack(id);

    if (track != null)
    {
        return Results.Ok(mapper.Map<TrackDto>(track));
    }
    else
    {
        return Results.NotFound();
    }
}).WithName("GetTrackById");

app.MapGet("playable/{trackId}", async ([FromRoute] Guid trackId, [FromServices] ModelRepository modelRepository, [FromServices] IMapper mapper) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track != null)
    {
        var sounds = track.GetPlayableEvents(Location.EmptyLocation);

        return Results.Ok(MapPlayableEvents(mapper, sounds));
    }
    else
    {
        return Results.NotFound();
    }
}).WithName("GetTrackPlayable");


app.MapGet("playable/{trackId}/clips/{clipId}", async ([FromRoute] Guid trackId, [FromRoute] Guid clipId, [FromServices] ModelRepository modelRepository, [FromServices] IMapper mapper) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track != null)
    {
        var clip = track.Clips.FirstOrDefault(x => x.Event.Id == clipId);

        if (clip != null)
        {
            var sounds = clip.GetPlayableEvents(Location.EmptyLocation);

            return Results.Ok(MapPlayableEvents(mapper, sounds));
        }
    }

    return Results.NotFound();

}).WithName("GetClipPlayable");



app.Run();

static List<EventAtLocationDto<IEnumerable<SoundEventDto>>> MapPlayableEvents(IMapper mapper, IReadOnlyCollection<EventAtLocation<IReadOnlyCollection<SoundEvent>>> soundEvents)
{
    var soundEventsMapped = new List<EventAtLocationDto<IEnumerable<SoundEventDto>>>();

    foreach (var soundEvent in soundEvents)
    {
        var mappedSoundEvent = mapper.Map<EventAtLocationDto<IEnumerable<SoundEventDto>>>(soundEvent);

        soundEventsMapped.Add(mappedSoundEvent);
    }

    return soundEventsMapped;
}

static void AddSounds(IEnumerable<EventAtLocationDto<SoundEventDto>> sounds, EventAtLocation<Clip> clip)
{
    var soundsModel = new List<EventAtLocation<SoundEvent>>();

    foreach (var sound in sounds)
    {
        soundsModel.Add(
            EventAtLocation<SoundEvent>.Create(
                Location.Create(TimeSpan.FromMilliseconds(sound.Location!.Start_ms), true),
                SoundEvent.Create(sound.Event!.Velocity, sound.Event.Pitch, TimeSpan.FromMilliseconds(sound.Event.Length_ms), TimeSpan.FromMilliseconds(sound.Event.Offset_ms))));
    }

    clip.Event.AddSounds(soundsModel);
}