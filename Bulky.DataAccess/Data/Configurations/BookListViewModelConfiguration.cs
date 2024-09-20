using Bulky.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulky.DataAccess.Data.Configurations
{
    internal class BookListViewModelConfiguration : IEntityTypeConfiguration<BookListViewModel>
    {
        public void Configure(EntityTypeBuilder<BookListViewModel> builder)
        {
            builder.HasNoKey()
                .ToView("BookList_View");

            builder.Property(b => b.Price)
                   .HasColumnType("decimal(18,2)");
        }
    }

}
