using System;
using System.Collections.Generic;
using System.Text;

namespace DAW.DDD.Domain.Notifications
{
    public class NullNotificationPublisher : INotificationPublisher
    {
        public static readonly NullNotificationPublisher Instance = new();

        public void Publish(DomainNotification notification)
        {
            // No operation will take place  
        }
    }
}
