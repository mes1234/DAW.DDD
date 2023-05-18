using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAW.DDD.Domain.Notifications
{
    // TODO make it observable!!
    public interface INotificationDispatcher
    {
        DomainNotification Fetch();
    }
}
