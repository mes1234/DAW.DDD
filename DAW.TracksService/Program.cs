using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;
using DAW.Repositories;
using DAW.Repositories.Commands;
using DAW.Repositories.DataAccess;
using DAW.Repositories.States;
using DAW.Services.Dto;
using DAW.Services.Services;
using DAW.TracksService;
using DAW.TracksService.BackgroundWorkers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;

namespace TrackServiceCore;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        //builder.Services.AddAuthorization();

        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                              policy =>
                              {
                                  policy.WithOrigins("http://localhost:4200");
                              });
        });


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddTransient<INotificationPublisher, NotificationPublisher>();
        builder.Services.AddTransient<ModelRepository>();

        builder.Services.AddMemoryCache();

        builder.Services.AddTransient<IModelStateReader<ClipState>, InMemoryStorage<ClipState>>();
        builder.Services.Decorate<IModelStateReader<ClipState>, LocalMemoryCacheReader<ClipState>>();

        builder.Services.AddTransient<IModelStateReader<TrackState>, InMemoryStorage<TrackState>>();
        builder.Services.Decorate<IModelStateReader<TrackState>, LocalMemoryCacheReader<TrackState>>();

        builder.Services.AddTransient<IModelStateReader<StoredSound>, InMemoryStorage<StoredSound>>();
        builder.Services.Decorate<IModelStateReader<StoredSound>, LocalMemoryCacheReader<StoredSound>>();

        builder.Services.AddTransient<IModelStateWriter<ClipState>, InMemoryStorage<ClipState>>();
        builder.Services.Decorate<IModelStateWriter<ClipState>, LocalMemoryCacheWriter<ClipState>>();

        builder.Services.AddTransient<IModelStateWriter<TrackState>, InMemoryStorage<TrackState>>();
        builder.Services.Decorate<IModelStateWriter<TrackState>, LocalMemoryCacheWriter<TrackState>>();

        builder.Services.AddTransient<IModelStateWriter<StoredSound>, InMemoryStorage<StoredSound>>();
        builder.Services.Decorate<IModelStateWriter<StoredSound>, LocalMemoryCacheWriter<StoredSound>>();


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

        builder.Services.Configure<CacheOptions>(builder.Configuration.GetSection(CacheOptions.Name));

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(MyAllowSpecificOrigins);

        //app.UseAuthorization();


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

        app.MapPost("sounds/{sourceId}/{pitch}", async ([FromForm] IFormFile file, [FromRoute] Guid sourceId, [FromRoute] int pitch, [FromServices] IModelStateWriter<StoredSound> soundRepository) =>
        {
            var content = new byte[file.Length];

            await file.CopyToAsync(new MemoryStream(content));

            var sound = new StoredSound(content, file.FileName, sourceId, pitch);

            var id = StoredSoundExtensions.GetStoredId(sound.SourceId, sound.Pitch);

            await soundRepository.TryAddOrUpdate(id, sound);

            return Results.Created(new Uri($"https://localhost:7267/sounds/{sourceId}/{pitch}"), file.FileName);
        });

        app.MapGet("sounds/{sourceId}/{pitch}", async ([FromRoute] Guid sourceId, [FromRoute] int pitch, [FromServices] IModelStateReader<StoredSound> soundRepository) =>
        {
            var id = StoredSoundExtensions.GetStoredId(sourceId, pitch);

            var sound = await soundRepository.TryGet(id);

            if (sound == null) return Results.NotFound();

            return Results.File(sound.Content, contentType: "audio/wav");

        }).Produces(StatusCodes.Status200OK, contentType: "audio/wav");

        app.Run();
    }

}