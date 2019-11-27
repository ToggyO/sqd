using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class ChangeInvite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_CreatorId",
                table: "Invites");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorId",
                table: "Invites",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_CreatorId",
                table: "Invites",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invites_Users_CreatorId",
                table: "Invites");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorId",
                table: "Invites",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Invites_Users_CreatorId",
                table: "Invites",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
