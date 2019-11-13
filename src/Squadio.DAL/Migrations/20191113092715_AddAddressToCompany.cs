using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class AddAddressToCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Companies");
        }
    }
}
