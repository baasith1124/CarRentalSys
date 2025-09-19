using CarRentalSystem.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;

namespace CarRentalSystem.Web.Services
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationService(IStringLocalizer<ValidationMessages> localizer)
        {
            _localizer = localizer;
        }

        public string GetString(string key, params object[] args)
        {
            var localizedString = _localizer[key];
            return args.Length > 0 ? string.Format(localizedString, args) : localizedString;
        }

        public string GetString(string key)
        {
            return _localizer[key];
        }
    }

    // Dummy class for resource file reference
    public class ValidationMessages
    {
    }
}
