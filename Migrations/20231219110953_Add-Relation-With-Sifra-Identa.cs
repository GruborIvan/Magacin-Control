using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationWithSifraIdenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NazivIdenta",
                table: "_css_IdentBarKod",
                newName: "SifraIdenta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SifraIdenta",
                table: "_css_IdentBarKod",
                newName: "NazivIdenta");
        }
    }
}
