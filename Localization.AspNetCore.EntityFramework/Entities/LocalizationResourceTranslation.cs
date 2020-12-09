using System;

namespace Localization.AspNetCore.EntityFramework.Entities
{
    public class LocalizationResourceTranslation
    {
        public long Id { get; set; }
        public string Language { get; set; }
        public long ResourceId { get; set; }
        public string Value { get; set; }
        public DateTime ModificationDate { get; set; }
        public LocalizationResource Resource { get; set; }
    }
}