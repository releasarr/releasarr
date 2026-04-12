using System;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Common.Http;

namespace NzbDrone.Core.Notifications.Pushover
{
    public interface IPushoverProxy
    {
        void SendNotification(string title, string message, PushoverSettings settings);
        void SendNotification(string title, string message, string url, string urlTitle, string imageUrl, PushoverSettings settings);
        ValidationFailure Test(PushoverSettings settings);
    }

    public class PushoverProxy : IPushoverProxy
    {
        private const string URL = "https://api.pushover.net/1/messages.json";
        private readonly IHttpClient _httpClient;
        private readonly Logger _logger;

        public PushoverProxy(IHttpClient httpClient, Logger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public void SendNotification(string title, string message, PushoverSettings settings)
        {
            SendNotification(title, message, null, null, null, settings);
        }

        public void SendNotification(string title, string message, string url, string urlTitle, string imageUrl, PushoverSettings settings)
        {
            var requestBuilder = new HttpRequestBuilder(URL).Post();

            requestBuilder.AddFormParameter("token", settings.ApiKey)
                          .AddFormParameter("user", settings.UserKey)
                          .AddFormParameter("device", string.Join(",", settings.Devices))
                          .AddFormParameter("title", title)
                          .AddFormParameter("message", message)
                          .AddFormParameter("priority", settings.Priority)
                          .AddFormParameter("html", "1");

            if (!url.IsNullOrWhiteSpace())
            {
                requestBuilder.AddFormParameter("url", url);
            }

            if (!urlTitle.IsNullOrWhiteSpace())
            {
                requestBuilder.AddFormParameter("url_title", urlTitle);
            }

            if ((PushoverPriority)settings.Priority == PushoverPriority.Emergency)
            {
                requestBuilder.AddFormParameter("retry", settings.Retry);
                requestBuilder.AddFormParameter("expire", settings.Expire);
            }

            if (!settings.Sound.IsNullOrWhiteSpace())
            {
                requestBuilder.AddFormParameter("sound", settings.Sound);
            }

            // Attach poster image if available
            if (!imageUrl.IsNullOrWhiteSpace())
            {
                try
                {
                    // Use w300 size for smaller download (Pushover limit is 2.5MB)
                    var smallImageUrl = imageUrl.Replace("/original/", "/w300/");
                    var imageRequest = new HttpRequestBuilder(smallImageUrl).Build();
                    var imageResponse = _httpClient.Get(imageRequest);

                    if (imageResponse.ResponseData != null && imageResponse.ResponseData.Length > 0)
                    {
                        requestBuilder.AddFormUpload("attachment", "poster.jpg", imageResponse.ResponseData, "image/jpeg");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex, "Failed to download poster image for Pushover notification");
                }
            }

            var request = requestBuilder.Build();

            _httpClient.Post(request);
        }

        public ValidationFailure Test(PushoverSettings settings)
        {
            try
            {
                const string title = "Test Notification";
                const string body = "This is a test message from Releasarr";

                SendNotification(title, body, settings);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to send test message");
                return new ValidationFailure("ApiKey", "Unable to send test message");
            }

            return null;
        }
    }
}
