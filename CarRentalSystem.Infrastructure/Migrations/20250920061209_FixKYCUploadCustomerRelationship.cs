using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixKYCUploadCustomerRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KYCUploads_Customers_CustomerId",
                table: "KYCUploads");

            migrationBuilder.AddForeignKey(
                name: "FK_KYCUploads_Customers_CustomerId",
                table: "KYCUploads",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KYCUploads_Customers_CustomerId",
                table: "KYCUploads");

            migrationBuilder.AddForeignKey(
                name: "FK_KYCUploads_Customers_CustomerId",
                table: "KYCUploads",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
