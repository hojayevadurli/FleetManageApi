using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorWorkOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderLines");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ServiceDate",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "WoNumber",
                table: "WorkOrders",
                newName: "WorkOrderNumber");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "WorkOrders",
                newName: "OpenedAt");

            migrationBuilder.RenameColumn(
                name: "Summary",
                table: "WorkOrders",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Odometer",
                table: "WorkOrders",
                newName: "OdometerAtService");

            migrationBuilder.RenameColumn(
                name: "DeletedStatus",
                table: "WorkOrders",
                newName: "HoursAtService");

            migrationBuilder.Sql("DELETE FROM \"WorkOrders\" WHERE \"EquipmentId\" IS NULL");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentId",
                table: "WorkOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ClosedAt",
                table: "WorkOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complaint",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostSource",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "WorkOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedTotal",
                table: "WorkOrders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WorkOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ManualActualTotal",
                table: "WorkOrders",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "WorkOrders",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkOrderInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    InvoiceDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrderInvoices_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Qty = table.Column<decimal>(type: "numeric", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PartNumber = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrderLineItems_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_VendorId",
                table: "WorkOrders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderInvoices_WorkOrderId",
                table: "WorkOrderInvoices",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLineItems_WorkOrderId",
                table: "WorkOrderLineItems",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ServicePartners_VendorId",
                table: "WorkOrders",
                column: "VendorId",
                principalTable: "ServicePartners",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ServicePartners_VendorId",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "WorkOrderInvoices");

            migrationBuilder.DropTable(
                name: "WorkOrderLineItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_VendorId",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Complaint",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "CostSource",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EstimatedTotal",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ManualActualTotal",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "WorkOrderNumber",
                table: "WorkOrders",
                newName: "WoNumber");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "WorkOrders",
                newName: "Summary");

            migrationBuilder.RenameColumn(
                name: "OpenedAt",
                table: "WorkOrders",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "OdometerAtService",
                table: "WorkOrders",
                newName: "Odometer");

            migrationBuilder.RenameColumn(
                name: "HoursAtService",
                table: "WorkOrders",
                newName: "DeletedStatus");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentId",
                table: "WorkOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WorkOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceDate",
                table: "WorkOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "WorkOrders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "WorkOrders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "WorkOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_WorkOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkOrderLines_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderLines_WorkOrderId",
                table: "WorkOrderLines",
                column: "WorkOrderId");
        }
    }
}
