using System.Globalization;

namespace Localization.AspNetCore.EntityFramework.Providers
{
    internal interface ICacheProvider
    {
        bool TryGetValue(string key, CultureInfo culture, out string value);
        string Get(string key, CultureInfo culture);
        void Set(string key, CultureInfo culture, string value);
        void Reset();
    }
}