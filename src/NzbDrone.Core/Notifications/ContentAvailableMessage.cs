using NzbDrone.Core.TrackedContent;

namespace NzbDrone.Core.Notifications
{
    public class ContentAvailableMessage
    {
        public string Title { get; set; }
        public ContentType ContentType { get; set; }
        public string Message { get; set; }
        public TrackedItem TrackedItem { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
