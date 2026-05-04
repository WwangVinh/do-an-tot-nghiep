using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SyncConsignmentGuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Consignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consignments_Users_UserId",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "GuestEmail",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "GuestName",
                table: "Consignments");

            migrationBuilder.DropColumn(
                name: "GuestPhone",
                table: "Consignments");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Consignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consignments_Users_UserId",
                table: "Consignments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}