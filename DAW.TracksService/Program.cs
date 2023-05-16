using AutoMapper;
using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.DDD.Domain.ValueObjects;
using DAW.Repositories;
using DAW.Repositories.Commands;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using DAW.Services.Dto;
using DAW.Services.Services;
using DAW.TracksService.BackgroundWorkers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<INotificationPublisher, NotificationPublisher>();
builder.Services.AddTransient<ModelRepository>();

builder.Services.AddMemoryCache();

builder.Services.AddTransient<IModelStateReader<ClipState>, InMemoryStorage<ClipState>>();
builder.Services.Decorate<IModelStateReader<ClipState>, LocalMemoryCacheReader<ClipState>>();

builder.Services.AddTransient<IModelStateReader<TrackState>, InMemoryStorage<TrackState>>();
builder.Services.Decorate<IModelStateReader<TrackState>, LocalMemoryCacheReader<TrackState>>();

builder.Services.AddTransient<IModelStateWriter<ClipState>, InMemoryStorage<ClipState>>();
builder.Services.Decorate<IModelStateWriter<ClipState>, LocalMemoryCacheWriter<ClipState>>();

builder.Services.AddTransient<IModelStateWriter<TrackState>, InMemoryStorage<TrackState>>();
builder.Services.Decorate<IModelStateWriter<TrackState>, LocalMemoryCacheWriter<TrackState>>();


builder.Services.AddTransient<INotificationPublisher, NotificationPublisher>();
builder.Services.AddTransient<INotificationDispatcher, NotificationPublisher>();

builder.Services.AddTransient<IDomainNotificationHandler<ClipCreatedNotification>, ClipsCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<AddSoundToClipNotification>, ClipsCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ChangeLengthOfClipNotification>, ClipsCommandsHandlers>();

builder.Services.AddTransient<IDomainNotificationHandler<TrackCreatedNotification>, TracksCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ClipAddedToTrackNotification>, TracksCommandsHandlers>();
builder.Services.AddTransient<IDomainNotificationHandler<ChangeTrackSourceIdNotification>, TracksCommandsHandlers>();
builder.Services.AddTransient<ITrackService, TrackService>();
builder.Services.AddTransient<ITrackView, TrackService>();

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



app.MapPut("tracks", ([FromQuery] Guid? sourceId, [FromQuery] string name, [FromServices] ITrackService trackService) =>
{
    var track = trackService.CreateTrack(sourceId, name);

    var response = new { trackId = track.Id };

    return Results.Created(new Uri($"https://localhost:7267/tracks/{track.Id}"), response);

}).WithName("PutNewTrack");

app.MapPut("tracks/{trackId}/clips", async ([FromRoute] Guid trackId, [FromQuery] int length_ms, [FromServices] ITrackService trackService) =>
{
    var clip = await trackService.CreateClip(trackId, length_ms);

    var response = new { clipId = clip?.Id ?? Guid.Empty };

    return (clip != null)
    ? Results.Created(new Uri($"https://localhost:7267/tracks/{trackId}/clips/{clip.Id}"), response)
    : Results.NotFound(trackId);

}).WithName("PutNewClip");

app.MapPut("tracks/{trackId}/clips/{clipId}/sounds", async ([FromRoute] Guid trackId, [FromRoute] Guid clipId, [FromBody] IEnumerable<EventAtLocationDto<SoundEventDto>> sounds, [FromServices] ITrackService trackService) =>
{
    var soundEventsMapped = await trackService.CreateSounds(trackId, clipId, sounds);

    return (soundEventsMapped != null)
    ? Results.Created(new Uri($"https://localhost:7267/playable/{trackId}/clips/{clipId}"), (soundEventsMapped))
    : Results.NotFound(clipId);

}).WithName("PutSounds");


app.MapGet("tracks/{id}", async ([FromRoute] Guid id, [FromServices] ITrackView trackView) =>
{
    var track = await trackView.GetTrack(id);

    return (track != null)
    ? Results.Ok(track)
    : Results.NotFound();

}).WithName("GetTrackById");

app.MapGet("playable/{trackId}", async ([FromRoute] Guid trackId, [FromServices] ITrackView trackView) =>
{
    var playable = await trackView.GetTrackPlayable(trackId);

    return (playable != null)
    ? Results.Ok(playable)
    : Results.NotFound();

}).WithName("GetTrackPlayable");


app.MapGet("playable/{trackId}/clips/{clipId}", async ([FromRoute] Guid trackId, [FromRoute] Guid clipId, [FromServices] ITrackView trackView) =>
{
    var playable = await trackView.GetClipPlayable(trackId, clipId);

    return (playable != null)
    ? Results.Ok(playable)
    : Results.NotFound();

}).WithName("GetClipPlayable");

app.Run();
