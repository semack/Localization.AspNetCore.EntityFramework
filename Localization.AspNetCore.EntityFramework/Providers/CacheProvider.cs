using System;
using System.Globalization;
using System.Threading;
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
        private readonly LocalizerOptions _localizerSettings;

        public CacheProvider(IMemoryCache cache, 
            IOptions<LocalizerOptions> localizerOptions)
        {
            _localizerSettings = localizerOptions == null
                ? throw new ArgumentNullException(nameof(localizerOptions))
                : localizerOptions.Value;
            _innerCache = cache;
        }

        private string GetCacheKey(string key, CultureInfo culture)
        {
            return $"{key}.{culture.Name}";
        }

        public void Set(string key, CultureInfo culture, string value)
        {
            key = GetCacheKey(key, culture);
            var options = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.Normal)
                .SetSlidingExpiration(_localizerSettings.CacheExpirationOffset);

            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));
            
            _innerCache.Set(key, value, options);
        }
        
        public string Get(string key, CultureInfo culture)
        {
            key = GetCacheKey(key, culture);
            return _innerCache.Get<string>(key);
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
    }
}