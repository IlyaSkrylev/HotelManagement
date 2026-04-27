using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHotelIdToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "hotel_id",
                table: "employees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_employees_hotel_id",
                table: "employees",
                column: "hotel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_hotels_hotel_id",
                table: "employees",
                column: "hotel_id",
                principalTable: "hotels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_hotels_hotel_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_employees_hotel_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "hotel_id",
                table: "employees");
        }
    }
}
