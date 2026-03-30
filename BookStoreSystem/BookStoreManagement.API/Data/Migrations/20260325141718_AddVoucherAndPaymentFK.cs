using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookStoreManagement.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherAndPaymentFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "invoice_id",
                table: "payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "voucher_id",
                table: "invoices",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    voucher_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    discount_percent = table.Column<int>(type: "integer", nullable: true),
                    discount_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usage_limit = table.Column<int>(type: "integer", nullable: true),
                    used_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vouchers", x => x.voucher_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payments_invoice_id",
                table: "payments",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_voucher_id",
                table: "invoices",
                column: "voucher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoices_vouchers_voucher_id",
                table: "invoices",
                column: "voucher_id",
                principalTable: "vouchers",
                principalColumn: "voucher_id");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_invoices_invoice_id",
                table: "payments",
                column: "invoice_id",
                principalTable: "invoices",
                principalColumn: "invoice_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoices_vouchers_voucher_id",
                table: "invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_invoices_invoice_id",
                table: "payments");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropIndex(
                name: "IX_payments_invoice_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_invoices_voucher_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "invoice_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "voucher_id",
                table: "invoices");
        }
    }
}
