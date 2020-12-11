using System;
using Localization.AspNetCore.EntityFramework.Enums;
using Localization.AspNetCore.EntityFramework.Managers;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.AspNetCore.EntityFramework.Localizers
{
    internal class LocalizerFactory<T> : IStringLocalizerFactory
    where T : DbContext
    {
        private readonly LocalizationManager<T> _manager;
        private readonly LocalizerOptions _localizerSettings;
        private const string Shared = nameof(Shared);

        public LocalizerFactory(IOptions<LocalizerOptions> localizerOptions,
            LocalizationManager<T> manager)
        {
            _manager = manager ?? throw new ArgumentException(nameof(manager));
            _localizerSettings = localizerOptions == null
                ? throw new ArgumentNullException(nameof(localizerOptions))
                : localizerOptions.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var sourceName = resourceSource.Name;

            switch (_localizerSettings.NamingConvention)
            {
                case NamingConventionEnum.PropertyOnly:
                    sourceName = Shared;
                    break;
                case NamingConventionEnum.FullTypeName:
                    sourceName = resourceSource.FullName;
                    break;
            }

            return new Localizer(_manager, sourceName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var sourceName = baseName;

            switch (_localizerSettings.NamingConvention)
            {
                case NamingConventionEnum.PropertyOnly:
                    sourceName = Shared;
                    break;
                case NamingConventionEnum.FullTypeName:
                    sourceName = $"{location}_{baseName}";
                    break;
            }

            return new Localizer(_manager, sourceName);
        }
    }
}