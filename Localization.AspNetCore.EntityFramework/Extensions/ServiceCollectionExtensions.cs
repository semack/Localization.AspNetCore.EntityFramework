using System;
using Localization.AspNetCore.EntityFramework.Factories;
using Localization.AspNetCore.EntityFramework.Managers;
using Localization.AspNetCore.EntityFramework.Providers;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.EntityFramework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalization<T>(this IServiceCollection services,
            Action<LocalizerOptions> setup = null)
            where T : DbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddOptions();

            services.AddMemoryCache();

            services.TryAdd(new ServiceDescriptor(
                typeof(ICacheProvider),
                typeof(CacheProvider),
                ServiceLifetime.Singleton));
            
            services.TryAdd(new ServiceDescriptor(
                typeof(LocalizationManager<>),
                typeof(LocalizationManager<>),
                ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(
                typeof(LocalizerFactory<>),
                typeof(LocalizerFactory<>),
                ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(
                typeof(ILocalizationManager),
                provider=> provider.GetRequiredService<LocalizationManager<T>>(),
                ServiceLifetime.Singleton));

            services.TryAdd(new ServiceDescriptor(
                typeof(IStringLocalizerFactory),
                provider=> provider.GetRequiredService<LocalizerFactory<T>>(),
                ServiceLifetime.Singleton));

            if (setup != null)
                services.Configure(setup);

            return services;
        }
    }
}