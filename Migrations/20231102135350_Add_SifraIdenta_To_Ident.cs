using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class Add_SifraIdenta_To_Ident : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SifraIdenta",
                table: "_css_RobaZaPakovanje_item",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SifraIdenta",
                table: "_css_RobaZaPakovanje_item");
        }
    }
}
