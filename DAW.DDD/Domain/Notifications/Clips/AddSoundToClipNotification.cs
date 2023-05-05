using DAW.DDD.Domain.Entities;
using DAW.DDD.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications.Clips;
public record AddSoundToClipNotification : DomainNotification
{
    public SoundEvent Sound { get; private set; }
    public Location Location { get; private set; }

    public AddSoundToClipNotification(Guid entityId, string entityName, EventAtLocation<SoundEvent> sound) : base(entityId, entityName)
    {
        Sound = sound.Event with { };
        Location = sound.Location with { };
    }
}
