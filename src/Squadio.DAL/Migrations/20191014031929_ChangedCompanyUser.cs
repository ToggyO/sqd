using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class ChangedCompanyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompaniesAdministrators");

            migrationBuilder.CreateTable(
                name: "CompaniesUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompaniesUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompaniesUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompaniesUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersRegistrationStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    StepName = table.Column<string>(nullable: true),
                    Step = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRegistrationStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersRegistrationStep_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesUsers_CompanyId",
                table: "CompaniesUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesUsers_UserId",
                table: "CompaniesUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRegistrationStep_UserId",
                table: "UsersRegistrationStep",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompaniesUsers");

            migrationBuilder.DropTable(
                name: "UsersRegistrationStep");

            migrationBuilder.CreateTable(
                name: "CompaniesAdministrators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
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
    }
}
