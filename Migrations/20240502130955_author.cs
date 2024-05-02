using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dannys.Migrations
{
    public partial class author : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Authors",
                newName: "Surname");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Authors",
                newName: "FullName");
        }
    }
}
