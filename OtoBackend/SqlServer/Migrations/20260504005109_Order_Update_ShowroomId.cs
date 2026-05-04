using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Order_Update_ShowroomId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShowroomId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShowroomId",
                table: "Orders",
                column: "ShowroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Showrooms_ShowroomId",
                table: "Orders",
                column: "ShowroomId",
                principalTable: "Showrooms",
                principalColumn: "ShowroomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Showrooms_ShowroomId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShowroomId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShowroomId",
                table: "Orders");
        }
    }
}
