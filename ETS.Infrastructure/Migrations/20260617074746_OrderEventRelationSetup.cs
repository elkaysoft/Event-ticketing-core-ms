using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrderEventRelationSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Order_EventId",
                table: "Order",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Events_EventId",
                table: "Order",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Events_EventId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_EventId",
                table: "Order");
        }
    }
}
