﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using playground.Data;

namespace playground.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20210204073212_AddedDateTimeForKeys")]
    partial class AddedDateTimeForKeys
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("playground.Entities.ERole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EUserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EUserId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("playground.Entities.EUser", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedUTC")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastLoginUTC")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastUpdatedUTC")
                        .HasColumnType("TEXT");

                    b.Property<int>("LoginsCount")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("PasswordHash")
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("BLOB");

                    b.HasKey("id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("playground.Entities.EUserActionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ActionKey")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedUTC")
                        .HasColumnType("TEXT");

                    b.Property<int?>("EUserid")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EUserid");

                    b.ToTable("UserActionKeys");
                });

            modelBuilder.Entity("playground.Entities.ERole", b =>
                {
                    b.HasOne("playground.Entities.EUser", "EUser")
                        .WithMany("Roles")
                        .HasForeignKey("EUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EUser");
                });

            modelBuilder.Entity("playground.Entities.EUserActionKey", b =>
                {
                    b.HasOne("playground.Entities.EUser", "EUser")
                        .WithMany("UserActionKeys")
                        .HasForeignKey("EUserid")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("EUser");
                });

            modelBuilder.Entity("playground.Entities.EUser", b =>
                {
                    b.Navigation("Roles");

                    b.Navigation("UserActionKeys");
                });
#pragma warning restore 612, 618
        }
    }
}
