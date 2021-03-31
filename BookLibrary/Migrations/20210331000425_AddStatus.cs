using Microsoft.EntityFrameworkCore.Migrations;

namespace BookLibrary.Migrations
{
    public partial class AddStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Rentals",
                newName: "BeginDate");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "BeginDate",
                table: "Rentals",
                newName: "StartDate");
        }
    }
}
