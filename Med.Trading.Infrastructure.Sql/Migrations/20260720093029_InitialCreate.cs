using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Med.Trading.Infrastructure.Sql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Trading");

            migrationBuilder.CreateTable(
                name: "Instruments",
                schema: "Trading",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m),
                    Volume24h = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m),
                    VolumeLastUpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2026, 7, 20, 9, 30, 28, 771, DateTimeKind.Utc).AddTicks(3050))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruments", x => x.Id);
                    table.UniqueConstraint("AK_Instruments_Symbol", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "UserPortfolios",
                schema: "Trading",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    InstrumentSymbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AvailableQuantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPortfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPortfolios_Instruments_InstrumentSymbol",
                        column: x => x.InstrumentSymbol,
                        principalSchema: "Trading",
                        principalTable: "Instruments",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TradeOrders",
                schema: "Trading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstrumentSymbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Side = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OrderType = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    PortfolioId = table.Column<long>(type: "bigint", nullable: false),
                    CommissionMoney = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilledOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TradeOrders_Instruments_InstrumentSymbol",
                        column: x => x.InstrumentSymbol,
                        principalSchema: "Trading",
                        principalTable: "Instruments",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TradeOrders_UserPortfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalSchema: "Trading",
                        principalTable: "UserPortfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentEntity_IsActive",
                schema: "Trading",
                table: "Instruments",
                columns: new[] { "UpdatedOn", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentEntity_VolumeLastUpdatedOn_IsActive",
                schema: "Trading",
                table: "Instruments",
                columns: new[] { "VolumeLastUpdatedOn", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Instruments_Symbol",
                schema: "Trading",
                table: "Instruments",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeOrders_InstrumentSymbol",
                schema: "Trading",
                table: "TradeOrders",
                column: "InstrumentSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOrders_PortfolioId",
                schema: "Trading",
                table: "TradeOrders",
                column: "PortfolioId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPortfolios_InstrumentSymbol",
                schema: "Trading",
                table: "UserPortfolios",
                column: "InstrumentSymbol");

            migrationBuilder.CreateIndex(
                name: "UX_UserPortfolio_UserId_InstrumentSymbol",
                schema: "Trading",
                table: "UserPortfolios",
                columns: new[] { "UserId", "InstrumentSymbol" },
                unique: true,
                filter: "[InstrumentSymbol] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeOrders",
                schema: "Trading");

            migrationBuilder.DropTable(
                name: "UserPortfolios",
                schema: "Trading");

            migrationBuilder.DropTable(
                name: "Instruments",
                schema: "Trading");
        }
    }
}
