using System.Collections.Generic;
using Microsoft.Extensions.Localization;

namespace Localizer.AspNetCore.EntityFramework
{
    public class Localizer: IStringLocalizer
    {
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new System.NotImplementedException();
        }

        public LocalizedString this[string name] => throw new System.NotImplementedException();

        public LocalizedString this[string name, params object[] arguments] => throw new System.NotImplementedException();
    }
}