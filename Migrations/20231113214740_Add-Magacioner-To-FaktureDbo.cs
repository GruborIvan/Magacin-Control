using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class AddMagacionerToFaktureDbo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Magacioner",
                table: "_css_RobaZaPakovanje_hd",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Magacioner",
                table: "_css_RobaZaPakovanje_hd");
        }
    }
}
