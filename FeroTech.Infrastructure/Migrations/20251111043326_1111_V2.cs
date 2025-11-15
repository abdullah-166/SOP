using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeroTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _1111_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReturnedDate",
                table: "DistributedAssets",
                newName: "EndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "DistributedAssets",
                newName: "ReturnedDate");
        }
    }
}
