using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class addImagePathforbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employee_shifts_employees_employee_id",
                table: "Employee_shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_shifts_shifts_shift_id",
                table: "Employee_shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee_shifts",
                table: "Employee_shifts");

            migrationBuilder.RenameTable(
                name: "Employee_shifts",
                newName: "employee_shifts");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_shifts_shift_id",
                table: "employee_shifts",
                newName: "IX_employee_shifts_shift_id");

            migrationBuilder.RenameIndex(
                name: "IX_Employee_shifts_employee_id",
                table: "employee_shifts",
                newName: "IX_employee_shifts_employee_id");

            migrationBuilder.AddColumn<string>(
                name: "image_path",
                table: "books",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employee_shifts",
                table: "employee_shifts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_employee_shifts_employees_employee_id",
                table: "employee_shifts",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employee_shifts_shifts_shift_id",
                table: "employee_shifts",
                column: "shift_id",
                principalTable: "shifts",
                principalColumn: "shifts",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employee_shifts_employees_employee_id",
                table: "employee_shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_employee_shifts_shifts_shift_id",
                table: "employee_shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employee_shifts",
                table: "employee_shifts");

            migrationBuilder.DropColumn(
                name: "image_path",
                table: "books");

            migrationBuilder.RenameTable(
                name: "employee_shifts",
                newName: "Employee_shifts");

            migrationBuilder.RenameIndex(
                name: "IX_employee_shifts_shift_id",
                table: "Employee_shifts",
                newName: "IX_Employee_shifts_shift_id");

            migrationBuilder.RenameIndex(
                name: "IX_employee_shifts_employee_id",
                table: "Employee_shifts",
                newName: "IX_Employee_shifts_employee_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee_shifts",
                table: "Employee_shifts",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_shifts_employees_employee_id",
                table: "Employee_shifts",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "employee_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_shifts_shifts_shift_id",
                table: "Employee_shifts",
                column: "shift_id",
                principalTable: "shifts",
                principalColumn: "shifts",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
