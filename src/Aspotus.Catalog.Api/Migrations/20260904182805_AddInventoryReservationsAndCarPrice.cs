using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aspotus.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryReservationsAndCarPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Cars",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql("UPDATE Cars SET Price = 14900000 WHERE Id = '4F41E6ED-023E-47CE-953B-0BBF1C6FAF84'");
            migrationBuilder.Sql("UPDATE Cars SET Price = 10500000 WHERE Id = '9815FF46-C614-4A98-B1E0-7AD7BF5453A2'");
            migrationBuilder.Sql("UPDATE Cars SET Price = 31900000 WHERE Id = 'DAE455C6-22A7-4E90-822D-3796A557BF9A'");

            migrationBuilder.CreateTable(
                name: "InventoryReservations",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryReservations", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryReservationItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Article = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    BrandName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ModelName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    GenerationName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryReservationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryReservationItems_InventoryReservations_OrderId",
                        column: x => x.OrderId,
                        principalTable: "InventoryReservations",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationItems_OrderId",
                table: "InventoryReservationItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryReservationItems_ProductType_ProductId",
                table: "InventoryReservationItems",
                columns: new[] { "ProductType", "ProductId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryReservationItems");

            migrationBuilder.DropTable(
                name: "InventoryReservations");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Cars");
        }
    }
}
