﻿// <auto-generated />
using System;
using CSS_MagacinControl_App.Models.DboModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231102135350_Add_SifraIdenta_To_Ident")]
    partial class Add_SifraIdenta_To_Ident
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.IdentBarkodDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BarkodIdenta")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NazivIdenta")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("_css_IdentBarKod");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.RobaZaPakovanjeDbo", b =>
                {
                    b.Property<string>("BrojFakture")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DatumFakture")
                        .HasColumnType("datetime2");

                    b.Property<string>("NazivKupca")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SifraKupca")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusFakture")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BrojFakture");

                    b.ToTable("_css_RobaZaPakovanje_hd");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.RobaZaPakovanjeItemDbo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("BrojFakture")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IdentBarkod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("KolicinaSaFakture")
                        .HasColumnType("int");

                    b.Property<string>("NazivIdenta")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PrimljenaKolicina")
                        .HasColumnType("int");

                    b.Property<string>("SifraIdenta")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.RobaZaPakovanjeItemDbo", b =>
                {
                    b.HasOne("CSS_MagacinControl_App.Models.DboModels.RobaZaPakovanjeDbo", "RobaZaPakovanje")
                        .WithMany("RobaZaPakovanjeItems")
                        .HasForeignKey("BrojFakture")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RobaZaPakovanje");
                });

            modelBuilder.Entity("CSS_MagacinControl_App.Models.DboModels.RobaZaPakovanjeDbo", b =>
                {
                    b.Navigation("RobaZaPakovanjeItems");
                });
#pragma warning restore 612, 618
        }
    }
}
