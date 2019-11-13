using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class Rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSignUpRequests");

            migrationBuilder.CreateTable(
                name: "UserConfirmEmailRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    IsActivated = table.Column<bool>(nullable: false),
                    ActivatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConfirmEmailRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConfirmEmailRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConfirmEmailRequests_UserId",
                table: "UserConfirmEmailRequests",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConfirmEmailRequests");

            migrationBuilder.CreateTable(
                name: "UserSignUpRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActivatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActivated = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSignUpRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSignUpRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSignUpRequests_UserId",
                table: "UserSignUpRequests",
                column: "UserId");
        }
    }
}
