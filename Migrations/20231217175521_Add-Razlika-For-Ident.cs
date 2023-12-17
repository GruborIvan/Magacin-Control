using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class AddRazlikaForIdent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "_css_RobaZaPakovanje_item");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "_css_RobaZaPakovanje_hd");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "_css_IdentBarKod");

            migrationBuilder.AddColumn<int>(
                name: "Razlika",
                table: "_css_RobaZaPakovanje_item",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Razlika",
                table: "_css_RobaZaPakovanje_item");

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Users",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "_css_RobaZaPakovanje_item",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "_css_RobaZaPakovanje_hd",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "_css_IdentBarKod",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }
    }
}
