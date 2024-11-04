using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserListView : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                CREATE VIEW UserList_View
                AS
                SELECT 
                    AspNetUsers.Name, 
                    AspNetUsers.Email, 
                    AspNetUsers.PhoneNumber AS Phone, 
                    Companies.Name AS Company, 
                    AspNetRoles.Name AS Role
                FROM 
                    AspNetUserRoles
                INNER JOIN 
                    AspNetUsers ON AspNetUserRoles.UserId = AspNetUsers.Id
                LEFT JOIN 
                    AspNetRoles ON AspNetUserRoles.RoleId = AspNetRoles.Id
                LEFT JOIN 
                    Companies ON AspNetUsers.CompanyId = Companies.Id;
            ");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP VIEW UserList_View;");
		}
	}
}
