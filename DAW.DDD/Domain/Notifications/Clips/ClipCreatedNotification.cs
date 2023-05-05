﻿using DAW.DDD.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Clips;
public record ClipCreatedNotification : DomainNotification
{
    public Guid SourceId { get; private set; }

    public ClipCreatedNotification(Guid entityId, string entityName, Clip clip) : base(entityId, entityName)
    {
        SourceId = clip.SourceId;
    }
}
