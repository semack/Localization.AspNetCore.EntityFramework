using Localization.AspNetCore.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Localization.AspNetCore.EntityFramework.Customizers
{
    internal class LocalizationProviderModelCustomizer : RelationalModelCustomizer
    {
        public LocalizationProviderModelCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies)
        {
        }

        public override void Customize(ModelBuilder builder, DbContext context)
        {
            builder.Entity<LocalizationResource>(entity =>
            {
                entity.ToTable("LocalizationResources");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ResourceKey)
                    .HasMaxLength(1024)
                    .IsRequired();
                entity.HasIndex(p => p.ResourceKey);
            });

            builder.Entity<LocalizationResourceTranslation>(entity =>
            {
                entity.ToTable("LocalizationResourceTranslations");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Language)
                    .HasMaxLength(16)
                    .IsRequired();
                entity.Property(p => p.Value)
                    .IsRequired(false);
                entity.HasOne(p => p.Resource)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(p => p.ResourceId);
                entity.HasIndex(p => p.Language)
                    .IsUnique();
            });

            base.Customize(builder, context);
        }
    }
}