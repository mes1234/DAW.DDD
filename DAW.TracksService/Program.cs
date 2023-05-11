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
using Microsoft.AspNetCore.Mvc;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPut("tracks", ([FromQuery] Guid sourceId, [FromQuery] string name, [FromServices] INotificationPublisher notificationPublisher) =>
{
    var track = Track.Create(Enumerable.Empty<EventAtLocation<Clip>>().ToList(), sourceId, notificationPublisher);

    return Results.Created(new Uri($"https://localhost:7267/tracks/{track.Id}"), track.Id);

}).WithName("PutNewTrack");

app.MapPut("tracks/{trackId}/clips", async ([FromRoute] Guid trackId, [FromQuery] int lengthSec, [FromServices] ModelRepository modelRepository, [FromServices] INotificationPublisher notificationPublisher) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track == null) return Results.BadRequest($"Track {trackId} doesn't exist");

    var clip = Clip.Create(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList(), TimeSpan.FromSeconds(lengthSec), Guid.Empty, notificationPublisher);

    return Results.Created(new Uri($"https://localhost:7267/tracks/{trackId}/clips/{clip.Id}"), clip.Id);

}).WithName("PutNewClip");


app.MapPut("tracks/{trackId}/clips/{clipId}/sounds", async ([FromRoute] Guid trackId, [FromRoute] Guid clipId, [FromBody] IEnumerable<EventAtLocationDto<SoundEventDto>> sounds, [FromServices] ModelRepository modelRepository, [FromServices] INotificationPublisher notificationPublisher) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track == null) return Results.BadRequest($"Track {trackId} doesn't exist");

    var clip = track.Clips.FirstOrDefault(x => x.Event.Id == clipId);

    if (clip == null) return Results.BadRequest($"Clip {clipId} doesn't exist");

    clip.Event.AddSounds(Enumerable.Empty<EventAtLocation<SoundEvent>>().ToList());

    //Return DTO!!
    return Results.Created(new Uri($"https://localhost:7267/playable/{trackId}/clips/{clipId}"), clip.GetPlayableEvents(Location.EmptyLocation));

}).WithName("PutSounds");


app.MapGet("tracks/{id}", async ([FromRoute] Guid id, [FromServices] ModelRepository modelRepository) =>
{
    var track = await modelRepository.GetTrack(id);

    if (track != null)
    {
        //TODO add mapper
        return Results.Ok(new TrackDto());
    }
    else
    {
        return Results.NotFound();
    }
}).WithName("GetTrackById");

app.MapGet("playable/{trackId}", async ([FromRoute] Guid trackId, [FromServices] ModelRepository modelRepository) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track != null)
    {
        //TODO add mapper
        return Results.Ok(track.GetPlayableEvents(Location.EmptyLocation));
    }
    else
    {
        return Results.NotFound();
    }
}).WithName("GetTrackPlayable");


app.MapGet("playable/{trackId}/clips/{clipId}", async ([FromRoute] Guid trackId, [FromRoute] Guid clipId, [FromServices] ModelRepository modelRepository) =>
{
    var track = await modelRepository.GetTrack(trackId);

    if (track != null)
    {
        var clip = track.Clips.FirstOrDefault(x => x.Event.Id == clipId);

        if (clip != null)
        {
            //TODO add mapper
            return Results.Ok(track.GetPlayableEvents(Location.EmptyLocation));
        }
    }

    return Results.NotFound();

}).WithName("GetClipPlayable");



app.Run();