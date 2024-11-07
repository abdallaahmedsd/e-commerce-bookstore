using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterUserListView : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                ALTER VIEW [dbo].[UserList_View]
                AS
                SELECT
					u.Id,
                    u.Name, 
                    u.Email, 
                    u.PhoneNumber AS Phone, 
                    c.Name AS Company, 
                    r.Name AS Role,
					CASE  
						WHEN u.LockoutEnd > SYSDATETIMEOFFSET() THEN CAST(1 AS BIT)
						else CAST(0 AS BIT)
					END AS IsLocked
                FROM 
                    AspNetUserRoles ur
                INNER JOIN 
                    AspNetUsers u ON ur.UserId = u.Id
                LEFT JOIN 
                    AspNetRoles r ON ur.RoleId = r.Id
                LEFT JOIN 
                    Companies c ON u.CompanyId = c.Id;
            ");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
                ALTER VIEW UserList_View
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
	}
}
