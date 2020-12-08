using System;
using Localizer.AspNetCore.EntityFramework.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Localizer.AspNetCore.EntityFramework
{
    public class LocalizerFactory: IStringLocalizerFactory
    {
        private readonly LocalizerOptions _options;
        
        public LocalizerFactory(IOptions<LocalizerOptions> localizerOptions)
        {
            _options =   localizerOptions == null 
                ? throw new ArgumentNullException(nameof(localizerOptions)) 
                : localizerOptions.Value;
        }
        
        public IStringLocalizer Create(Type resourceSource)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            throw new NotImplementedException();
        }
    }
}