using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations
{
    public partial class ModelsUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveTime",
                table: "Artists");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Playlist",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "CurrentSong",
                newName: "Image");

            migrationBuilder.AddColumn<string>(
                name: "Artists",
                table: "Playlist",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gendres",
                table: "Playlist",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genres",
                table: "Item",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Item",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Artists",
                table: "Playlist");

            migrationBuilder.DropColumn(
                name: "Gendres",
                table: "Playlist");

            migrationBuilder.DropColumn(
                name: "Genres",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Item");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Playlist",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "CurrentSong",
                newName: "ImageUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveTime",
                table: "Artists",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
