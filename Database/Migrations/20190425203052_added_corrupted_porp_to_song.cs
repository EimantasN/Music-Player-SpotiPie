using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class added_corrupted_porp_to_song : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Corrupted",
                table: "Song",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Corrupted",
                table: "Song");
        }
    }
}
