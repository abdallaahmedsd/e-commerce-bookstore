using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateBookHomeView : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[BookHome_View]
                AS
                SELECT 
                    Id, 
                    Title, 
                    Author, 
                    ListPrice, 
                    Price100, 
                    ImageUrl 
                FROM Books;
            ");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW [dbo].[BookHome_View];");
		}
	}
}
