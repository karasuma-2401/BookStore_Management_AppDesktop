using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployee_ShiftMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                column: "password_hash",
                value: "$2a$12$D1vG.G0.iA22bZ3hU.Z8/e2xK.6kX4A1X.N/H.8H.J.K.U.V.Q.C.q");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "user_id",
                keyValue: 1,
                column: "password_hash",
                value: "$2a$12$kLRmN/MLkfha9kaVD.zPHOT7NHIlGPwoQ.FkyzQ.MHGa9.Oo3FT6u");
        }
    }
}
