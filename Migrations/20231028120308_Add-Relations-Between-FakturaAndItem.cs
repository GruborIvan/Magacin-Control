using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSS_MagacinControl_App.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationsBetweenFakturaAndItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BrojFakture",
                table: "_css_RobaZaPakovanje_item",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX__css_RobaZaPakovanje_item_BrojFakture",
                table: "_css_RobaZaPakovanje_item",
                column: "BrojFakture");

            migrationBuilder.AddForeignKey(
                name: "FK__css_RobaZaPakovanje_item__css_RobaZaPakovanje_hd_BrojFakture",
                table: "_css_RobaZaPakovanje_item",
                column: "BrojFakture",
                principalTable: "_css_RobaZaPakovanje_hd",
                principalColumn: "BrojFakture",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__css_RobaZaPakovanje_item__css_RobaZaPakovanje_hd_BrojFakture",
                table: "_css_RobaZaPakovanje_item");

            migrationBuilder.DropIndex(
                name: "IX__css_RobaZaPakovanje_item_BrojFakture",
                table: "_css_RobaZaPakovanje_item");

            migrationBuilder.AlterColumn<string>(
                name: "BrojFakture",
                table: "_css_RobaZaPakovanje_item",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
