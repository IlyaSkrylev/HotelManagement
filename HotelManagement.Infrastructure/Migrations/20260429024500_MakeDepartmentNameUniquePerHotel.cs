using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelManagement.Infrastructure.Migrations;

public partial class MakeDepartmentNameUniquePerHotel : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_departments_name",
            table: "departments");

        migrationBuilder.CreateIndex(
            name: "IX_departments_hotel_id_name",
            table: "departments",
            columns: new[] { "hotel_id", "name" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_departments_hotel_id_name",
            table: "departments");

        migrationBuilder.CreateIndex(
            name: "IX_departments_name",
            table: "departments",
            column: "name",
            unique: true);
    }
}
