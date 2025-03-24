﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using userService.Data;

#nullable disable

namespace userService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("userService.Models.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("userService.Models.DeviceUserRelation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("SetDevice")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("UserId");

                    b.ToTable("DeviceUserRelations");
                });

            modelBuilder.Entity("userService.Models.RelationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("DeviceUserRelationId1")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("DeviceUserRelationId2")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId1")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("UserId2")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceUserRelationId1");

                    b.HasIndex("DeviceUserRelationId2");

                    b.HasIndex("UserId1");

                    b.HasIndex("UserId2");

                    b.ToTable("RelationUsers");
                });

            modelBuilder.Entity("userService.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ConfirmationToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<DateTime>("CreatedAt"));

                    b.Property<string>("DeviceToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdatedAt")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)");

                    MySqlPropertyBuilderExtensions.UseMySqlComputedColumn(b.Property<DateTime>("UpdatedAt"));

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("userService.Models.DeviceUserRelation", b =>
                {
                    b.HasOne("userService.Models.Device", "Device")
                        .WithMany("DeviceUserRelations")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("userService.Models.User", "User")
                        .WithMany("DeviceUserRelations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("User");
                });

            modelBuilder.Entity("userService.Models.RelationUser", b =>
                {
                    b.HasOne("userService.Models.DeviceUserRelation", "DeviceUserRelation1")
                        .WithMany()
                        .HasForeignKey("DeviceUserRelationId1")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("userService.Models.DeviceUserRelation", "DeviceUserRelation2")
                        .WithMany()
                        .HasForeignKey("DeviceUserRelationId2")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("userService.Models.User", "User1")
                        .WithMany("RelationUsersAsUser1")
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("userService.Models.User", "User2")
                        .WithMany("RelationUsersAsUser2")
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("DeviceUserRelation1");

                    b.Navigation("DeviceUserRelation2");

                    b.Navigation("User1");

                    b.Navigation("User2");
                });

            modelBuilder.Entity("userService.Models.Device", b =>
                {
                    b.Navigation("DeviceUserRelations");
                });

            modelBuilder.Entity("userService.Models.User", b =>
                {
                    b.Navigation("DeviceUserRelations");

                    b.Navigation("RelationUsersAsUser1");

                    b.Navigation("RelationUsersAsUser2");
                });
#pragma warning restore 612, 618
        }
    }
}
