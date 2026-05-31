using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceColumnsToEmployeeShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
            name: "check_in_time",
            table: "employee_shifts",
            type: "timestamp with time zone",
            nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_paid",
                table: "employee_shifts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "employee_shifts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "check_in_time",
                table: "employee_shifts");

            migrationBuilder.DropColumn(
                name: "is_paid",
                table: "employee_shifts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "employee_shifts");

        }
    }
}
