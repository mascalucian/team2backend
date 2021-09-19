using Microsoft.EntityFrameworkCore.Migrations;

namespace team2backend.Migrations
{
    public partial class AddedRecommendationUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Recomandations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Recomandations");
        }
    }
}
