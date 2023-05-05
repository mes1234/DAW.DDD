using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Clips;
public record ChangeLengthOfClipNotification : DomainNotification
{
    public TimeSpan Length { get; private set; }

    public ChangeLengthOfClipNotification(Guid entityId, string entityName, TimeSpan length) : base(entityId, entityName)
    {
        Length = length;
    }
}
