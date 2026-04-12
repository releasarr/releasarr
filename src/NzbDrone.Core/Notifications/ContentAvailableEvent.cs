using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Notifications
{
    public class ContentAvailableEvent : IEvent
    {
        public ContentAvailableMessage ContentMessage { get; set; }

        public ContentAvailableEvent(ContentAvailableMessage message)
        {
            ContentMessage = message;
        }
    }
}
