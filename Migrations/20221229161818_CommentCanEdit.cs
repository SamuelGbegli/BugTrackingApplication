using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTrackingApplication.Migrations
{
    public partial class CommentCanEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEdit",
                table: "Comment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanEdit",
                table: "Comment");
        }
    }
}
