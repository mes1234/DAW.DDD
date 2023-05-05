using DAW.DDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Tracks;
public record TrackCreatedNotification : DomainNotification
{
    public Guid SourceId { get; private set; }
    public TrackCreatedNotification(Guid entityId, string entityName, Track track) : base(entityId, entityName)
    {
        SourceId = track.SourceId;
    }
}
