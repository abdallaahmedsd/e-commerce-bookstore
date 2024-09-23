using Bulky.Models.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulky.DataAccess.Data.Configurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
			builder.Property(x => x.Name)
				.HasMaxLength(100)
				.IsRequired();

            builder.Property(x => x.City)
				.HasMaxLength(50)
				.IsRequired(false);

            builder.Property(x => x.State)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(x => x.StreetAddress)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(x => x.PostalCode)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired(false);
        }
    }
}
