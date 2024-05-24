    using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dannys.Migrations
{
    public partial class editProductPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "781a2698-29a1-48c8-9b95-7baf50b220d7");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d660a95b-330a-4a0c-986e-656416443a96");
        }
    }
}
