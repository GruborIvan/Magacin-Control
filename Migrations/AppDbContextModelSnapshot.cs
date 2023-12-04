﻿// <auto-generated />
using System;
using CSS_MagacinControl_App.Models.DboModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.FakturaDbo", b =>
                {
                    b.Property<string>("BrojFakture")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DatumFakture")
                        .HasColumnType("datetime2");

                    b.Property<string>("Magacioner")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NazivKupca")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SifraKupca")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusFakture")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("BrojFakture");

                    b.ToTable("_css_RobaZaPakovanje_hd");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.IdentBarkodDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BarkodIdenta")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NazivIdenta")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.ToTable("_css_IdentBarKod");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.IdentDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BrojFakture")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("KolicinaSaFakture")
                        .HasColumnType("int");

                    b.Property<string>("NazivIdenta")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Oznaka")
                        .HasColumnType("int");

                    b.Property<string>("OznakaUsluge")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PrimljenaKolicina")
                        .HasColumnType("int");

                    b.Property<string>("SifraIdenta")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("BrojFakture");

                    b.ToTable("_css_RobaZaPakovanje_item");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.UserDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.IdentDbo", b =>
                {
                    b.HasOne("CSS_MagacinControl_App.Models.DboModels.FakturaDbo", "RobaZaPakovanje")
                        .WithMany("RobaZaPakovanjeItems")
                        .HasForeignKey("BrojFakture");

                    b.Navigation("RobaZaPakovanje");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.FakturaDbo", b =>
                {
                    b.Navigation("RobaZaPakovanjeItems");
                });
#pragma warning restore 612, 618
        }
    }
}
