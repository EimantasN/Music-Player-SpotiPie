using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class Added_Album_Prop_Tracks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Tracks",
                table: "Albums",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tracks",
                table: "Albums");
        }
    }
}
