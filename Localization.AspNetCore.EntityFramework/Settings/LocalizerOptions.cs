using System;
using Localization.AspNetCore.EntityFramework.Enums;

namespace Localization.AspNetCore.EntityFramework.Settings
{
    public class LocalizerOptions
    {
        public FallBackBehaviorEnum FallBackBehavior { get; set; } = FallBackBehaviorEnum.DefaultCulture;
        public NamingConventionEnum NamingConvention { get; set; } = NamingConventionEnum.TypeName;
        public bool CreateMissingTranslationsIfNotFound { get; set; } = true;
        public TimeSpan CacheExpirationOffset { get; set; } = new TimeSpan(1, 0, 0);
    }
}