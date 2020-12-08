using Localizer.AspNetCore.EntityFramework.Customizers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Localizer.AspNetCore.EntityFramework.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddLocalizerEntities(this DbContextOptionsBuilder builder)
        {
            builder.ReplaceService<IModelCustomizer, LocalizationProviderModelCustomizer>();
            return builder;
        }
    }
}