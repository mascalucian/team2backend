using Microsoft.EntityFrameworkCore.Migrations;

namespace team2backend.Migrations
{
    public partial class AddAuthMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recomandations_User_UserId",
                table: "Recomandations");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Recomandations_UserId",
                table: "Recomandations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Recomandations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Recomandations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recomandations_UserId",
                table: "Recomandations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recomandations_User_UserId",
                table: "Recomandations",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
