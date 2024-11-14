using Bulky.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Data.Configurations
{
    internal class BookImageConfiguration : IEntityTypeConfiguration<TbBookImage>
    {
        public void Configure(EntityTypeBuilder<TbBookImage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ImageUrl)
                .IsRequired();

            builder.HasOne(x => x.Book)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.BookId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("BookImages");
        }
    }
}
