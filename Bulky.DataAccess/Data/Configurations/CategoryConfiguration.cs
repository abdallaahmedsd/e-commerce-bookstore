using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulky.DataAccess.Data.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<TbCategory>
    {
        public void Configure(EntityTypeBuilder<TbCategory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.ToTable("Categories");

            builder.HasData(SeedCategory());
        }

        private static List<TbCategory> SeedCategory()
        {
            return
            [
                new TbCategory { Id = 1, Name = "Action", DisplayOrder = 1},
                new TbCategory { Id = 2, Name = "SciFi", DisplayOrder = 2},
                new TbCategory { Id = 3, Name = "History", DisplayOrder = 3}
            ];
        }
    }
}
