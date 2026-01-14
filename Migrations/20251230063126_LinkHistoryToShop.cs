using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class LinkHistoryToShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Skipped - ServiceHistories table does not exist in production
            // migrationBuilder.CreateIndex(name: "IX_ServiceHistories_VendorId", table: "ServiceHistories", column: "VendorId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_ServiceHistories_ServicePartners_VendorId",
            //     table: "ServiceHistories",
            //     column: "VendorId",
            //     principalTable: "ServicePartners",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistories_ServicePartners_VendorId",
                table: "ServiceHistories");

            migrationBuilder.DropIndex(
                name: "IX_ServiceHistories_VendorId",
                table: "ServiceHistories");
        }
    }
}
