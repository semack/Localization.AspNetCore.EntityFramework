using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localization.AspNetCore.EntityFramework.Entities;
using Localization.AspNetCore.EntityFramework.Enums;
using Localization.AspNetCore.EntityFramework.Extensions;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.AspNetCore.EntityFramework.Managers
{
    internal class LocalizationManager<T> : ILocalizationManager
        where T : DbContext
    {
        private readonly LocalizerOptions _localizerSettings;
        private readonly RequestLocalizationOptions _requestLocalizationSettings;
        private readonly IServiceProvider _serviceProvider;
        private List<LocalizationResource> _cache = new List<LocalizationResource>();

        public LocalizationManager(IServiceProvider serviceProvider,
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

        private string DefaultLanguage => _requestLocalizationSettings.DefaultRequestCulture.UICulture.Name;

        public void ResetCache()
        {
            lock (_cache)
            {
                using (var scope = _serviceProvider.GetScopedService(out T context))
                {
                    _cache = context.Set<LocalizationResource>()
                        .Include(r => r.Translations)
                        .AsNoTracking()
                        .ToList();
                }
            }
        }

        public void Import(IList<LocalizationResource> source)
        {
            if (source == null)
                throw new ArgumentException(nameof(source));

            using (var scope = _serviceProvider.GetScopedService(out T context))
            {
                //  clear all resources
                context.Set<LocalizationResource>()
                    .RemoveRange(context.Set<LocalizationResource>());
                // add import
                context.Set<LocalizationResource>()
                    .AddRange(source);

                context.SaveChanges();
            }
        }

        public IList<LocalizationResource> Export()
        {
            using (var scope = _serviceProvider.GetScopedService(out T context))
            {
                var values = context.Set<LocalizationResource>()
                    .Include(r => r.Translations)
                    .AsNoTracking()
                    .ToList();
                return values;
            }
        }

        public IList<CultureInfo> SupportedCultures => _requestLocalizationSettings.SupportedCultures;

        public LocalizedString GetResource(string resourceKey, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentException(nameof(resourceKey));

            var item = _cache
                .SelectMany(r => r.Translations)
                .Where(t => t.Resource.ResourceKey == resourceKey
                            && t.Language == culture.Name)
                .Select(p => new
                {
                    p.Value
                })
                .SingleOrDefault();

            if (item == null && _localizerSettings.CreateMissingTranslationsIfNotFound)
                AddMissingResourceKeys(resourceKey);

            var result = item?.Value ?? string.Empty;

            if (string.IsNullOrWhiteSpace(result))
                switch (_localizerSettings.FallBackBehavior)
                {
                    case FallBackBehaviorEnum.KeyName:
                        result = resourceKey;
                        break;

                    case FallBackBehaviorEnum.DefaultCulture:
                        result = _cache
                            .SelectMany(r => r.Translations)
                            .Where(t => t.Resource.ResourceKey == resourceKey
                                        && t.Language == DefaultLanguage)
                            .Select(t => t.Value ?? string.Empty)
                            .SingleOrDefault();
                        break;
                }

            return new LocalizedString(resourceKey, result!);
        }

        public void Sync()
        {
            throw new NotImplementedException();
        }

        private void UpdateCache(string resourceKey)
        {
            lock (_cache)
            {
                using (var scope = _serviceProvider.GetScopedService(out T context))
                {
                    var values = context.Set<LocalizationResource>()
                        .Include(r => r.Translations)
                        .AsNoTracking()
                        .Where(r => r.ResourceKey == resourceKey)
                        .ToList();

                    _cache.Where(m => m.ResourceKey == resourceKey)
                        .ToList()
                        .ForEach(item => { _cache.Remove(item); });

                    if (values.Any())
                        _cache.AddRange(values);
                }
            }
        }

        private void AddMissingResourceKeys(string resourceKey)
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