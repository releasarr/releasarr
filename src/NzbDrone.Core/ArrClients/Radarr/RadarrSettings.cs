using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.ArrClients.Radarr
{
    public class RadarrSettingsValidator : AbstractValidator<RadarrSettings>
    {
        public RadarrSettingsValidator()
        {
            RuleFor(c => c.BaseUrl).IsValidUrl();
            RuleFor(c => c.ApiKey).NotEmpty();
        }
    }

    public class RadarrSettings : IProviderConfig
    {
        private static readonly RadarrSettingsValidator Validator = new RadarrSettingsValidator();

        public RadarrSettings()
        {
            BaseUrl = "http://localhost:7878";
        }

        [FieldDefinition(0, Label = "Radarr URL", Type = FieldType.Url, HelpText = "URL of your Radarr instance")]
        public string BaseUrl { get; set; }

        [FieldDefinition(1, Label = "API Key", Type = FieldType.Password, Privacy = PrivacyLevel.ApiKey, HelpText = "Radarr API key (found in Settings > General)")]
        public string ApiKey { get; set; }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
