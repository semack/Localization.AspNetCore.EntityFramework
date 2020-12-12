using System;
using System.Threading;
using Localization.AspNetCore.EntityFramework.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Localization.AspNetCore.EntityFramework.Providers
{
    internal class CacheProvider 
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

        public T Set<T>(object key, T value) 
        {
            /* some other code removed for brevity */
            var options = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.Normal)
                .SetSlidingExpiration(_localizerSettings.CacheExpirationOffset);
                //.SetAbsoluteExpiration(typeExpiration);
            
            options.AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _innerCache.Set(key, value, options);

            return value;
        }
        

        public void Reset()
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }
    }
}