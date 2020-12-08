using System;

namespace Localizer.AspNetCore.EntityFramework.Entities
{
    internal class LocalizationResourceTranslationEntity
    {
        public long Id { get; set; }
        public string Language { get; set; }
        public long ResourceId { get; set; }
        public string Value { get; set; }
        public DateTime ModificationDate { get; set; }
        public LocalizationResourceEntity Resource { get; set; }
    }
}