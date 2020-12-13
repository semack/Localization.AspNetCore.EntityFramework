using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Localization.AspNetCore.EntityFramework.Providers.Interfaces;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.EntityFramework
{
    public class Localizer : IStringLocalizer
    {
        private readonly ILocalizationProvider _provider;
        private readonly string _sourceName;

        public Localizer(ILocalizationProvider provider, string sourceName)
        {
            _provider = provider;
            _sourceName = sourceName;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return Enumerable.Empty<LocalizedString>();
        }

        public LocalizedString this[string name]
        {
            get
            {
                var resourceKey = $"{_sourceName}.{name}";
                return _provider.GetResource(resourceKey, CultureInfo.CurrentUICulture);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var result = string.Format(this[name].Value, arguments);
                return new LocalizedString(name, result);
            }
        }
    }
}