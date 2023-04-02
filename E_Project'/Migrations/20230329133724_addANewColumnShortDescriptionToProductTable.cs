using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Project_.Migrations
{
    public partial class addANewColumnShortDescriptionToProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Product");
        }
    }
}
