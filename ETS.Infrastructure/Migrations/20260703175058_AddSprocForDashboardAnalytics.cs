using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSprocForDashboardAnalytics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER PROCEDURE [dbo].[sproc_GetDashboardAnalytics]
	@StartDate DATETIME,
	@EndDate DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
    DATENAME(MONTH, oi.CreatedAt) + ', ' + CAST(YEAR(oi.CreatedAt) AS VARCHAR(4)) AS TransactionMonth,
    SUM(oi.Unit) AS TotalTicketsSold
FROM OrderItem oi
WHERE oi.PaymentStatus = 'Completed'
  AND oi.CreatedAt >= @StartDate
  AND oi.CreatedAt <= @EndDate
GROUP BY
    YEAR(oi.CreatedAt),
    MONTH(oi.CreatedAt),
    DATENAME(MONTH, oi.CreatedAt)
ORDER BY
    YEAR(oi.CreatedAt),
    MONTH(oi.CreatedAt);

END
GO

");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS [dbo].[sproc_GetDashboardAnalytics]");
        }
    }
}
