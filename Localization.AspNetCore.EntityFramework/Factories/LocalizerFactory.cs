using System;
using Localization.AspNetCore.EntityFramework.Localizers;
using Localization.AspNetCore.EntityFramework.Providers;
using Localization.AspNetCore.EntityFramework.Settings;
using Localization.AspNetCore.EntityFramework.Settings.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.AspNetCore.EntityFramework.Factories
{
    internal class LocalizerFactory<T> : IStringLocalizerFactory
        where T : DbContext
    {
        private const string Shared = nameof(Shared);
        private readonly LocalizerOptions _settings;
        private readonly LocalizationProvider<T> _provider;

        public LocalizerFactory(IOptions<LocalizerOptions> localizerOptions,
            LocalizationProvider<T> manager)
        {
            _provider = manager ?? throw new ArgumentException(nameof(manager));
            _settings = localizerOptions == null
                ? throw new ArgumentNullException(nameof(localizerOptions))
                : localizerOptions.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var sourceName = resourceSource.Name;

            switch (_settings.NamingConvention)
            {
                case NamingConventionEnum.PropertyOnly:
                    sourceName = Shared;
                    break;
                case NamingConventionEnum.FullTypeName:
                    sourceName = resourceSource.FullName;
                    break;
            }

            return new Localizer(_provider, sourceName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            var sourceName = baseName;

            switch (_settings.NamingConvention)
            {
                case NamingConventionEnum.PropertyOnly:
                    sourceName = Shared;
                    break;
                case NamingConventionEnum.FullTypeName:
                    sourceName = $"{location}_{baseName}";
                    break;
            }

            return new Localizer(_provider, sourceName);
        }
    }
}