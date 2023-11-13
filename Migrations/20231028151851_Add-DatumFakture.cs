using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class AddDatumFakture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DatumFakture",
                table: "_css_RobaZaPakovanje_hd",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatumFakture",
                table: "_css_RobaZaPakovanje_hd");
        }
    }
}
