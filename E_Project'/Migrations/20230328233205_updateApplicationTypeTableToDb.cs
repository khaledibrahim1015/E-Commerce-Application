using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Project_.Migrations
{
    public partial class updateApplicationTypeTableToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MyProperty",
                table: "MyProperty");

            migrationBuilder.RenameTable(
                name: "MyProperty",
                newName: "ApplicationType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationType",
                table: "ApplicationType",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationType",
                table: "ApplicationType");

            migrationBuilder.RenameTable(
                name: "ApplicationType",
                newName: "MyProperty");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyProperty",
                table: "MyProperty",
                column: "Id");
        }
    }
}
