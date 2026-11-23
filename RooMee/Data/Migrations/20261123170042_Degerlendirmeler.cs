using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Degerlendirmeler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Degerlendirmeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YazanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HakkindaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Puan = table.Column<int>(type: "int", nullable: false),
                    Yorum = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Degerlendirmeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_AspNetUsers_HakkindaId",
                        column: x => x.HakkindaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Degerlendirmeler_AspNetUsers_YazanId",
                        column: x => x.YazanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_HakkindaId",
                table: "Degerlendirmeler",
                column: "HakkindaId");

            migrationBuilder.CreateIndex(
                name: "IX_Degerlendirmeler_YazanId_HakkindaId",
                table: "Degerlendirmeler",
                columns: new[] { "YazanId", "HakkindaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Degerlendirmeler");
        }
    }
}
