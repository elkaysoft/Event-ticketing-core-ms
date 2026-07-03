using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemIdInEmailLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemId",
                table: "EmailLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_OrderItemId",
                table: "EmailLogs",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailLogs_OrderItem_OrderItemId",
                table: "EmailLogs",
                column: "OrderItemId",
                principalTable: "OrderItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailLogs_OrderItem_OrderItemId",
                table: "EmailLogs");

            migrationBuilder.DropIndex(
                name: "IX_EmailLogs_OrderItemId",
                table: "EmailLogs");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "EmailLogs");
        }
    }
}
