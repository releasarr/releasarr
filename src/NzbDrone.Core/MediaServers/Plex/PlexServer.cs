using System;
using System.Collections.Generic;
using FluentValidation.Results;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.MediaServers.Plex
{
    public class PlexServer : IMediaServer
    {
        private readonly IPlexProxy _proxy;

        public PlexServer(IPlexProxy proxy)
        {
            _proxy = proxy;
        }

        public string Name => "Plex";
        public string Link => "https://www.plex.tv/";

        public Type ConfigContract => typeof(PlexSettings);

        public ProviderMessage Message => null;

        public IEnumerable<ProviderDefinition> DefaultDefinitions => new List<ProviderDefinition>();

        public ProviderDefinition Definition { get; set; }

        public PlexSettings Settings => (PlexSettings)Definition.Settings;

        public ValidationResult Test()
        {
            var failures = new List<ValidationFailure>();
            failures.AddIfNotNull(_proxy.Test(Settings));
            return new ValidationResult(failures);
        }

        public object RequestAction(string stage, IDictionary<string, string> query)
        {
            return null;
        }

        public List<PlexWatchlistItem> GetWatchlistItems()
        {
            var items = new List<PlexWatchlistItem>();

            if (Settings.WatchlistEnabled)
            {
                items.AddRange(_proxy.GetWatchlist(Settings));
            }

            if (!string.IsNullOrWhiteSpace(Settings.PlaylistName))
            {
                items.AddRange(_proxy.GetPlaylistItems(Settings.PlaylistName, Settings));
            }

            return items;
        }
    }
}
