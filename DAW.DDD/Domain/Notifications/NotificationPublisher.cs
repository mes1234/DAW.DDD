using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications
{
    // TODO switch to observable
    public class NotificationPublisher : INotificationPublisher, INotificationDispatcher
    {

        public int NotificationCount => _notifications.Count;

        private static readonly ConcurrentQueue<DomainNotification> _notifications = new ConcurrentQueue<DomainNotification>();

        public void Publish(DomainNotification notification)
        {
            _notifications.Enqueue(notification);
        }

        public DomainNotification Fetch()
        {
            var status = _notifications.TryDequeue(out var notification);

            if (status) return notification;

            else return null;
        }
    }
}
