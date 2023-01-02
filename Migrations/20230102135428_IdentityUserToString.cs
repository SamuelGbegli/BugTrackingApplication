using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingApplication.Migrations
{
    public partial class IdentityUserToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bug_AspNetUsers_UserId",
                table: "Bug");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_UserId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_UserId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Bug_UserId",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bug");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Bug",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Bug");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Bug",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_UserId",
                table: "Project",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bug_UserId",
                table: "Bug",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bug_AspNetUsers_UserId",
                table: "Bug",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_UserId",
                table: "Project",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
