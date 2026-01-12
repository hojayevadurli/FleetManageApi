using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FleetManage.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTenantForStripe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingStatus",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CurrentPeriodEnd",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeactivatedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastActivityAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OnboardingCompletedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanKey",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceId",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionItemId",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SuspendedAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuspendedReason",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TrialEndsAt",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingStatus",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CurrentPeriodEnd",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "OnboardingCompletedAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "PlanKey",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripePriceId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionItemId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "SuspendedReason",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TrialEndsAt",
                table: "Tenants");
        }
    }
}
