using System.Collections.Generic;
using System.Globalization;
using Localization.AspNetCore.EntityFramework.Entities;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.EntityFramework.Providers.Interfaces
{
    public interface ILocalizationProvider
    {
        void Import(IList<LocalizationResource> source);
        IList<LocalizationResource> Export();
        LocalizedString GetResource(string name, CultureInfo culture);
        void Sync();
    }
}