using System;
using System.Collections.Generic;

namespace Localization.AspNetCore.EntityFramework.Entities
{
    public class LocalizationResource
    {
        public long Id { get; set; }
        public DateTime Modified { get; set; }
        public string ResourceKey { get; set; }
        public ICollection<LocalizationResourceTranslation> Translations { get; set; }
    }
}