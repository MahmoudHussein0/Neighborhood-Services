using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighborhood.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changingprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AiAnalyses_BookingId",
                table: "AiAnalyses");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "AiAnalyses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_AiAnalyses_BookingId",
                table: "AiAnalyses",
                column: "BookingId",
                unique: true,
                filter: "[BookingId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AiAnalyses_BookingId",
                table: "AiAnalyses");

            migrationBuilder.AlterColumn<int>(
                name: "BookingId",
                table: "AiAnalyses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiAnalyses_BookingId",
                table: "AiAnalyses",
                column: "BookingId",
                unique: true);
        }
    }
}
