using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class InitialModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_css_IdentBarKod",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NazivIdenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BarkodIdenta = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__css_IdentBarKod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "_css_RobaZaPakovanje_hd",
                columns: table => new
                {
                    BrojFakture = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Datum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SifraKupca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NazivKupca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusFakture = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__css_RobaZaPakovanje_hd", x => x.BrojFakture);
                });

            migrationBuilder.CreateTable(
                name: "_css_RobaZaPakovanje_item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrojFakture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentBarkod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NazivIdenta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KolicinaSaFakture = table.Column<int>(type: "int", nullable: false),
                    PrimljenaKolicina = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__css_RobaZaPakovanje_item", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_css_IdentBarKod");

            migrationBuilder.DropTable(
                name: "_css_RobaZaPakovanje_hd");

            migrationBuilder.DropTable(
                name: "_css_RobaZaPakovanje_item");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
