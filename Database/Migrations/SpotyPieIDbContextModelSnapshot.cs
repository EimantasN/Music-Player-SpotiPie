﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Migrations
{
    [DbContext(typeof(SpotyPieIDbContext))]
    partial class SpotyPieIDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Models.BackEnd.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArtistId");

                    b.Property<bool>("IsPlayable");

                    b.Property<string>("LargeImage");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<string>("MediumImage");

                    b.Property<string>("Name");

                    b.Property<int>("Popularity");

                    b.Property<string>("ReleaseDate");

                    b.Property<string>("SmallImage");

                    b.Property<string>("SpotifyId");

                    b.Property<int>("Tracks");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("Models.BackEnd.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Genres");

                    b.Property<string>("LargeImage");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<string>("MediumImage");

                    b.Property<string>("Name");

                    b.Property<long>("Popularity");

                    b.Property<string>("SmallImage");

                    b.Property<string>("SpotifyId");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Models.BackEnd.CurrentSong", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlbumId");

                    b.Property<int>("ArtistId");

                    b.Property<int>("CurrentMs");

                    b.Property<long>("DurationMs");

                    b.Property<string>("Image");

                    b.Property<string>("LocalUrl");

                    b.Property<string>("Name");

                    b.Property<int>("PlaylistId");

                    b.Property<int>("SongId");

                    b.HasKey("Id");

                    b.ToTable("CurrentSong");
                });

            modelBuilder.Entity("Models.BackEnd.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Height");

                    b.Property<string>("Url");

                    b.Property<long>("Width");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Models.BackEnd.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Artists");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Gendres");

                    b.Property<string>("Image");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<long>("Limit");

                    b.Property<string>("Name");

                    b.Property<long>("Popularity");

                    b.Property<long>("Total");

                    b.HasKey("Id");

                    b.ToTable("Playlist");
                });

            modelBuilder.Entity("Models.BackEnd.Quote", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ArtistId");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("ArtistId");

                    b.ToTable("Quotes");
                });

            modelBuilder.Entity("Models.BackEnd.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AlbumId");

                    b.Property<int>("ArtistId");

                    b.Property<bool>("Corrupted");

                    b.Property<long>("DiscNumber");

                    b.Property<long>("DurationMs");

                    b.Property<bool>("Explicit");

                    b.Property<bool>("IsLocal");

                    b.Property<bool>("IsPlayable");

                    b.Property<string>("LargeImage");

                    b.Property<DateTime>("LastActiveTime");

                    b.Property<string>("LocalUrl");

                    b.Property<string>("MediumImage");

                    b.Property<string>("Name");

                    b.Property<int?>("PlaylistId");

                    b.Property<int>("Popularity");

                    b.Property<long>("Size");

                    b.Property<string>("SmallImage");

                    b.Property<string>("SpotifyId");

                    b.Property<long>("TrackNumber");

                    b.Property<DateTime>("UploadTime");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("PlaylistId");

                    b.ToTable("Song");
                });

            modelBuilder.Entity("Models.BackEnd.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("Birthdate");

                    b.Property<string>("Country");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email");

                    b.Property<string>("Images");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Models.BackEnd.Album", b =>
                {
                    b.HasOne("Models.BackEnd.Artist")
                        .WithMany("Albums")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Models.BackEnd.Quote", b =>
                {
                    b.HasOne("Models.BackEnd.Artist")
                        .WithMany("Quotes")
                        .HasForeignKey("ArtistId");
                });

            modelBuilder.Entity("Models.BackEnd.Song", b =>
                {
                    b.HasOne("Models.BackEnd.Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Models.BackEnd.Playlist")
                        .WithMany("Songs")
                        .HasForeignKey("PlaylistId");
                });
#pragma warning restore 612, 618
        }
    }
}
