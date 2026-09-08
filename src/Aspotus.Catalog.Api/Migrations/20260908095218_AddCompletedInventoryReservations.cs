using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aspotus.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedInventoryReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "InventoryReservations",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "InventoryReservations");
        }
    }
}
