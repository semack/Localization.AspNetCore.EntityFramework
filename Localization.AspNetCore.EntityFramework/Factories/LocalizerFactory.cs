using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localization.AspNetCore.EntityFramework.Abstract;
using Localization.AspNetCore.EntityFramework.Entities;
using Localization.AspNetCore.EntityFramework.Enums;
using Localization.AspNetCore.EntityFramework.Extensions;
using Localization.AspNetCore.EntityFramework.Models;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.AspNetCore.EntityFramework.Factories
{
    internal class LocalizerFactory<T> : IStringLocalizerFactory, ILocalizationManager
        where T : DbContext
    {
        private const string Shared = nameof(Shared);
        private readonly LocalizerOptions _localizerSettings;
        private readonly RequestLocalizationOptions _requestLocalizationSettings;
        private readonly IServiceProvider _serviceProvider;
        private List<LocalizationModel> _cache = new List<LocalizationModel>();

        public LocalizerFactory(IServiceProvider serviceProvider,
            IOptions<LocalizerOptions> localizerOptions,
            IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _serviceProvider = serviceProvider;
            _localizerSettings = localizerOptions == null
                ? throw new ArgumentNullException(nameof(localizerOptions))
                : localizerOptions.Value;
            _requestLocalizationSettings = requestLocalizationOptions == null
                ? throw new ArgumentNullException(nameof(requestLocalizationOptions))
                : requestLocalizationOptions.Value;
            ResetCache();
        }

        private string CurrentLanguage => CultureInfo.CurrentUICulture.Name;

        public void ResetCache()
        {
            lock (_cache)
            {
                using (var scope = _serviceProvider.GetScopedService(out T context))
                {
                    _cache = context.Set<LocalizationResource>()
                        .SelectMany(l => l.Translations)
                        .Select(t => new LocalizationModel
                        {
                            Id = t.Resource.Id,
                            ResourceId = t.ResourceId,
                            ResourceKey = t.Resource.ResourceKey,
                            Language = t.Language,
                            Value = t.Value
                        })
                        .ToList();
                }
            }
        }


        public void Import(IList<LocalizationResource> source)
        {
            throw new NotImplementedException();
        }

        public IList<LocalizationResource> Export()
        {
            throw new NotImplementedException();
        }

        public IList<CultureInfo> SupportedCultures => _requestLocalizationSettings.SupportedCultures;

        public LocalizedString GetResource(string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentException(nameof(resourceKey));

            var values = _cache
                .Where(t => t.ResourceKey == resourceKey
                            && t.Language == CurrentLanguage)
                .Select(p => p.Value)
                .ToList();

            if (_localizerSettings.CreateNewRecordWhenLocalisedStringDoesNotExist && !values.Any())
                AddNewResourceKey(resourceKey);

            var result = values.SingleOrDefault();

            if (_localizerSettings.ReturnOnlyKeyIfNotFound && string.IsNullOrWhiteSpace(result))
                result = resourceKey;
            return new LocalizedString(resourceKey, result!);
        }

        public void Sync()
        {
            throw new NotImplementedException();
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

            return new Localizer(this, sourceName);
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

            return new Localizer(this, sourceName);
        }

        private void UpdateCache(string resourceKey)
        {
            lock (_cache)
            {
                using (var scope = _serviceProvider.GetScopedService(out T context))
                {
                    var values = context.Set<LocalizationResource>()
                        .Where(r => r.ResourceKey == resourceKey)
                        .SelectMany(l => l.Translations)
                        .Select(t => new LocalizationModel
                        {
                            Id = t.Resource.Id,
                            ResourceId = t.ResourceId,
                            ResourceKey = t.Resource.ResourceKey,
                            Language = t.Language,
                            Value = t.Value
                        })
                        .ToList();

                    _cache.Where(m => m.ResourceKey == resourceKey)
                        .ToList()
                        .ForEach(item => { _cache.Remove(item); });

                    if (values.Any())
                        _cache.AddRange(values);
                }
            }
        }

        private void AddNewResourceKey(string resourceKey)
        {
            using (var scope = _serviceProvider.GetScopedService(out T context))
            {
                var resource = context
                    .Set<LocalizationResource>()
                    .Include(t => t.Translations)
                    .SingleOrDefault(r => r.ResourceKey == resourceKey);

                if (resource == null)
                {
                    resource = new LocalizationResource
                    {
                        ResourceKey = resourceKey,
                        Modified = DateTime.UtcNow,
                        Translations = new List<LocalizationResourceTranslation>()
                    };

                    context.Add(resource);
                }

                foreach (var culture in SupportedCultures)
                    if (resource.Translations.All(t => t.Language != culture.Name))
                    {
                        resource.Modified = DateTime.UtcNow;
                        resource.Translations.Add(new LocalizationResourceTranslation
                        {
                            Language = culture.Name,
                            Modified = DateTime.UtcNow
                        });
                    }

                context.SaveChanges();

                UpdateCache(resourceKey);
            }
        }
    }
}