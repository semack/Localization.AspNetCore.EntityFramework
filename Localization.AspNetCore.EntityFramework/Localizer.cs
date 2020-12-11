using System;
using System.Collections.Generic;
using System.Globalization;
using Localization.AspNetCore.EntityFramework.Factories;
using Localization.AspNetCore.EntityFramework.Managers;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.EntityFramework
{
    public class Localizer : IStringLocalizer
    {
        private readonly ILocalizationManager _manager;
        private readonly string _sourceName;

        public Localizer(ILocalizationManager manager, string sourceName)
        {
            _manager = manager;
            _sourceName = sourceName;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public LocalizedString this[string name]
        {
            get
            {
                var resourceKey = $"{_sourceName}.{name}";
                return _manager.GetResource(resourceKey, CultureInfo.CurrentUICulture);
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