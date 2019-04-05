using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpotifyId = table.Column<string>(nullable: true),
                    Genres = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Popularity = table.Column<long>(nullable: false),
                    LargeImage = table.Column<string>(nullable: true),
                    MediumImage = table.Column<string>(nullable: true),
                    SmallImage = table.Column<string>(nullable: true),
                    LastActiveTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentSong",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArtistId = table.Column<int>(nullable: false),
                    PlaylistId = table.Column<int>(nullable: false),
                    AlbumId = table.Column<int>(nullable: false),
                    SongId = table.Column<int>(nullable: false),
                    DurationMs = table.Column<long>(nullable: false),
                    CurrentMs = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LocalUrl = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentSong", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Url = table.Column<string>(nullable: true),
                    Height = table.Column<long>(nullable: false),
                    Width = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Artists = table.Column<string>(nullable: true),
                    Gendres = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    LastActiveTime = table.Column<DateTime>(nullable: false),
                    Limit = table.Column<long>(nullable: false),
                    Total = table.Column<long>(nullable: false),
                    Popularity = table.Column<long>(nullable: false),
                    Image = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Birthdate = table.Column<DateTimeOffset>(nullable: false),
                    Country = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Images = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpotifyId = table.Column<string>(nullable: true),
                    LargeImage = table.Column<string>(nullable: true),
                    MediumImage = table.Column<string>(nullable: true),
                    SmallImage = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<string>(nullable: true),
                    Popularity = table.Column<int>(nullable: false),
                    IsPlayable = table.Column<bool>(nullable: false),
                    LastActiveTime = table.Column<DateTime>(nullable: false),
                    ArtistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Albums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Albums_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Song",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpotifyId = table.Column<string>(nullable: true),
                    DiscNumber = table.Column<long>(nullable: false),
                    DurationMs = table.Column<long>(nullable: false),
                    Explicit = table.Column<bool>(nullable: false),
                    IsLocal = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TrackNumber = table.Column<long>(nullable: false),
                    LargeImage = table.Column<string>(nullable: true),
                    MediumImage = table.Column<string>(nullable: true),
                    SmallImage = table.Column<string>(nullable: true),
                    LocalUrl = table.Column<string>(nullable: true),
                    Popularity = table.Column<int>(nullable: false),
                    IsPlayable = table.Column<bool>(nullable: false),
                    LastActiveTime = table.Column<DateTime>(nullable: false),
                    AlbumId = table.Column<int>(nullable: true),
                    PlaylistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Song", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Song_Albums_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "Albums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Song_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Albums_ArtistId",
                table: "Albums",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_Song_AlbumId",
                table: "Song",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Song_PlaylistId",
                table: "Song",
                column: "PlaylistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentSong");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Song");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Albums");

            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropTable(
                name: "Artists");
        }
    }
}
