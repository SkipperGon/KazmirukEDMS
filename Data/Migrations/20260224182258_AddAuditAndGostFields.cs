using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KazmirukEDMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditAndGostFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "DocumentVersions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignatureAlgorithm",
                table: "DocumentVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SigningCertificateSerial",
                table: "DocumentVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedById",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Documents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoragePeriodYears",
                table: "Documents",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "SignatureAlgorithm",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "SigningCertificateSerial",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ArchivedById",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "StoragePeriodYears",
                table: "Documents");
        }
    }
}
