using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncludeRedemptionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "Order",
                newName: "RedemptionDate");

            migrationBuilder.AddColumn<string>(
                name: "RedemptionStatus",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RedemptionStatus",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "RedemptionDate",
                table: "Order",
                newName: "CompletedAt");
        }
    }
}
