using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedEquipmentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EquipmentTypes",
                columns: new[] { "Id", "Code", "DepartmentId", "HasSerial", "HasVin", "IndustryId", "IsActive", "MeterMode", "Name" },
                values: new object[,]
                {
                    { 1001, "TRCTR", 101, false, true, 2, true, 1, "Tractor (Semi Truck)" },
                    { 1002, "DAY", 101, false, true, 2, true, 1, "Day Cab Truck" },
                    { 1003, "SLP", 101, false, true, 2, true, 1, "Sleeper Truck" },
                    { 1004, "STR", 101, false, true, 2, true, 1, "Straight Truck" },
                    { 1005, "BOX", 101, false, true, 2, true, 1, "Box Truck" },
                    { 1006, "VAN", 101, false, true, 2, true, 1, "Cargo Van" },
                    { 1007, "PUP", 101, false, true, 2, true, 1, "Pickup Truck" },
                    { 1101, "DV", 101, false, true, 2, true, 0, "Dry Van Trailer" },
                    { 1102, "RFR", 101, false, true, 2, true, 0, "Refrigerated Trailer (Reefer)" },
                    { 1103, "FLT", 101, false, true, 2, true, 0, "Flatbed Trailer" },
                    { 1104, "STD", 101, false, true, 2, true, 0, "Step Deck Trailer" },
                    { 1105, "LBY", 101, false, true, 2, true, 0, "Lowboy Trailer" },
                    { 1106, "TNK", 101, false, true, 2, true, 0, "Tanker Trailer" },
                    { 1107, "DMP", 101, false, true, 2, true, 0, "Dump Trailer" },
                    { 1108, "CHS", 101, false, true, 2, true, 0, "Intermodal Chassis" },
                    { 1201, "SVC", 102, false, true, 2, true, 1, "Service Truck" },
                    { 1202, "MMT", 102, false, true, 2, true, 1, "Mobile Mechanic Truck" },
                    { 1203, "TIRE", 102, false, true, 2, true, 1, "Tire Service Unit" },
                    { 1204, "FUEL", 102, false, true, 2, true, 1, "Fuel Service Truck" },
                    { 1301, "YRD", 103, false, true, 2, true, 1, "Yard Tractor (Spotter)" },
                    { 1302, "TRM", 103, false, true, 2, true, 1, "Terminal Tractor" },
                    { 1303, "GEN", 103, true, false, 2, true, 2, "Generator" },
                    { 1304, "LGT", 103, true, false, 2, true, 2, "Light Tower" },
                    { 1305, "OFF", 103, true, false, 2, true, 0, "Mobile Office Trailer" },
                    { 2001, "EXC", 201, true, false, 1, true, 2, "Excavator" },
                    { 2002, "MEX", 201, true, false, 1, true, 2, "Mini Excavator" },
                    { 2003, "DOZ", 201, true, false, 1, true, 2, "Bulldozer" },
                    { 2004, "WLD", 201, true, false, 1, true, 2, "Wheel Loader" },
                    { 2005, "SSL", 201, true, false, 1, true, 2, "Skid Steer Loader" },
                    { 2006, "CTL", 201, true, false, 1, true, 2, "Compact Track Loader" },
                    { 2007, "BHL", 201, true, true, 1, true, 3, "Backhoe Loader" },
                    { 2008, "GRD", 201, true, true, 1, true, 3, "Motor Grader" },
                    { 2101, "FLK", 202, true, false, 1, true, 2, "Forklift" },
                    { 2102, "RTF", 202, true, false, 1, true, 2, "Rough Terrain Forklift" },
                    { 2103, "TEL", 202, true, false, 1, true, 2, "Telehandler" },
                    { 2104, "CRN", 202, true, false, 1, true, 2, "Mobile Crane" },
                    { 2105, "TCR", 202, true, false, 1, true, 2, "Tower Crane" },
                    { 2106, "BML", 202, true, false, 1, true, 2, "Boom Lift" },
                    { 2107, "SCL", 202, true, false, 1, true, 2, "Scissor Lift" },
                    { 2201, "ROL", 203, true, false, 1, true, 2, "Road Roller (Compactor)" },
                    { 2202, "PAV", 203, true, false, 1, true, 2, "Asphalt Paver" },
                    { 2203, "MIL", 203, true, false, 1, true, 2, "Cold Planer (Milling)" },
                    { 2204, "SCP", 203, true, false, 1, true, 2, "Soil Compactor" },
                    { 2205, "PLC", 203, true, false, 1, true, 2, "Plate Compactor" },
                    { 2301, "CMT", 204, false, true, 1, true, 1, "Concrete Mixer Truck" },
                    { 2302, "CPT", 204, false, true, 1, true, 1, "Concrete Pump Truck" },
                    { 2303, "CBP", 204, true, false, 1, true, 2, "Concrete Batching Plant" },
                    { 2304, "PTR", 204, true, false, 1, true, 2, "Power Trowel" },
                    { 2305, "CSW", 204, true, false, 1, true, 2, "Concrete Saw" },
                    { 2401, "GEN", 205, true, false, 1, true, 2, "Generator" },
                    { 2402, "ACM", 205, true, false, 1, true, 2, "Air Compressor" },
                    { 2403, "LGT", 205, true, false, 1, true, 2, "Light Tower" },
                    { 2404, "WLDG", 205, true, false, 1, true, 2, "Welding Machine" },
                    { 2405, "PWSH", 205, true, false, 1, true, 2, "Pressure Washer" },
                    { 2501, "DEX", 206, true, false, 1, true, 2, "Demolition Excavator" },
                    { 2502, "HBR", 206, true, false, 1, true, 2, "Hydraulic Breaker" },
                    { 2503, "CRS", 206, true, false, 1, true, 2, "Crusher" },
                    { 2504, "SCR", 206, true, false, 1, true, 2, "Screener" },
                    { 2601, "DMP", 207, false, true, 1, true, 1, "Dump Truck" },
                    { 2602, "ADT", 207, true, false, 1, true, 2, "Articulated Dump Truck" },
                    { 2603, "WTR", 207, false, true, 1, true, 1, "Water Truck" },
                    { 2604, "STRL", 207, true, false, 1, true, 0, "Site Trailer" },
                    { 2605, "MOFF", 207, true, false, 1, true, 0, "Mobile Office" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1001);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1002);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1003);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1004);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1005);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1006);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1007);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1101);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1102);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1103);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1104);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1105);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1106);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1107);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1108);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1201);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1202);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1203);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1204);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1301);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1302);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1303);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1304);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 1305);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2002);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2004);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2005);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2006);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2007);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2008);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2101);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2102);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2103);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2104);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2105);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2106);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2107);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2201);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2202);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2203);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2204);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2205);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2301);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2302);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2303);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2304);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2305);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2401);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2402);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2403);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2404);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2405);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2501);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2502);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2503);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2504);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2601);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2602);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2603);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2604);

            migrationBuilder.DeleteData(
                table: "EquipmentTypes",
                keyColumn: "Id",
                keyValue: 2605);
        }
    }
}
