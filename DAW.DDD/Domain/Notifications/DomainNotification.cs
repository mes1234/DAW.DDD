using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications;
public abstract record DomainNotification
{
    public DateTimeOffset TimeStamp { get; private set; }

    public Guid EntityId { get; private set; }

    public string EntityName { get; private set; }

    protected DomainNotification(Guid entityId, string entityName)
    {

        TimeStamp = DateTimeOffset.Now;
        EntityId = entityId;
        EntityName = entityName;
    }
}
