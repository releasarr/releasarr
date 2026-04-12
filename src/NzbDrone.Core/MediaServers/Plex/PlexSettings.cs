using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.MediaServers.Plex
{
    public class PlexSettingsValidator : AbstractValidator<PlexSettings>
    {
        public PlexSettingsValidator()
        {
            RuleFor(c => c.AuthToken).NotEmpty();
        }
    }

    public class PlexSettings : IProviderConfig
    {
        private static readonly PlexSettingsValidator Validator = new PlexSettingsValidator();

        public PlexSettings()
        {
            ServerUrl = "http://localhost:32400";
            WatchlistEnabled = true;
        }

        [FieldDefinition(0, Label = "Server URL", Type = FieldType.Url, HelpText = "Plex server URL (e.g. http://localhost:32400)")]
        public string ServerUrl { get; set; }

        [FieldDefinition(1, Label = "Auth Token", Type = FieldType.Password, Privacy = PrivacyLevel.ApiKey, HelpText = "Plex authentication token")]
        public string AuthToken { get; set; }

        [FieldDefinition(2, Label = "Monitor Watchlist", Type = FieldType.Checkbox, HelpText = "Monitor your Plex watchlist for new content")]
        public bool WatchlistEnabled { get; set; }

        [FieldDefinition(3, Label = "Playlist Name", HelpText = "Optional: name of a Plex playlist to monitor (leave blank to skip)")]
        public string PlaylistName { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
