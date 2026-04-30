using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShiftTypeCycleAndEmployeeCycleStartFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_time",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "start_time",
                table: "shift_types");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "day_shift_end_time",
                table: "shift_types",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "day_shift_start_time",
                table: "shift_types",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "night_shift_end_time",
                table: "shift_types",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "night_shift_start_time",
                table: "shift_types",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "rest_days",
                table: "shift_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_cycle_days",
                table: "shift_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "working_day_shifts",
                table: "shift_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "working_night_shifts",
                table: "shift_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "shift_cycle_starts_with_day",
                table: "employees",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "day_shift_end_time",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "day_shift_start_time",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "night_shift_end_time",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "night_shift_start_time",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "rest_days",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "total_cycle_days",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "working_day_shifts",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "working_night_shifts",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "shift_cycle_starts_with_day",
                table: "employees");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "end_time",
                table: "shift_types",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "start_time",
                table: "shift_types",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
