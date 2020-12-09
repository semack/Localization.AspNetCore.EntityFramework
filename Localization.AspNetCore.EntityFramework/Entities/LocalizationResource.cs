using System;
using System.Collections.Generic;

namespace Localization.AspNetCore.EntityFramework.Entities
{
    public class LocalizationResource
    {
        public long Id { get; set; }
        public string Author { get; set; }
        public bool FromCode { get; set; }
        public bool IsHidden { get; set; }
        public bool IsModified { get; set; }
        public DateTime ModificationDate { get; set; }
        public string ResourceKey { get; set; }
        public string Notes { get; set; }
        public ICollection<LocalizationResourceTranslation> Translations { get; set; }
    }
}