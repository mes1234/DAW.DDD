using DAW.DDD.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Primitives;
public interface IEntity
{
    public Guid Id { get; }

    /// <summary>
    /// Attach new <see cref="INotificationPublisher"/> and activate Model
    /// </summary>
    /// <param name="notificationPublisher">Active notification publisher</param>
    public void Activate(INotificationPublisher notificationPublisher);
}
