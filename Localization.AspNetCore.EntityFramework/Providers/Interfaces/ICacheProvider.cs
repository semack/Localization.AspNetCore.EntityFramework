using System.Globalization;

namespace Localization.AspNetCore.EntityFramework.Providers.Interfaces
{
    internal interface ICacheProvider
    {
        bool TryGetValue(string key, CultureInfo culture, out string value);
        void Set(string key, CultureInfo culture, string value);
        void Reset();
    }
}