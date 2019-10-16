using Microsoft.EntityFrameworkCore.Migrations;

namespace Squadio.DAL.Migrations
{
    public partial class AddFluentAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersRegistrationStep_UserId",
                table: "UsersRegistrationStep");

            migrationBuilder.DropIndex(
                name: "IX_TeamsUsers_TeamId",
                table: "TeamsUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectsUsers_ProjectId",
                table: "ProjectsUsers");

            migrationBuilder.DropIndex(
                name: "IX_CompaniesUsers_CompanyId",
                table: "CompaniesUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teams",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersRegistrationStep_UserId",
                table: "UsersRegistrationStep",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamsUsers_TeamId_UserId",
                table: "TeamsUsers",
                columns: new[] { "TeamId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUsers_ProjectId_UserId",
                table: "ProjectsUsers",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesUsers_CompanyId_UserId",
                table: "CompaniesUsers",
                columns: new[] { "CompanyId", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsersRegistrationStep_UserId",
                table: "UsersRegistrationStep");

            migrationBuilder.DropIndex(
                name: "IX_TeamsUsers_TeamId_UserId",
                table: "TeamsUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectsUsers_ProjectId_UserId",
                table: "ProjectsUsers");

            migrationBuilder.DropIndex(
                name: "IX_CompaniesUsers_CompanyId_UserId",
                table: "CompaniesUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Teams",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_UsersRegistrationStep_UserId",
                table: "UsersRegistrationStep",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamsUsers_TeamId",
                table: "TeamsUsers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectsUsers_ProjectId",
                table: "ProjectsUsers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CompaniesUsers_CompanyId",
                table: "CompaniesUsers",
                column: "CompanyId");
        }
    }
}
