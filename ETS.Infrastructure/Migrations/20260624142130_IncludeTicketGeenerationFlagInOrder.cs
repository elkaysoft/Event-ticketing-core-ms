using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncludeTicketGeenerationFlagInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTicketGenerated",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTicketGenerated",
                table: "Order");
        }
    }
}
