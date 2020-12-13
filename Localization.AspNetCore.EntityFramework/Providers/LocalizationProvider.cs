using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Localization.AspNetCore.EntityFramework.Entities;
using Localization.AspNetCore.EntityFramework.Enums;
using Localization.AspNetCore.EntityFramework.Extensions;
using Localization.AspNetCore.EntityFramework.Providers.Interfaces;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localization.AspNetCore.EntityFramework.Providers
{
    internal class LocalizationProvider<T> : ILocalizationProvider
        where T : DbContext
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly LocalizerOptions _settings;
        private readonly RequestLocalizationOptions _requestLocalizationSettings;
        private readonly IServiceProvider _serviceProvider;

        public LocalizationProvider(IServiceProvider serviceProvider,
            ICacheProvider cacheProvider,
            IOptions<LocalizerOptions> localizationOptions,
            IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _serviceProvider = serviceProvider;
            _cacheProvider = cacheProvider;
            _settings = localizationOptions == null
                ? throw new ArgumentNullException(nameof(localizationOptions))
                : localizationOptions.Value;
            _requestLocalizationSettings = requestLocalizationOptions == null
                ? throw new ArgumentNullException(nameof(requestLocalizationOptions))
                : requestLocalizationOptions.Value;
            _cacheProvider.Reset();
        }

        private CultureInfo DefaultCulture => _requestLocalizationSettings.DefaultRequestCulture.UICulture;

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

        public LocalizedString GetResource(string resourceKey, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentException(nameof(resourceKey));

            if (!_cacheProvider.TryGetValue(resourceKey, culture, out var value))
            {
                using (var scope = _serviceProvider.GetScopedService(out T context))
                {
                    var item = context
                        .Set<LocalizationResource>()
                        .SelectMany(r => r.Translations)
                        .Where(t => t.Resource.ResourceKey == resourceKey
                                    && t.Language == culture.Name)
                        .Select(p => new
                        {
                            p.Value
                        })
                        .SingleOrDefault();

                    if (item == null)
                        if (_settings.CreateMissingTranslationsIfNotFound)
                            AddMissingResourceKeys(resourceKey);

                    value = item?.Value ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(value))
                        switch (_settings.FallBackBehavior)
                        {
                            case FallBackBehaviorEnum.KeyName:
                                value = resourceKey;
                                break;

                            case FallBackBehaviorEnum.DefaultCulture:
                                if (culture.Name != DefaultCulture.Name)
                                    return GetResource(resourceKey, DefaultCulture);
                                break;
                        }
                }

                _cacheProvider.Set(resourceKey, culture, value);
            }

            return new LocalizedString(resourceKey, value!);
        }

        public void Sync()
        {
            using (var scope = _serviceProvider.GetScopedService(out T context))
            {
                Parallel.ForEach(context.Set<LocalizationResource>()
                        .Include(r => r.Translations),
                    resource =>
                    {
                        _requestLocalizationSettings.SupportedCultures.ToList()
                            .ForEach(culture =>
                            {
                                if (resource.Translations.All(t => t.Language != culture.Name))
                                {
                                    resource.Modified = DateTime.UtcNow;
                                    resource.Translations.Add(new LocalizationResourceTranslation
                                    {
                                        Language = culture.Name,
                                        Modified = DateTime.UtcNow
                                    });
                                }
                            });
                    }
                );
                context.SaveChanges();
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

                _requestLocalizationSettings.SupportedCultures.ToList()
                    .ForEach(culture =>
                    {
                        if (resource.Translations.All(t => t.Language != culture.Name))
                        {
                            resource.Modified = DateTime.UtcNow;
                            resource.Translations.Add(new LocalizationResourceTranslation
                            {
                                Language = culture.Name,
                                Modified = DateTime.UtcNow
                            });
                        }
                    });

                context.SaveChanges();
            }
        }
    }
}