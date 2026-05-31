using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Neighborhood.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoneyFlowConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromoCodeUsages_PromoCodeId",
                table: "PromoCodeUsages");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PromoCodeUsages",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderToken",
                table: "PaymentMethods",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_OriginalTransactionId",
                table: "Transactions",
                column: "OriginalTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_PromoCodeId_UserId",
                table: "PromoCodeUsages",
                columns: new[] { "PromoCodeId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_OriginalTransactionId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodeUsages_PromoCodeId_UserId",
                table: "PromoCodeUsages");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PromoCodeUsages",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderToken",
                table: "PaymentMethods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_PromoCodeId",
                table: "PromoCodeUsages",
                column: "PromoCodeId");
        }
    }
}
