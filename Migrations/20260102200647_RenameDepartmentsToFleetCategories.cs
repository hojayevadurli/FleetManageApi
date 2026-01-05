using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameDepartmentsToFleetCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Departments_DepartmentId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_Departments_DepartmentId",
                table: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "EquipmentTypes",
                newName: "FleetCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentTypes_DepartmentId",
                table: "EquipmentTypes",
                newName: "IX_EquipmentTypes_FleetCategoryId");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Equipments",
                newName: "FleetCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_DepartmentId",
                table: "Equipments",
                newName: "IX_Equipments_FleetCategoryId");

            migrationBuilder.CreateTable(
                name: "FleetCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IndustryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ListEquipment = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FleetCategories_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FleetCategories",
                columns: new[] { "Id", "Code", "IndustryId", "IsActive", "ListEquipment", "Name" },
                values: new object[,]
                {
                    { 101, "OPS", 2, true, true, "Fleet Operations" },
                    { 102, "MNT", 2, true, false, "Maintenance" },
                    { 103, "YRD", 2, true, false, "Yard & Terminal" },
                    { 201, "MOV", 1, true, true, "Earthmoving" },
                    { 202, "LFT", 1, true, true, "Lifting & Handling" },
                    { 203, "PAV", 1, true, true, "Road & Paving" },
                    { 204, "CON", 1, true, true, "Concrete" },
                    { 205, "UTL", 1, true, true, "Utilities & Support" },
                    { 206, "DEM", 1, true, true, "Demolition" },
                    { 207, "LOG", 1, true, false, "Site Logistics" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FleetCategories_IndustryId",
                table: "FleetCategories",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_FleetCategories_FleetCategoryId",
                table: "Equipments",
                column: "FleetCategoryId",
                principalTable: "FleetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_FleetCategories_FleetCategoryId",
                table: "EquipmentTypes",
                column: "FleetCategoryId",
                principalTable: "FleetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_FleetCategories_FleetCategoryId",
                table: "Equipments");

            migrationBuilder.DropForeignKey(
                name: "FK_EquipmentTypes_FleetCategories_FleetCategoryId",
                table: "EquipmentTypes");

            migrationBuilder.DropTable(
                name: "FleetCategories");

            migrationBuilder.RenameColumn(
                name: "FleetCategoryId",
                table: "EquipmentTypes",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_EquipmentTypes_FleetCategoryId",
                table: "EquipmentTypes",
                newName: "IX_EquipmentTypes_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "FleetCategoryId",
                table: "Equipments",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipments_FleetCategoryId",
                table: "Equipments",
                newName: "IX_Equipments_DepartmentId");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IndustryId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ListEquipment = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Industries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "IndustryId", "IsActive", "ListEquipment", "Name" },
                values: new object[,]
                {
                    { 101, "OPS", 2, true, true, "Fleet Operations" },
                    { 102, "MNT", 2, true, false, "Maintenance" },
                    { 103, "YRD", 2, true, false, "Yard & Terminal" },
                    { 201, "MOV", 1, true, true, "Earthmoving" },
                    { 202, "LFT", 1, true, true, "Lifting & Handling" },
                    { 203, "PAV", 1, true, true, "Road & Paving" },
                    { 204, "CON", 1, true, true, "Concrete" },
                    { 205, "UTL", 1, true, true, "Utilities & Support" },
                    { 206, "DEM", 1, true, true, "Demolition" },
                    { 207, "LOG", 1, true, false, "Site Logistics" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IndustryId",
                table: "Departments",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Departments_DepartmentId",
                table: "Equipments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EquipmentTypes_Departments_DepartmentId",
                table: "EquipmentTypes",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
