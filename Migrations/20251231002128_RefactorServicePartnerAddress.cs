using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorServicePartnerAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "ServicePartners",
                newName: "Address1");

            // Manual cast for NetworkTier string -> int
            migrationBuilder.Sql(
                @"ALTER TABLE ""ServicePartners"" 
                  ALTER COLUMN ""NetworkTier"" TYPE integer 
                  USING CASE 
                    WHEN ""NetworkTier"" ILIKE 'Preferred' THEN 1 
                    WHEN ""NetworkTier"" ILIKE 'Warning' THEN 2 
                    WHEN ""NetworkTier"" ILIKE 'DoNotUse' THEN 3 
                    ELSE 0 
                  END;");
            
            // AlterColumn definition for EF model snapshot (optional but good for metadata if needed not really, Sql handles DB)
            // But we still need EF to know it's an int now. 
            // Actually, Sql() changes DB schema. EF snapshot already thinks it's int.
            // We just replaced the OPERATION that EF generates.
            // So we don't need AlterColumn again.

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "ServicePartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "ServicePartners",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ServicePartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ServicePartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IndustryId",
                table: "ServicePartners",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "ServicePartners",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ServicePartners",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServicePartners_IndustryId",
                table: "ServicePartners",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePartners_Industries_IndustryId",
                table: "ServicePartners",
                column: "IndustryId",
                principalTable: "Industries",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePartners_Industries_IndustryId",
                table: "ServicePartners");

            migrationBuilder.DropIndex(
                name: "IX_ServicePartners_IndustryId",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "IndustryId",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "ServicePartners");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ServicePartners");

            migrationBuilder.RenameColumn(
                name: "Address1",
                table: "ServicePartners",
                newName: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "NetworkTier",
                table: "ServicePartners",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
