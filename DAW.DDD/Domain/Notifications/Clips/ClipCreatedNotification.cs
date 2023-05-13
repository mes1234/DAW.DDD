using DAW.DDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Clips;
public record ClipCreatedNotification : DomainNotification
{
    public ClipCreatedNotification(Guid entityId, string entityName, Clip clip) : base(entityId, entityName)
    {
    }
}
