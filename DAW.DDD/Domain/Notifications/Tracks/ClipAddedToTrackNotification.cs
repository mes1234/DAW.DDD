using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Tracks;
public record ClipAddedToTrackNotification : DomainNotification
{
    public Guid ClipId { get; private set; }

    public TimeSpan Start { get; private set; }

    public ClipAddedToTrackNotification(Guid entityId, string entityName, EventAtLocation<Clip> clip) : base(entityId, entityName)
    {
        ClipId = clip.Event.Id;
        Start = clip.Location.Start;
    }
}
