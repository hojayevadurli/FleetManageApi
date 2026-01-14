using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveServiceHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Skipped dropping tables that were never created in migration history
            // migrationBuilder.DropTable(name: "ServiceHistoryLines");
            // migrationBuilder.DropTable(name: "ServiceHistories");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceDate",
                table: "WorkOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "WorkOrders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VendorNameRaw",
                table: "WorkOrders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "InvoiceDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "VendorNameRaw",
                table: "WorkOrders");

            migrationBuilder.CreateTable(
                name: "ServiceHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "numeric", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedStatus = table.Column<int>(type: "integer", nullable: true),
                    ExtractedJson = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: true),
                    Odometer = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VendorNameRaw = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHistories_Equipments_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceHistories_ServicePartners_VendorId",
                        column: x => x.VendorId,
                        principalTable: "ServicePartners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServiceHistories_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceHistoryLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceHistoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    PartNumber = table.Column<string>(type: "text", nullable: true),
                    Qty = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceHistoryLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceHistoryLines_ServiceHistories_ServiceHistoryId",
                        column: x => x.ServiceHistoryId,
                        principalTable: "ServiceHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistories_EquipmentId",
                table: "ServiceHistories",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistories_VendorId",
                table: "ServiceHistories",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistories_WorkOrderId",
                table: "ServiceHistories",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistoryLines_ServiceHistoryId",
                table: "ServiceHistoryLines",
                column: "ServiceHistoryId");
        }
    }
}
