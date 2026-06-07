using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookStoreManagement.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSellingPriceToImportDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "selling_price",
                table: "import_details",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "publish_year",
                table: "books",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "debt_reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    month = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    customer_id = table.Column<int>(type: "integer", nullable: false),
                    opening_debt = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    change_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    closing_debt = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debt_reports", x => x.report_id);
                    table.ForeignKey(
                        name: "FK_debt_reports_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_reports",
                columns: table => new
                {
                    report_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    month = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<int>(type: "integer", nullable: false),
                    opening_stock = table.Column<int>(type: "integer", nullable: false),
                    change_amount = table.Column<int>(type: "integer", nullable: false),
                    closing_stock = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_reports", x => x.report_id);
                    table.ForeignKey(
                        name: "FK_inventory_reports_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    setting_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.setting_name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_debt_reports_customer_id",
                table: "debt_reports",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_debt_reports_month_year_customer_id",
                table: "debt_reports",
                columns: new[] { "month", "year", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_inventory_reports_book_id",
                table: "inventory_reports",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_reports_month_year_book_id",
                table: "inventory_reports",
                columns: new[] { "month", "year", "book_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "debt_reports");

            migrationBuilder.DropTable(
                name: "inventory_reports");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropColumn(
                name: "selling_price",
                table: "import_details");

            migrationBuilder.AlterColumn<int>(
                name: "publish_year",
                table: "books",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
