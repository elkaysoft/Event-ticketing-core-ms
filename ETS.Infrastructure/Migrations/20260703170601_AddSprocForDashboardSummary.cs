using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSprocForDashboardSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE [dbo].[sproc_GetDashboardSummary]
	        @StartDate DATETIME,
	        @EndDate DATETIME
        AS
        BEGIN
	        SET NOCOUNT ON;
            DECLARE @TotalTickets INT;
            DECLARE @ActiveEvents INT;

            -- Get total tickets from Events
            SELECT @TotalTickets = SUM(ev.ItemCount)
            FROM [Events] ev
            WHERE ev.CreatedAt >= @StartDate
            AND ev.CreatedAt <= @EndDate;

            SELECT @ActiveEvents = COUNT(p.Id)
            FROM [Events] p
            WHERE p.EventDate >= @StartDate
            AND p.EventDate <= @EndDate;

	         -- Daily breakdown CTE (reused for summary)
            WITH TicketRevenue AS (
                SELECT
                    CAST(oi.CreatedAt AS DATE)              AS TransactionDate,
                    SUM(oi.UnitPrice * oi.Unit)             AS DailyRevenue,
                    SUM(oi.Unit)                            AS DailyTicketsSold
                FROM OrderItem oi
                WHERE oi.PaymentStatus = 'Completed'
                  AND oi.CreatedAt >= @StartDate
                  AND oi.CreatedAt <= @EndDate
                GROUP BY CAST(oi.CreatedAt AS DATE)
            )

            SELECT 
                COALESCE(SUM(tr.DailyRevenue), 0) AS TotalRevenue,
                COALESCE(SUM(tr.DailyTicketsSold), 0) AS TotalTicketsSold,
                COALESCE(@TotalTickets, 0) AS TotalTickets,
                COALESCE(@ActiveEvents, 0) AS ActiveEvents
                FROM TicketRevenue tr;

            END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[sproc_GetDashboardSummary]");
        }
    }
}
