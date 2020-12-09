using System;
using Localization.AspNetCore.EntityFramework.Enums;
using Localization.AspNetCore.EntityFramework.Managers.Abstract;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.AspNetCore.EntityFramework.Factories
{
    internal class LocalizerFactory : IStringLocalizerFactory
    {
        private readonly ILocalizationManager _manager;
        private readonly LocalizerOptions _localizerSettings;
        private const string Shared = nameof(Shared);

        public LocalizerFactory(ILocalizationManager manager,
        IOptions<LocalizerOptions> localizerOptions)
        {
            _manager = manager;
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
                // case NamingConventionEnum.FullTypeName:
                //     sourceName = baseName + location;
                //     break;
            }
            
            return new Localizer(_manager, sourceName);
        }
    }
}