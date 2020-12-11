using Localization.AspNetCore.EntityFramework.Enums;

namespace Localization.AspNetCore.EntityFramework.Settings
{
    public class LocalizerOptions
    {
        public NamingConventionEnum NamingConvention { get; set; } = NamingConventionEnum.TypeName;
        public bool ReturnKeyNameIfNoTranslation { get; set; } = true;
        public bool CreateMissingTranslationsIfNotFound { get; set; } = true;
    }
}