using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MTGDraft.Migrations
{
    /// <inheritdoc />
    public partial class Draft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DraftSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetCode = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DraftSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PackNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalSeat = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packs_DraftSessions_DraftSessionId",
                        column: x => x.DraftSessionId,
                        principalTable: "DraftSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", nullable: false),
                    SetCode = table.Column<string>(type: "TEXT", nullable: false),
                    CardNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    SetId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PackId = table.Column<int>(type: "INTEGER", nullable: false),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFoil = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPicked = table.Column<bool>(type: "INTEGER", nullable: false),
                    PickedByPlayerId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackCards_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackCards_Packs_PackId",
                        column: x => x.PackId,
                        principalTable: "Packs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_SetId",
                table: "Cards",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_PackCards_CardId",
                table: "PackCards",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_PackCards_PackId",
                table: "PackCards",
                column: "PackId");

            migrationBuilder.CreateIndex(
                name: "IX_Packs_DraftSessionId",
                table: "Packs",
                column: "DraftSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackCards");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Packs");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "DraftSessions");
        }
    }
}
