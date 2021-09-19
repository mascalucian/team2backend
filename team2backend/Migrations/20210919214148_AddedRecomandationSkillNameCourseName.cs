using Microsoft.EntityFrameworkCore.Migrations;

namespace team2backend.Migrations
{
    public partial class AddedRecomandationSkillNameCourseName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseTitle",
                table: "Recomandations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkillName",
                table: "Recomandations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseTitle",
                table: "Recomandations");

            migrationBuilder.DropColumn(
                name: "SkillName",
                table: "Recomandations");
        }
    }
}
