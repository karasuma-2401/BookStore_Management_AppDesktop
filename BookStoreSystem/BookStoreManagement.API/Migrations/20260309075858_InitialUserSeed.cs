using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                column: "password_hash",
                value: "$2a$12$kLRmN/MLkfha9kaVD.zPHOT7NHIlGPwoQ.FkyzQ.MHGa9.Oo3FT6u");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                column: "password_hash",
                value: "123");
        }
    }
}
