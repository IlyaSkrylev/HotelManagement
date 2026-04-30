using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmploymentTypesUseShiftTypeInEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_employment_types_employment_type_id",
                table: "employees");

            migrationBuilder.DropTable(
                name: "employment_types");

            migrationBuilder.RenameColumn(
                name: "employment_type_id",
                table: "employees",
                newName: "shift_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_employees_employment_type_id",
                table: "employees",
                newName: "IX_employees_shift_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_employees_shift_types_shift_type_id",
                table: "employees",
                column: "shift_type_id",
                principalTable: "shift_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employees_shift_types_shift_type_id",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "shift_type_id",
                table: "employees",
                newName: "employment_type_id");

            migrationBuilder.RenameIndex(
                name: "IX_employees_shift_type_id",
                table: "employees",
                newName: "IX_employees_employment_type_id");

            migrationBuilder.CreateTable(
                name: "employment_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_types", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_employment_types_code",
                table: "employment_types",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_employment_types_employment_type_id",
                table: "employees",
                column: "employment_type_id",
                principalTable: "employment_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
