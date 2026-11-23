using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mesajlasma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Konusmalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kullanici1Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Kullanici2Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IlanId = table.Column<int>(type: "int", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonMesajTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Konusmalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Konusmalar_AspNetUsers_Kullanici1Id",
                        column: x => x.Kullanici1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Konusmalar_AspNetUsers_Kullanici2Id",
                        column: x => x.Kullanici2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Konusmalar_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Mesajlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KonusmaId = table.Column<int>(type: "int", nullable: false),
                    GonderenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Metin = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Okundu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesajlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mesajlar_AspNetUsers_GonderenId",
                        column: x => x.GonderenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mesajlar_Konusmalar_KonusmaId",
                        column: x => x.KonusmaId,
                        principalTable: "Konusmalar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Konusmalar_IlanId",
                table: "Konusmalar",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Konusmalar_Kullanici1Id_Kullanici2Id",
                table: "Konusmalar",
                columns: new[] { "Kullanici1Id", "Kullanici2Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Konusmalar_Kullanici2Id",
                table: "Konusmalar",
                column: "Kullanici2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GonderenId",
                table: "Mesajlar",
                column: "GonderenId");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_KonusmaId_Tarih",
                table: "Mesajlar",
                columns: new[] { "KonusmaId", "Tarih" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mesajlar");

            migrationBuilder.DropTable(
                name: "Konusmalar");
        }
    }
}
