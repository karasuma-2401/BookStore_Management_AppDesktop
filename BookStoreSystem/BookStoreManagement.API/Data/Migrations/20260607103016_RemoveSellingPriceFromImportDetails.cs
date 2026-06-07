using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSellingPriceFromImportDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selling_price",
                table: "import_details");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "selling_price",
                table: "import_details",
                type: "numeric(12,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
