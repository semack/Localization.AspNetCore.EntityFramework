using System;
using System.Runtime.Serialization;

namespace Localization.AspNetCore.EntityFramework.Entities
{
    public class LocalizationResourceTranslation
    {
        public long Id { get; set; }
        public string Language { get; set; }
        public long ResourceId { get; set; }
        public string Value { get; set; }
        public DateTime Modified { get; set; }

        [IgnoreDataMember] public LocalizationResource Resource { get; set; }
    }
}