using DAW.DDD.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications;
public interface IDomainNotificationHandler<in T> where T : DomainNotification
{
    Task Handle(T notification);
}
