using Bulky.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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

		// Seed data for the TbCompany and AddressInfo
		builder.HasData(
			new TbCompany { Id = 1, Name = "Tech Innovations" },
			new TbCompany { Id = 2, Name = "Global Solutions" },
			new TbCompany { Id = 3, Name = "Future Enterprises" }
		);

		// Seed data for AddressInfo as part of TbCompany (using the key)
		builder.OwnsOne(x => x.AddressInfo).HasData(
			new
			{
				TbCompanyId = 1,
				City = "Doha",
				State = "Qatar",
				StreetAddress = "123 Tech Street",
				PostalCode = "10001"
			},
			new
			{
				TbCompanyId = 2,
				City = "New York",
				State = "NY",
				StreetAddress = "456 Global Ave",
				PostalCode = "10002"
			},
			new
			{
				TbCompanyId = 3,
				City = "Berlin",
				State = "Berlin",
				StreetAddress = "789 Future Blvd",
				PostalCode = "10003"
			}
		);
	}
}
