using DAW.DDD.Domain.Notifications;
using DAW.DDD.Domain.Notifications.Clips;
using DAW.DDD.Domain.Notifications.Tracks;

namespace DAW.TracksService.BackgroundWorkers;

public class DomainEventsDispatcher : BackgroundService
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IServiceProvider _serviceProvider;

    public DomainEventsDispatcher(INotificationDispatcher dispatcher, IServiceProvider serviceProvider)
    {
        _dispatcher = dispatcher;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10);

            var notification = _dispatcher.Fetch();

            if (notification == null) continue;


            await (notification switch
            {
                TrackCreatedNotification casted => _serviceProvider.GetService<IDomainNotificationHandler<TrackCreatedNotification>>()!.Handle(casted!),
                ClipAddedToTrackNotification casted => _serviceProvider.GetService<IDomainNotificationHandler<ClipAddedToTrackNotification>>()!.Handle(casted!),
                ChangeTrackSourceIdNotification casted => _serviceProvider.GetService<IDomainNotificationHandler<ChangeTrackSourceIdNotification>>()!.Handle(casted!),
                ClipCreatedNotification casted => _serviceProvider.GetService<IDomainNotificationHandler<ClipCreatedNotification>>()!.Handle(casted!),
                AddSoundToClipNotification casted => _serviceProvider.GetService<IDomainNotificationHandler<AddSoundToClipNotification>>()!.Handle(casted!),
                ChangeLengthOfClipNotification casted => _serviceProvider.GetService<IDomainNotificationHandler<ChangeLengthOfClipNotification>>()!.Handle(casted!),
                _ => throw new NotImplementedException(),
            });
        }

    }
}
