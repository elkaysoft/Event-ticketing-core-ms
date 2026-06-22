using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncludeUnitSoldInEventCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemCount",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitSold",
                table: "EventCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "UnitSold",
                table: "EventCategory");
        }
    }
}
