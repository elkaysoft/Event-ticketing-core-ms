using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderItemsWithPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTicketGenerated",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "TicketStatus",
                table: "OrderItem",
                newName: "TicketGenerationStatus");

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RedemptionStatus",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "RedemptionStatus",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "TicketGenerationStatus",
                table: "OrderItem",
                newName: "TicketStatus");

            migrationBuilder.AddColumn<bool>(
                name: "IsTicketGenerated",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
