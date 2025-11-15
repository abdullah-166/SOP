using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeroTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _1511_V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QRCodeValue",
                table: "DistributedAssets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "QRCodePdfPath",
                table: "DistributedAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DistributedAssets_AssetId",
                table: "DistributedAssets",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributedAssets_CategoryId",
                table: "DistributedAssets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributedAssets_EmployeeId",
                table: "DistributedAssets",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DistributedAssets_Assets_AssetId",
                table: "DistributedAssets",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistributedAssets_Categories_CategoryId",
                table: "DistributedAssets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DistributedAssets_Employees_EmployeeId",
                table: "DistributedAssets",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DistributedAssets_Assets_AssetId",
                table: "DistributedAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_DistributedAssets_Categories_CategoryId",
                table: "DistributedAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_DistributedAssets_Employees_EmployeeId",
                table: "DistributedAssets");

            migrationBuilder.DropIndex(
                name: "IX_DistributedAssets_AssetId",
                table: "DistributedAssets");

            migrationBuilder.DropIndex(
                name: "IX_DistributedAssets_CategoryId",
                table: "DistributedAssets");

            migrationBuilder.DropIndex(
                name: "IX_DistributedAssets_EmployeeId",
                table: "DistributedAssets");

            migrationBuilder.DropColumn(
                name: "QRCodePdfPath",
                table: "DistributedAssets");

            migrationBuilder.AlterColumn<string>(
                name: "QRCodeValue",
                table: "DistributedAssets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
