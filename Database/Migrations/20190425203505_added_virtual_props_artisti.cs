using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class added_virtual_props_artisti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArtistId",
                table: "Song",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtistId",
                table: "Song");
        }
    }
}
