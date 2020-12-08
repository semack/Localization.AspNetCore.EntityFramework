using Microsoft.AspNetCore.Builder;

namespace Localizer.AspNetCore.EntityFramework.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLocalizer(this IApplicationBuilder app)
        {
            return app;
        }
    }
}