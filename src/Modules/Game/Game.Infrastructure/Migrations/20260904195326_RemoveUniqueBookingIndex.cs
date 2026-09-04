using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueBookingIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_games_BookingId",
                table: "games");

            migrationBuilder.CreateIndex(
                name: "IX_games_BookingId",
                table: "games",
                column: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_games_BookingId",
                table: "games");

            migrationBuilder.CreateIndex(
                name: "IX_games_BookingId",
                table: "games",
                column: "BookingId",
                unique: true);
        }
    }
}
