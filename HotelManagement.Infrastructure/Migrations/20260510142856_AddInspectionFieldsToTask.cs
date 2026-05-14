using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionFieldsToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "inspected_at",
                table: "tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "inspected_by_id",
                table: "tasks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "requires_inspection",
                table: "tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_tasks_inspected_by_id",
                table: "tasks",
                column: "inspected_by_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_employees_inspected_by_id",
                table: "tasks",
                column: "inspected_by_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_employees_inspected_by_id",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_inspected_by_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "inspected_at",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "inspected_by_id",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "requires_inspection",
                table: "tasks");
        }
    }
}
