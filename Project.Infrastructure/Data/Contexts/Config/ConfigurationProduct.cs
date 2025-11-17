using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities.Business;

namespace Project.Infrastructure.Data.Contexts.Config
{
    internal class ConfigurationProduct : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.HasOne(p => p.Category)
                .WithMany(pc => pc.Products)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
