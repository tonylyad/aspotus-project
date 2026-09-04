using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aspotus.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixLegacyCarPriceBackfill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE Cars
                SET Price = CASE
                    WHEN ModelId IN (SELECT Id FROM CarModels WHERE Name = 'Camry') THEN 14900000
                    WHEN ModelId IN (SELECT Id FROM CarModels WHERE Name = 'Corolla') THEN 10500000
                    WHEN ModelId IN (SELECT Id FROM CarModels WHERE Name = 'X5') THEN 31900000
                    ELSE 1000000
                END
                WHERE CAST(Price AS REAL) <= 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
