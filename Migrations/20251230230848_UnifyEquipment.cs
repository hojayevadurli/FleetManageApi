using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class UnifyEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop old FKs
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Trucks_TruckId",
                table: "WorkOrders");

            // 2. Rename tables (Archive)
            migrationBuilder.RenameTable(
                name: "Trucks",
                newName: "ArchivedTrucks");

            migrationBuilder.RenameTable(
                name: "Trailers",
                newName: "ArchivedTrailers");

            // 3. Create Equipments Table
            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EquipmentTypeId = table.Column<int>(type: "integer", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: false),
                    UnitNumber = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Vin = table.Column<string>(type: "text", nullable: true),
                    SerialNumber = table.Column<string>(type: "text", nullable: true),
                    PlateNumber = table.Column<string>(type: "text", nullable: true),
                    Make = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    LifecycleStatus = table.Column<short>(type: "smallint", nullable: false),
                    OperationalStatus = table.Column<short>(type: "smallint", nullable: false),
                    OdometerCurrent = table.Column<int>(type: "integer", nullable: true),
                    HoursCurrent = table.Column<int>(type: "integer", nullable: true),
                    AcquiredDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InServiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    OutOfServiceDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Equipments_EquipmentTypes_EquipmentTypeId",
                        column: x => x.EquipmentTypeId,
                        principalTable: "EquipmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // 4. Data Migration (SQL)
            // Migrate Trucks -> Equipments (Type 1001, Dept 101)
            migrationBuilder.Sql(@"
                INSERT INTO ""Equipments""
                (""Id"", ""TenantId"", ""UnitNumber"", ""Vin"", ""SerialNumber"", ""PlateNumber"", ""Make"", ""Model"", ""Year"",
                 ""EquipmentTypeId"", ""DepartmentId"", ""LifecycleStatus"", ""OperationalStatus"", ""OdometerCurrent"",
                 ""AcquiredDate"", ""IsDeleted"", ""CreatedAt"", ""UpdatedAt"")
                SELECT
                 ""Id"", ""TenantId"", ""Number"", ""Vin"", NULL, ""PlateNumber"", ""Make"", ""Model"", ""Year"",
                 1001, 101, 1, 1, CAST(""Mileage"" AS integer),
                 CAST(""PurchasedAt"" AS date), FALSE, NOW(), NOW()
                FROM ""ArchivedTrucks"";
            ");

            // Migrate Trailers -> Equipments (Type 1101, Dept 101)
            migrationBuilder.Sql(@"
                INSERT INTO ""Equipments""
                (""Id"", ""TenantId"", ""UnitNumber"", ""Vin"", ""SerialNumber"", ""PlateNumber"", ""Make"", ""Model"", ""Year"",
                 ""EquipmentTypeId"", ""DepartmentId"", ""LifecycleStatus"", ""OperationalStatus"", ""OdometerCurrent"",
                 ""AcquiredDate"", ""IsDeleted"", ""CreatedAt"", ""UpdatedAt"")
                SELECT
                 ""Id"", ""TenantId"", ""Number"", ""Vin"", NULL, NULL, ""Make"", ""Model"", ""Year"",
                 1101, 101, 1, 1, NULL,
                 CAST(""PurchasedAt"" AS date), FALSE, NOW(), NOW()
                FROM ""ArchivedTrailers"";
            ");

            // 5. Schema Updates (WorkOrder & ServiceHistory)

            // WorkOrders: Drop TruckId column which was probably a shadow FK
            migrationBuilder.DropColumn(
                name: "TruckId",
                table: "WorkOrders");

            // Drop AssetType (historical)
            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "WorkOrders");

            // Rename AssetId -> EquipmentId (Preserving Data IDs)
            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "WorkOrders",
                newName: "EquipmentId");

            // Make EquipmentId Nullable (since WorkOrder might not reference Equipment, or orphaned)
            migrationBuilder.AlterColumn<Guid>(
                name: "EquipmentId",
                table: "WorkOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            // Create Index on new EquipmentId
            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_EquipmentId",
                table: "WorkOrders",
                column: "EquipmentId");


            // ServiceHistory: Drop AssetType
            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "ServiceHistories");

            // Rename AssetId -> EquipmentId
            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "ServiceHistories",
                newName: "EquipmentId");


            // 6. Indexes & FKs for Equipment
            migrationBuilder.CreateIndex(
                name: "IX_ServiceHistories_EquipmentId",
                table: "ServiceHistories",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_DepartmentId",
                table: "Equipments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_EquipmentTypeId",
                table: "Equipments",
                column: "EquipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_TenantId_UnitNumber",
                table: "Equipments",
                columns: new[] { "TenantId", "UnitNumber" },
                unique: true);

            // Clean up invalid FKs before applying constraint
            migrationBuilder.Sql(@"
                UPDATE ""WorkOrders"" 
                SET ""EquipmentId"" = NULL 
                WHERE ""EquipmentId"" IS NOT NULL 
                AND ""EquipmentId"" NOT IN (SELECT ""Id"" FROM ""Equipments"");

                DELETE FROM ""ServiceHistories"" 
                WHERE ""EquipmentId"" NOT IN (SELECT ""Id"" FROM ""Equipments"");
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceHistories_Equipments_EquipmentId",
                table: "ServiceHistories",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Equipments_EquipmentId",
                table: "WorkOrders",
                column: "EquipmentId",
                principalTable: "Equipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceHistories_Equipments_EquipmentId",
                table: "ServiceHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Equipments_EquipmentId",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_ServiceHistories_EquipmentId",
                table: "ServiceHistories");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "WorkOrders",
                newName: "TruckId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_EquipmentId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_TruckId");

            migrationBuilder.RenameColumn(
                name: "EquipmentId",
                table: "ServiceHistories",
                newName: "AssetId");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "WorkOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssetType",
                table: "ServiceHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Length = table.Column<double>(type: "double precision", nullable: true),
                    Make = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Vin = table.Column<string>(type: "text", nullable: false),
                    WeightCapacity = table.Column<double>(type: "double precision", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trucks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EngineType = table.Column<string>(type: "text", nullable: true),
                    Make = table.Column<string>(type: "text", nullable: true),
                    Mileage = table.Column<double>(type: "double precision", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: false),
                    PlateNumber = table.Column<string>(type: "text", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Vin = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trucks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_TenantId_Number",
                table: "Trailers",
                columns: new[] { "TenantId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_TenantId_Number",
                table: "Trucks",
                columns: new[] { "TenantId", "Number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Trucks_TruckId",
                table: "WorkOrders",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id");
        }
    }
}
