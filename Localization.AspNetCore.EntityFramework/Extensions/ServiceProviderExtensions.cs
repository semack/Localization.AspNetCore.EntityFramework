using System;
using Microsoft.Extensions.DependencyInjection;

namespace Localization.AspNetCore.EntityFramework.Extensions
{
    internal static class ServiceProviderExtensions
    {
        public static IServiceScope GetScopedService<T>(this IServiceProvider provider, out T context)
        {
            var scope = provider.CreateScope();
            context = scope.ServiceProvider.GetRequiredService<T>();
            return scope;
        }
    }
}