using Localization.AspNetCore.EntityFramework.Customizers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Localization.AspNetCore.EntityFramework.Extensions
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseLocalizationEntities(this DbContextOptionsBuilder builder)
        {
            builder.ReplaceService<IModelCustomizer, LocalizationProviderModelCustomizer>();
            return builder;
        }
    }
}