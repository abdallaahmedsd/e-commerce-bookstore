using Bulky.Models.ViewModels.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulky.DataAccess.Data.Configurations
{
    internal class BookHomeViewModelConfiguration : IEntityTypeConfiguration<BookHomeViewModel>
	{
		public void Configure(EntityTypeBuilder<BookHomeViewModel> builder)
		{
			builder.HasNoKey()
				.ToView("BookHome_View");

            builder.Property(b => b.ListPrice)
				.HasColumnType("decimal(18,2)");

            builder.Property(b => b.Price100)
				.HasColumnType("decimal(18,2)");

			builder.Property(b => b.Price100)
				.HasColumnType("decimal(18,2)");
		}
	}
}
