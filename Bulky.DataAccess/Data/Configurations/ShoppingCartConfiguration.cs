using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulky.DataAccess.Data.Configurations
{
	internal class ShoppingCartConfiguration : IEntityTypeConfiguration<TbShoppingCart>
	{
		public void Configure(EntityTypeBuilder<TbShoppingCart> builder)
		{
			builder.HasKey(x => x.Id);

			builder.HasOne(x => x.Book)
				.WithMany()
				.HasForeignKey(x => x.BookId)
				.IsRequired();

			builder.HasOne(x => x.User)
				.WithMany()
				.HasForeignKey(x => x.UserId)
				.IsRequired();

			builder.ToTable("ShoppingCarts");
		}
	}
}
