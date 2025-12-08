using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class n2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextBillingDate",
                table: "ProfessionalProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PremiumSince",
                table: "ProfessionalProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionId",
                table: "ProfessionalProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionStatus",
                table: "ProfessionalProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerProfessionalId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "UnassignedAt",
                table: "Patients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Homeworks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Homeworks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Homeworks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                table: "Homeworks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionalUserId",
                table: "Homeworks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_PatientId",
                table: "Homeworks",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Homeworks_ProfessionalUserId",
                table: "Homeworks",
                column: "ProfessionalUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_Patients_PatientId",
                table: "Homeworks",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Homeworks_Users_ProfessionalUserId",
                table: "Homeworks",
                column: "ProfessionalUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_Patients_PatientId",
                table: "Homeworks");

            migrationBuilder.DropForeignKey(
                name: "FK_Homeworks_Users_ProfessionalUserId",
                table: "Homeworks");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_PatientId",
                table: "Homeworks");

            migrationBuilder.DropIndex(
                name: "IX_Homeworks_ProfessionalUserId",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "NextBillingDate",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "PremiumSince",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "SubscriptionStatus",
                table: "ProfessionalProfiles");

            migrationBuilder.DropColumn(
                name: "UnassignedAt",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "ProfessionalUserId",
                table: "Homeworks");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerProfessionalId",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Homeworks",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Homeworks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
