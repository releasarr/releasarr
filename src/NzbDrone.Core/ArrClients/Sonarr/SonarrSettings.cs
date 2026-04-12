using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.ArrClients.Sonarr
{
    public class SonarrSettingsValidator : AbstractValidator<SonarrSettings>
    {
        public SonarrSettingsValidator()
        {
            RuleFor(c => c.BaseUrl).IsValidUrl();
            RuleFor(c => c.ApiKey).NotEmpty();
        }
    }

    public class SonarrSettings : IProviderConfig
    {
        private static readonly SonarrSettingsValidator Validator = new SonarrSettingsValidator();

        public SonarrSettings()
        {
            BaseUrl = "http://localhost:8989";
        }

        [FieldDefinition(0, Label = "Sonarr URL", Type = FieldType.Url, HelpText = "URL of your Sonarr instance")]
        public string BaseUrl { get; set; }

        [FieldDefinition(1, Label = "API Key", Type = FieldType.Password, Privacy = PrivacyLevel.ApiKey, HelpText = "Sonarr API key (found in Settings > General)")]
        public string ApiKey { get; set; }

        [FieldDefinition(2, Label = "Sync Tag", Type = FieldType.Select, SelectOptionsProviderAction = "getTags", HelpText = "Series with this tag in Sonarr will be auto-tracked in Releasarr (optional)")]
        public int? SyncTagId { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
