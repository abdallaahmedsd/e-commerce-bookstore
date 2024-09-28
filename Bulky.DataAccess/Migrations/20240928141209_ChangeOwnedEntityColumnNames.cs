using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOwnedEntityColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressInfo_StreetAddress",
                table: "Companies",
                newName: "StreetAddress");

            migrationBuilder.RenameColumn(
                name: "AddressInfo_State",
                table: "Companies",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "AddressInfo_PostalCode",
                table: "Companies",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "AddressInfo_City",
                table: "Companies",
                newName: "City");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "Companies",
                newName: "AddressInfo_StreetAddress");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Companies",
                newName: "AddressInfo_State");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Companies",
                newName: "AddressInfo_PostalCode");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Companies",
                newName: "AddressInfo_City");
        }
    }
}
