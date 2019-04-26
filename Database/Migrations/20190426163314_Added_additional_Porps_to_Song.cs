using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class Added_additional_Porps_to_Song : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlbumName",
                table: "Song",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "Song",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumName",
                table: "Song");

            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "Song");
        }
    }
}
