using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionForOrderItemEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OrderItem",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OrderItem");
        }
    }
}
