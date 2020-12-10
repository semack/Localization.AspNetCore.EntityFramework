using Localization.AspNetCore.EntityFramework.Enums;

namespace Localization.AspNetCore.EntityFramework.Settings
{
    public class LocalizerOptions
    {
        public NamingConventionEnum NamingConvention { get; set; } = NamingConventionEnum.Name;
        public bool ReturnKeyNameIfNoTranslation { get; set; } = true;
        public bool CreateMissingKeysIfNotFound { get; set; } = true;
    }
}