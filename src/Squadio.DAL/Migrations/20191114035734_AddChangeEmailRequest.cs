using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class AddChangeEmailRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPasswordRequests");

            migrationBuilder.CreateTable(
                name: "UserChangeEmailRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    NewEmail = table.Column<string>(nullable: true),
                    IsActivated = table.Column<bool>(nullable: false),
                    ActivatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChangeEmailRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChangeEmailRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRestorePasswordRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsActivated = table.Column<bool>(nullable: false),
                    ActivatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRestorePasswordRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRestorePasswordRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChangeEmailRequests_UserId",
                table: "UserChangeEmailRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRestorePasswordRequests_UserId",
                table: "UserRestorePasswordRequests",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChangeEmailRequests");

            migrationBuilder.DropTable(
                name: "UserRestorePasswordRequests");

            migrationBuilder.CreateTable(
                name: "UserPasswordRequests",
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
                    table.PrimaryKey("PK_UserPasswordRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPasswordRequests_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordRequests_UserId",
                table: "UserPasswordRequests",
                column: "UserId");
        }
    }
}
