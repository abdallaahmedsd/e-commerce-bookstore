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
			builder.OwnsOne(x => x.AddressInfo, contact =>
			{
				contact.Property(x => x.City)
					.HasMaxLength(50)
					.IsRequired(false)
					.HasColumnName("City");

				contact.Property(x => x.State)
					.HasMaxLength(100)
					.IsRequired(false)
					.HasColumnName("State");

				contact.Property(x => x.StreetAddress)
					.HasMaxLength(150)
					.IsRequired(false)
					.HasColumnName("StreetAddress");

				contact.Property(x => x.PostalCode)
					.HasMaxLength(20)
					.IsRequired(false)
					.HasColumnName("PostalCode");
			});

			builder.ToTable("Companies");
		}
	}
}
