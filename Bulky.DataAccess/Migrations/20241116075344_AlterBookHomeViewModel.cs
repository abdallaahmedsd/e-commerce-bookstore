using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterBookHomeViewModel : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[BookHome_View]
                AS
                SELECT 
                    b.Id, 
                    b.Title, 
                    b.Author, 
                    b.ListPrice, 
                    b.Price100,
                    bi.ImageUrl AS MainImageUrl
                FROM 
                    Books b
                LEFT JOIN 
                    BookImages bi 
                ON 
                    b.Id = bi.BookId AND bi.IsMainImage = 1;
            ");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[BookHome_View]
                AS
                SELECT 
                    Id, 
                    Title, 
                    Author, 
                    ListPrice, 
                    Price100
                FROM Books;
            ");
		}
	}
}
