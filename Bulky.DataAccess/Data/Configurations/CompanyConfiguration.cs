using Bulky.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace Bulky.DataAccess.Data.Configurations
{
	internal class CompanyConfiguration : IEntityTypeConfiguration<TbCompany>
	{
		public void Configure(EntityTypeBuilder<TbCompany> builder)
		{
			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				.HasMaxLength(150)
				.IsRequired();

			// Configuring AddressInfo as an owned entity
			builder.OwnsOne(x => x.AddressInfo, address =>
			{
				address.Property(a => a.City)
					.HasMaxLength(50)
					.IsRequired(false);

				address.Property(a => a.State)
					.HasMaxLength(100)
					.IsRequired(false);

				address.Property(a => a.StreetAddress)
					.HasMaxLength(150)
					.IsRequired(false);

				address.Property(a => a.PostalCode)
					.HasMaxLength(20)
					.IsRequired(false);
			});

			builder.ToTable("Companies");
		}
	}
}
