using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications
{
    public interface INotificationPublisher
    {
        void Publish(DomainNotification notification);
    }
}
