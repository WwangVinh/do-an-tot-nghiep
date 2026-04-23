using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddCarPricingVersionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarPricingVersions",
                columns: table => new
                {
                    PricingVersionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    VersionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PriceVnd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPricingVersions", x => x.PricingVersionId);
                    table.ForeignKey(
                        name: "FK_CarPricingVersions_Cars",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "CarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarPricingVersions_CarId",
                table: "CarPricingVersions",
                column: "CarId");

            migrationBuilder.Sql(
                """
                INSERT INTO [CarPricingVersions] ([CarId], [VersionName], [PriceVnd], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt])
                SELECT c.CarId, v.VersionName, v.PriceVnd, v.SortOrder, 1, GETDATE(), GETDATE()
                FROM [Cars] c
                INNER JOIN (
                    VALUES
                        ('VF7', N'VinFast VF7 Eco - Kèm Pin', CAST(799000000 AS decimal(18,2)), 1),
                        ('VF7', N'VinFast VF7 Plus Trần Thép - Nâng Cấp', CAST(999000000 AS decimal(18,2)), 2),
                        ('VF7', N'VinFast VF7 Plus Trần Kính - Nâng Cấp', CAST(1019000000 AS decimal(18,2)), 3),
                        ('VF8', N'VinFast VF8 Eco', CAST(1090000000 AS decimal(18,2)), 1),
                        ('VF8', N'VinFast VF8 Plus', CAST(1260000000 AS decimal(18,2)), 2),
                        ('VF3', N'VinFast VF3 Tiêu chuẩn', CAST(281000000 AS decimal(18,2)), 1),
                        ('VF5', N'VinFast VF5 Eco', CAST(458000000 AS decimal(18,2)), 1),
                        ('VF5', N'VinFast VF5 Plus', CAST(497000000 AS decimal(18,2)), 2),
                        ('VF6', N'VinFast VF6 Eco', CAST(599000000 AS decimal(18,2)), 1),
                        ('VF6', N'VinFast VF6 Plus', CAST(647000000 AS decimal(18,2)), 2),
                        ('VF9', N'VinFast VF9 Eco', CAST(1251000000 AS decimal(18,2)), 1),
                        ('VF9', N'VinFast VF9 Plus', CAST(1349000000 AS decimal(18,2)), 2)
                ) v (CarCode, VersionName, PriceVnd, SortOrder)
                    ON UPPER(ISNULL(c.[Name], '')) LIKE '%' + v.CarCode + '%'
                WHERE c.[IsDeleted] = 0
                  AND NOT EXISTS (
                      SELECT 1
                      FROM [CarPricingVersions] cpv
                      WHERE cpv.[CarId] = c.[CarId]
                        AND cpv.[VersionName] = v.[VersionName]
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPricingVersions");
        }
    }
}
