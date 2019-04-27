using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class song_playlist_image_update_and_added_gendre_class : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artists",
                table: "Playlist");

            migrationBuilder.DropColumn(
                name: "Gendres",
                table: "Playlist");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Playlist");

            migrationBuilder.AlterColumn<int>(
                name: "Corrupted",
                table: "Song",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Song",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Base64",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocalUrl",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlaylistId",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "Images",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlaylistId",
                table: "Artists",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Gendres",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    PlaylistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gendres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gendres_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_PlaylistId",
                table: "Images",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_SongId",
                table: "Images",
                column: "SongId");

            migrationBuilder.CreateIndex(
                name: "IX_Artists_PlaylistId",
                table: "Artists",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_Gendres_PlaylistId",
                table: "Gendres",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_Playlist_PlaylistId",
                table: "Artists",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Playlist_PlaylistId",
                table: "Images",
                column: "PlaylistId",
                principalTable: "Playlist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Song_SongId",
                table: "Images",
                column: "SongId",
                principalTable: "Song",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_Playlist_PlaylistId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Playlist_PlaylistId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Song_SongId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "Gendres");

            migrationBuilder.DropIndex(
                name: "IX_Images_PlaylistId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_SongId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Artists_PlaylistId",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Song");

            migrationBuilder.DropColumn(
                name: "Base64",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "LocalUrl",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Artists");

            migrationBuilder.AlterColumn<bool>(
                name: "Corrupted",
                table: "Song",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Artists",
                table: "Playlist",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gendres",
                table: "Playlist",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Playlist",
                nullable: true);
        }
    }
}
