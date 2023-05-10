using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Tracks;
public record ChangeTrackSourceIdNotification : DomainNotification
{
    public Guid SourceId { get; private set; }

    public ChangeTrackSourceIdNotification(Guid entityId, string entityName, Guid sourceId) : base(entityId, entityName)
    {
        SourceId = sourceId;
    }
}
