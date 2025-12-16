using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ne22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PremiumSince",
                table: "ProfessionalProfiles",
                newName: "PremiumActivatedAt");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionStatus",
                table: "ProfessionalProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FailedAttempts",
                table: "ProfessionalProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryAt",
                table: "ProfessionalProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tier",
                table: "ProfessionalProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedAttempts",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "NextRetryAt",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "Tier",
                table: "ProfessionalProfiles");

            migrationBuilder.RenameColumn(
                name: "PremiumActivatedAt",
                table: "ProfessionalProfiles",
                newName: "PremiumSince");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptionStatus",
                table: "ProfessionalProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
