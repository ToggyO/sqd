using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class AddCompaniesAdmins : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_AdministratorId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_AdministratorId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdministratorId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompaniesAdministrators",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompaniesAdministrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompaniesAdministrators_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompaniesAdministrators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesAdministrators_CompanyId",
                table: "CompaniesAdministrators",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesAdministrators_UserId",
                table: "CompaniesAdministrators",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompaniesAdministrators");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdministratorId",
                table: "Companies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AdministratorId",
                table: "Companies",
                column: "AdministratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_AdministratorId",
                table: "Companies",
                column: "AdministratorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
