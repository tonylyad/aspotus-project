using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aspotus.Orders.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CustomerEmail = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    CustomerPhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    DetailsJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRequests_CreatedAtUtc",
                table: "CustomerRequests",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerRequests");
        }
    }
}
