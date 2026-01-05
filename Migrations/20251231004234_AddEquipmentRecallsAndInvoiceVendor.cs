using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentRecallsAndInvoiceVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "WorkOrderInvoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecallCampaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Manufacturer = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecallCampaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentRecalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecallCampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FirstSeenAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastCheckedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResolvedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentRecalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipmentRecalls_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipmentRecalls_RecallCampaigns_RecallCampaignId",
                        column: x => x.RecallCampaignId,
                        principalTable: "RecallCampaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderInvoices_VendorId",
                table: "WorkOrderInvoices",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRecalls_EquipmentId",
                table: "EquipmentRecalls",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipmentRecalls_RecallCampaignId",
                table: "EquipmentRecalls",
                column: "RecallCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_RecallCampaigns_Code",
                table: "RecallCampaigns",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderInvoices_ServicePartners_VendorId",
                table: "WorkOrderInvoices",
                column: "VendorId",
                principalTable: "ServicePartners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderInvoices_ServicePartners_VendorId",
                table: "WorkOrderInvoices");

            migrationBuilder.DropTable(
                name: "EquipmentRecalls");

            migrationBuilder.DropTable(
                name: "RecallCampaigns");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrderInvoices_VendorId",
                table: "WorkOrderInvoices");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "WorkOrderInvoices");
        }
    }
}
