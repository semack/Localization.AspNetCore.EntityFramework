using System;
using Localizer.AspNetCore.EntityFramework.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Localizer.AspNetCore.EntityFramework.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalizer<T>(this IServiceCollection services,
            Action<LocalizerOptions> setup = null)
            where T : DbContext
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            services.AddOptions();
            
            services.TryAdd(new ServiceDescriptor(
                typeof(IStringLocalizerFactory),
                typeof(LocalizerFactory),
                ServiceLifetime.Singleton));
            
            if (setup != null)
                services.Configure(setup);
            
            return services;
        }
    }
}