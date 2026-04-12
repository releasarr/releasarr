namespace NzbDrone.Core.Notifications.Webhook
{
    public class WebhookContentAvailablePayload : WebhookPayload
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
