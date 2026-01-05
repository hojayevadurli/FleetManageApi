using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFleetCategorySeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FleetCategories",
                columns: new[] { "Id", "Code", "IndustryId", "IsActive", "ListEquipment", "Name" },
                values: new object[,]
                {
                    { 1, "MOVE", 1, true, true, "Earthmoving" },
                    { 2, "PAVE", 1, true, true, "Road & Paving" },
                    { 3, "CONC", 1, true, true, "Concrete" },
                    { 4, "UTIL", 1, true, true, "Utilities" },
                    { 5, "DEM", 1, true, true, "Demolition" },
                    { 6, "SITE", 1, true, true, "Site Logistics" },
                    { 7, "SERV", 1, true, true, "Service / Support" },
                    { 8, "SPARE", 1, true, true, "Spare / Backup" },
                    { 9, "LINE", 2, true, true, "Linehaul" },
                    { 10, "LOCAL", 2, true, true, "Local / P&D" },
                    { 11, "DED", 2, true, true, "Dedicated" },
                    { 12, "YARD", 2, true, true, "Yard / Spotter" },
                    { 13, "DRAY", 2, true, true, "Intermodal / Drayage" },
                    { 14, "SPARE", 2, true, true, "Spare / Backup" },
                    { 15, "OO", 2, true, true, "Owner-Op (Managed)" },
                    { 16, "ROUTE", 3, true, true, "AM / PM Routes" },
                    { 17, "SPEC", 3, true, true, "Special Needs" },
                    { 18, "FIELD", 3, true, true, "Field Trips / Charter" },
                    { 19, "ACT", 3, true, true, "Activity Buses" },
                    { 20, "SPARE", 3, true, true, "Spare / Relief" }
                });

            // Manual SQL to update existing Equipment to new Category IDs before deleting old Categories
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 9 WHERE \"FleetCategoryId\" = 101;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 14 WHERE \"FleetCategoryId\" = 102;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 12 WHERE \"FleetCategoryId\" = 103;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 1 WHERE \"FleetCategoryId\" = 201;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 7 WHERE \"FleetCategoryId\" = 202;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 2 WHERE \"FleetCategoryId\" = 203;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 3 WHERE \"FleetCategoryId\" = 204;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 4 WHERE \"FleetCategoryId\" = 205;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 5 WHERE \"FleetCategoryId\" = 206;");
            migrationBuilder.Sql("UPDATE \"Equipments\" SET \"FleetCategoryId\" = 6 WHERE \"FleetCategoryId\" = 207;");

            // Also update EquipmentTypes to ensure no cascade deletes trigger from there
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 9 WHERE \"FleetCategoryId\" = 101;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 14 WHERE \"FleetCategoryId\" = 102;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 12 WHERE \"FleetCategoryId\" = 103;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 1 WHERE \"FleetCategoryId\" = 201;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 7 WHERE \"FleetCategoryId\" = 202;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 2 WHERE \"FleetCategoryId\" = 203;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 3 WHERE \"FleetCategoryId\" = 204;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 4 WHERE \"FleetCategoryId\" = 205;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 5 WHERE \"FleetCategoryId\" = 206;");
            migrationBuilder.Sql("UPDATE \"EquipmentTypes\" SET \"FleetCategoryId\" = 6 WHERE \"FleetCategoryId\" = 207;");
            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1001,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1002,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1003,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1004,
                column: "FleetCategoryId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1005,
                column: "FleetCategoryId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1006,
                column: "FleetCategoryId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1007,
                column: "FleetCategoryId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1101,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1102,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1103,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1104,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1105,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1106,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1107,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1108,
                column: "FleetCategoryId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1201,
                column: "FleetCategoryId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1202,
                column: "FleetCategoryId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1203,
                column: "FleetCategoryId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1204,
                column: "FleetCategoryId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1301,
                column: "FleetCategoryId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1302,
                column: "FleetCategoryId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1303,
                column: "FleetCategoryId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1304,
                column: "FleetCategoryId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1305,
                column: "FleetCategoryId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2001,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2002,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2003,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2004,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2005,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2006,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2007,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2008,
                column: "FleetCategoryId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2101,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2102,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2103,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2104,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2105,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2106,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2107,
                column: "FleetCategoryId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2201,
                column: "FleetCategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2202,
                column: "FleetCategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2203,
                column: "FleetCategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2204,
                column: "FleetCategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2205,
                column: "FleetCategoryId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2301,
                column: "FleetCategoryId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2302,
                column: "FleetCategoryId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2303,
                column: "FleetCategoryId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2304,
                column: "FleetCategoryId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2305,
                column: "FleetCategoryId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2401,
                column: "FleetCategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2402,
                column: "FleetCategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2403,
                column: "FleetCategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2404,
                column: "FleetCategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2405,
                column: "FleetCategoryId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2501,
                column: "FleetCategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2502,
                column: "FleetCategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2503,
                column: "FleetCategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2504,
                column: "FleetCategoryId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2601,
                column: "FleetCategoryId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2602,
                column: "FleetCategoryId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2603,
                column: "FleetCategoryId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2604,
                column: "FleetCategoryId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2605,
                column: "FleetCategoryId",
                value: 6);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "FleetCategories",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1001,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1002,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1003,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1004,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1005,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1006,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1007,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1101,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1102,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1103,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1104,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1105,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1106,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1107,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1108,
                column: "FleetCategoryId",
                value: 101);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1201,
                column: "FleetCategoryId",
                value: 102);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1202,
                column: "FleetCategoryId",
                value: 102);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1203,
                column: "FleetCategoryId",
                value: 102);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1204,
                column: "FleetCategoryId",
                value: 102);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1301,
                column: "FleetCategoryId",
                value: 103);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1302,
                column: "FleetCategoryId",
                value: 103);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1303,
                column: "FleetCategoryId",
                value: 103);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1304,
                column: "FleetCategoryId",
                value: 103);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1305,
                column: "FleetCategoryId",
                value: 103);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2001,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2002,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2003,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2004,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2005,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2006,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2007,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2008,
                column: "FleetCategoryId",
                value: 201);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2101,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2102,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2103,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2104,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2105,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2106,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2107,
                column: "FleetCategoryId",
                value: 202);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2201,
                column: "FleetCategoryId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2202,
                column: "FleetCategoryId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2203,
                column: "FleetCategoryId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2204,
                column: "FleetCategoryId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2205,
                column: "FleetCategoryId",
                value: 203);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2301,
                column: "FleetCategoryId",
                value: 204);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2302,
                column: "FleetCategoryId",
                value: 204);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2303,
                column: "FleetCategoryId",
                value: 204);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2304,
                column: "FleetCategoryId",
                value: 204);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2305,
                column: "FleetCategoryId",
                value: 204);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2401,
                column: "FleetCategoryId",
                value: 205);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2402,
                column: "FleetCategoryId",
                value: 205);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2403,
                column: "FleetCategoryId",
                value: 205);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2404,
                column: "FleetCategoryId",
                value: 205);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2405,
                column: "FleetCategoryId",
                value: 205);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2501,
                column: "FleetCategoryId",
                value: 206);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2502,
                column: "FleetCategoryId",
                value: 206);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2503,
                column: "FleetCategoryId",
                value: 206);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2504,
                column: "FleetCategoryId",
                value: 206);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2601,
                column: "FleetCategoryId",
                value: 207);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2602,
                column: "FleetCategoryId",
                value: 207);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2603,
                column: "FleetCategoryId",
                value: 207);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2604,
                column: "FleetCategoryId",
                value: 207);

            migrationBuilder.UpdateData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2605,
                column: "FleetCategoryId",
                value: 207);

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
        }
    }
}
