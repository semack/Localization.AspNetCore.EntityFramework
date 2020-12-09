namespace Localization.AspNetCore.EntityFramework.Models
{
    public class LocalizationModel
    {
        public long Id { get; set; }
        public long ResourceId { get; set; }
        public string ResourceKey { get; set; }
        public string Language { get; set; }
        public string Value { get; set; }
    }
}