using System.Collections.Generic;
using System.Globalization;
using Localization.AspNetCore.EntityFramework.Entities;
using Microsoft.Extensions.Localization;

namespace Localization.AspNetCore.EntityFramework.Managers.Abstract
{
    public interface ILocalizationManager
    {
        IList<CultureInfo> SupportedCultures { get; }
        void ResetCache();
        void Import(IList<LocalizationResource> source);
        IList<LocalizationResource> Export();
        LocalizedString GetResource(string name);
        void Sync();
    }
}