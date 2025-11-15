using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeroTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _1511_V13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRCodePdfPath",
                table: "DistributedAssets");

            migrationBuilder.DropColumn(
                name: "QRCodeValue",
                table: "DistributedAssets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QRCodePdfPath",
                table: "DistributedAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QRCodeValue",
                table: "DistributedAssets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
