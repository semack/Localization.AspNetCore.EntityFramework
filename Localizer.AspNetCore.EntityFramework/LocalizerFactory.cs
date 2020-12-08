using System;
using Localizer.AspNetCore.EntityFramework.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localizer.AspNetCore.EntityFramework
{
    internal class LocalizerFactory<T> : IStringLocalizerFactory
        where T : DbContext
    {
        private readonly LocalizerOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public LocalizerFactory(
            IOptions<LocalizerOptions> localizerOptions,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _options = localizerOptions == null
                ? throw new ArgumentNullException(nameof(localizerOptions))
                : localizerOptions.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var result = new Localizer<T>();
            return result;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            throw new NotImplementedException();
        }
    }
}