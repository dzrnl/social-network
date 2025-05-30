﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SocialNetwork.Infrastructure.DataAccess;

#nullable disable

namespace SocialNetwork.Infrastructure.DataAccess.Migrations
{
    [DbContext(typeof(SocialNetworkDbContext))]
    [Migration("20250426212639_AddUniqueConstraintToUsername")]
    partial class AddUniqueConstraintToUsername
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Friendship", b =>
                {
                    b.Property<long>("FriendId")
                        .HasColumnType("bigint")
                        .HasColumnName("friend_id");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("user_id");

                    b.HasKey("FriendId", "UserId")
                        .HasName("pk_friendship");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_friendship_user_id");

                    b.ToTable("friendship", (string)null);
                });

            modelBuilder.Entity("SocialNetwork.Infrastructure.DataAccess.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password_hash");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("username");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasDatabaseName("ix_users_username");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Friendship", b =>
                {
                    b.HasOne("SocialNetwork.Infrastructure.DataAccess.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("FriendId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_friendship_users_friend_id");

                    b.HasOne("SocialNetwork.Infrastructure.DataAccess.Entities.UserEntity", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_friendship_users_user_id");
                });
#pragma warning restore 612, 618
        }
    }
}
