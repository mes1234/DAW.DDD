using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        public int NotificationCount => _notifications.Count;

        private readonly ConcurrentQueue<DomainNotification> _notifications = new ConcurrentQueue<DomainNotification>();

        public void Publish(DomainNotification notification)
        {
            _notifications.Enqueue(notification);
        }
    }
}
