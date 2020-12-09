using Localization.AspNetCore.EntityFramework.Enums;

namespace Localization.AspNetCore.EntityFramework.Settings
{
    public class LocalizerOptions
    {
        public NamingConventionEnum NamingConvention { get; set; } = NamingConventionEnum.Name;
        public bool ReturnOnlyKeyIfNotFound { get; set; } = true;
        public bool CreateNewRecordWhenLocalisedStringDoesNotExist { get; set; } = true;
    }
}