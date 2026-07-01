using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GetTransactionReportProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE [dbo].[sproc_GetOrderItemsReport]
    @StartDate DATETIME,
    @EndDate   DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    -- Daily breakdown CTE (reused for summary, analytics and details)
    WITH DailyRevenue AS (
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
        -- Summary
        COALESCE(SUM(dr.DailyRevenue), 0)       AS TotalRevenue,
        COALESCE(SUM(dr.DailyTicketsSold), 0)   AS TicketSold,

        -- Analytics
        COALESCE(MAX(dr.DailyRevenue), 0)        AS MaximumRevenue,
        COALESCE(MIN(dr.DailyRevenue), 0)        AS MinimumRevenue
    FROM DailyRevenue dr;

    -- Analytics details: daily breakdown
    SELECT
        CAST(oi.CreatedAt AS DATE)              AS TransactionDate,
        COALESCE(SUM(oi.UnitPrice * oi.Unit), 0) AS Amount
    FROM OrderItem oi
    WHERE oi.PaymentStatus = 'Completed'
      AND oi.CreatedAt >= @StartDate
      AND oi.CreatedAt <= @EndDate
    GROUP BY CAST(oi.CreatedAt AS DATE)
    ORDER BY CAST(oi.CreatedAt AS DATE);
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE sproc_GetOrderItemsReport");
        }
    }
}
