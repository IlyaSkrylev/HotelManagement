using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HotelManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "employment_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employment_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resume_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resume_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shift_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task_priorities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    level = table.Column<int>(type: "integer", nullable: true),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_priorities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "task_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_login = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hotels",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    address = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    created_by_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotels", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotels_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hotel_floors",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hotel_id = table.Column<long>(type: "bigint", nullable: false),
                    floor_number = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_floors", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotel_floors_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_hotel_roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    hotel_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_hotel_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_hotel_roles_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_hotel_roles_user_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "user_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_hotel_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hotel_rooms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hotel_id = table.Column<long>(type: "bigint", nullable: false),
                    floor_id = table.Column<long>(type: "bigint", nullable: false),
                    room_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    room_status_id = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotel_rooms_hotel_floors_floor_id",
                        column: x => x.floor_id,
                        principalTable: "hotel_floors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hotel_rooms_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_hotel_rooms_room_statuses_room_status_id",
                        column: x => x.room_status_id,
                        principalTable: "room_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    hotel_id = table.Column<long>(type: "bigint", nullable: false),
                    manager_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departments", x => x.id);
                    table.ForeignKey(
                        name: "FK_departments_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    department_id = table.Column<long>(type: "bigint", nullable: false),
                    position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    hire_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    dismissal_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    dismissal_reason = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    salary = table.Column<int>(type: "integer", nullable: true),
                    salary_supplement = table.Column<int>(type: "integer", nullable: true),
                    employment_type_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    shift_cycle_start_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employees", x => x.id);
                    table.ForeignKey(
                        name: "FK_employees_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employees_employment_types_employment_type_id",
                        column: x => x.employment_type_id,
                        principalTable: "employment_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_employees_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "task_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    department_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_types", x => x.id);
                    table.ForeignKey(
                        name: "FK_task_types_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "financial_operations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    created_by_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_operations", x => x.id);
                    table.ForeignKey(
                        name: "FK_financial_operations_employees_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_financial_operations_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resumes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    hotel_id = table.Column<long>(type: "bigint", nullable: false),
                    desired_position = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    experience = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    education = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    file_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reviewed_by_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resumes", x => x.id);
                    table.ForeignKey(
                        name: "FK_resumes_employees_reviewed_by_id",
                        column: x => x.reviewed_by_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_resumes_hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resumes_resume_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "resume_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_resumes_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_shifts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_id = table.Column<long>(type: "bigint", nullable: false),
                    shift_type_id = table.Column<long>(type: "bigint", nullable: false),
                    shift_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    start_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    end_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_shifts", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_shifts_employees_employee_id",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_work_shifts_shift_types_shift_type_id",
                        column: x => x.shift_type_id,
                        principalTable: "shift_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    task_type_id = table.Column<long>(type: "bigint", nullable: false),
                    task_status_id = table.Column<long>(type: "bigint", nullable: false),
                    priority_id = table.Column<long>(type: "bigint", nullable: false),
                    assigned_to_id = table.Column<long>(type: "bigint", nullable: false),
                    created_by_id = table.Column<long>(type: "bigint", nullable: false),
                    room_id = table.Column<long>(type: "bigint", nullable: true),
                    due_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_tasks_employees_assigned_to_id",
                        column: x => x.assigned_to_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tasks_employees_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tasks_hotel_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "hotel_rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_tasks_task_priorities_priority_id",
                        column: x => x.priority_id,
                        principalTable: "task_priorities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tasks_task_statuses_task_status_id",
                        column: x => x.task_status_id,
                        principalTable: "task_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tasks_task_types_task_type_id",
                        column: x => x.task_type_id,
                        principalTable: "task_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_departments_hotel_id",
                table: "departments",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_manager_id",
                table: "departments",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_name",
                table: "departments",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employees_department_id",
                table: "employees",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_employees_employment_type_id",
                table: "employees",
                column: "employment_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_employees_user_id",
                table: "employees",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_employment_types_code",
                table: "employment_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_financial_operations_created_by_id",
                table: "financial_operations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_financial_operations_employee_id",
                table: "financial_operations",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_floors_hotel_id",
                table: "hotel_floors",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_rooms_floor_id",
                table: "hotel_rooms",
                column: "floor_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_rooms_hotel_id",
                table: "hotel_rooms",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_rooms_room_status_id",
                table: "hotel_rooms",
                column: "room_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_hotels_created_by_id",
                table: "hotels",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_resume_statuses_code",
                table: "resume_statuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_resumes_hotel_id",
                table: "resumes",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_resumes_reviewed_by_id",
                table: "resumes",
                column: "reviewed_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_resumes_status_id",
                table: "resumes",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_resumes_user_id",
                table: "resumes",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_statuses_code",
                table: "room_statuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shift_types_code",
                table: "shift_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_priorities_code",
                table: "task_priorities",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_statuses_code",
                table: "task_statuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_types_code",
                table: "task_types",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_task_types_department_id",
                table: "task_types",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_assigned_to_id",
                table: "tasks",
                column: "assigned_to_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_created_by_id",
                table: "tasks",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_priority_id",
                table: "tasks",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_room_id",
                table: "tasks",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_task_status_id",
                table: "tasks",
                column: "task_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_task_type_id",
                table: "tasks",
                column: "task_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_hotel_roles_hotel_id",
                table: "user_hotel_roles",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_hotel_roles_role_id",
                table: "user_hotel_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_hotel_roles_user_id",
                table: "user_hotel_roles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_code",
                table: "user_roles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_shifts_employee_id",
                table: "work_shifts",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_shifts_shift_type_id",
                table: "work_shifts",
                column: "shift_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_departments_employees_manager_id",
                table: "departments",
                column: "manager_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_departments_employees_manager_id",
                table: "departments");

            migrationBuilder.DropTable(
                name: "financial_operations");

            migrationBuilder.DropTable(
                name: "resumes");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "user_hotel_roles");

            migrationBuilder.DropTable(
                name: "work_shifts");

            migrationBuilder.DropTable(
                name: "resume_statuses");

            migrationBuilder.DropTable(
                name: "hotel_rooms");

            migrationBuilder.DropTable(
                name: "task_priorities");

            migrationBuilder.DropTable(
                name: "task_statuses");

            migrationBuilder.DropTable(
                name: "task_types");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "shift_types");

            migrationBuilder.DropTable(
                name: "hotel_floors");

            migrationBuilder.DropTable(
                name: "room_statuses");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "employment_types");

            migrationBuilder.DropTable(
                name: "hotels");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
