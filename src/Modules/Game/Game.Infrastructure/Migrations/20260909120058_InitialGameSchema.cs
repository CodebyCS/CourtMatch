using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Game.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialGameSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourtId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    WinningTeam = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "player_rankings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GamesPlayed = table.Column<int>(type: "integer", nullable: false),
                    GamesWon = table.Column<int>(type: "integer", nullable: false),
                    SetsWon = table.Column<int>(type: "integer", nullable: false),
                    SetsLost = table.Column<int>(type: "integer", nullable: false),
                    RankingPoints = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_rankings", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "game_participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamNumber = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_game_participants_games_GameId",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_sets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    TeamOneGames = table.Column<int>(type: "integer", nullable: false),
                    TeamTwoGames = table.Column<int>(type: "integer", nullable: false),
                    TieBreakTeamOne = table.Column<int>(type: "integer", nullable: true),
                    TieBreakTeamTwo = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_game_sets_games_GameId",
                        column: x => x.GameId,
                        principalTable: "games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_participants_GameId_UserId",
                table: "game_participants",
                columns: new[] { "GameId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_game_sets_GameId_SetNumber",
                table: "game_sets",
                columns: new[] { "GameId", "SetNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_BookingId",
                table: "games",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_games_court_scheduled_at",
                table: "games",
                columns: new[] { "CourtId", "ScheduledAt" },
                unique: true,
                filter: "\"Status\" <> 'Cancelled'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_participants");

            migrationBuilder.DropTable(
                name: "game_sets");

            migrationBuilder.DropTable(
                name: "player_rankings");

            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
