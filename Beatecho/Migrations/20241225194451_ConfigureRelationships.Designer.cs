﻿// <auto-generated />
using System;
using Beatecho.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Beatecho.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20241225194451_ConfigureRelationships")]
    partial class ConfigureRelationships
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Beatecho.DAL.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ReleaseYear")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.AlbumSongs", b =>
                {
                    b.Property<int>("SongId")
                        .HasColumnType("integer");

                    b.Property<int>("AlbumId")
                        .HasColumnType("integer");

                    b.Property<int>("TrackNum")
                        .HasColumnType("integer");

                    b.HasKey("SongId", "AlbumId");

                    b.HasIndex("AlbumId");

                    b.ToTable("AlbumSongs");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Artist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("bio")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.ArtistAlbums", b =>
                {
                    b.Property<int>("ArtistId")
                        .HasColumnType("integer");

                    b.Property<int>("AlbumId")
                        .HasColumnType("integer");

                    b.HasKey("ArtistId", "AlbumId");

                    b.HasIndex("AlbumId");

                    b.ToTable("ArtistAlbums");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<TimeSpan>("CreatedAt")
                        .HasColumnType("interval");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Songs");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.SongGenre", b =>
                {
                    b.Property<int>("SongId")
                        .HasColumnType("integer");

                    b.Property<int>("GenreId")
                        .HasColumnType("integer");

                    b.HasKey("SongId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("SongGenres");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Salt")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.Property<int?>("UserTypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserTypeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.UserType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserTypes");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.AlbumSongs", b =>
                {
                    b.HasOne("Beatecho.DAL.Models.Album", "Album")
                        .WithMany("AlbumSongs")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Beatecho.DAL.Models.Song", "Song")
                        .WithMany("AlbumSongs")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.ArtistAlbums", b =>
                {
                    b.HasOne("Beatecho.DAL.Models.Album", "Album")
                        .WithMany("ArtistAlbums")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Beatecho.DAL.Models.Artist", "Artist")
                        .WithMany("ArtistAlbums")
                        .HasForeignKey("ArtistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Artist");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.SongGenre", b =>
                {
                    b.HasOne("Beatecho.DAL.Models.Genre", "Genre")
                        .WithMany("SongGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Beatecho.DAL.Models.Song", "Song")
                        .WithMany("SongGenres")
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Genre");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.User", b =>
                {
                    b.HasOne("Beatecho.DAL.Models.UserType", "UserType")
                        .WithMany("Users")
                        .HasForeignKey("UserTypeId");

                    b.Navigation("UserType");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Album", b =>
                {
                    b.Navigation("AlbumSongs");

                    b.Navigation("ArtistAlbums");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Artist", b =>
                {
                    b.Navigation("ArtistAlbums");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Genre", b =>
                {
                    b.Navigation("SongGenres");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.Song", b =>
                {
                    b.Navigation("AlbumSongs");

                    b.Navigation("SongGenres");
                });

            modelBuilder.Entity("Beatecho.DAL.Models.UserType", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
