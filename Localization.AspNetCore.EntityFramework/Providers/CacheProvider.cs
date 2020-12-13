using System;
using System.Globalization;
using System.Threading;
using Localization.AspNetCore.EntityFramework.Providers.Interfaces;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Localization.AspNetCore.EntityFramework.Providers
{
    internal class CacheProvider : ICacheProvider
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();
        private readonly IMemoryCache _innerCache;
        private readonly LocalizerOptions _settings;

        public CacheProvider(IMemoryCache cache,
            IOptions<LocalizerOptions> localizationOptions)
        {
            _settings = localizationOptions == null
                ? throw new ArgumentNullException(nameof(localizationOptions))
                : localizationOptions.Value;
            _innerCache = cache;
        }

        public void Set(string key, CultureInfo culture, string value)
        {
            key = GetCacheKey(key, culture);
            var options = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.Normal)
                .SetSlidingExpiration(_settings.CacheExpirationOffset);

            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _innerCache.Set(key, value, options);
        }

        public bool TryGetValue(string key, CultureInfo culture, out string value)
        {
            key = GetCacheKey(key, culture);
            return _innerCache.TryGetValue(key, out value);
        }

        public void Reset()
        {
            if (_resetCacheToken != null
                && !_resetCacheToken.IsCancellationRequested
                && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }

        private string GetCacheKey(string key, CultureInfo culture)
        {
            return $"{key}.{culture.Name}";
        }
    }
}