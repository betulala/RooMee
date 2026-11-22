using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class Modeller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ad",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "KayitTarihi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "KvkkOnayTarihi",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Soyad",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Ilanlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Sehir = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ilce = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Kira = table.Column<int>(type: "int", nullable: false),
                    OdaTipi = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SadeceOgrenci = table.Column<bool>(type: "bit", nullable: false),
                    SigaraIcmeyen = table.Column<bool>(type: "bit", nullable: false),
                    Sessiz = table.Column<bool>(type: "bit", nullable: false),
                    EvcilHayvanKabul = table.Column<bool>(type: "bit", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ilanlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ilanlar_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Universite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Bolum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ButceMin = table.Column<int>(type: "int", nullable: true),
                    ButceMax = table.Column<int>(type: "int", nullable: true),
                    YatisSaati = table.Column<TimeOnly>(type: "time", nullable: true),
                    KalkisSaati = table.Column<TimeOnly>(type: "time", nullable: true),
                    SigaraIciyor = table.Column<bool>(type: "bit", nullable: false),
                    EvcilHayvanVar = table.Column<bool>(type: "bit", nullable: false),
                    SessizOrtamIster = table.Column<bool>(type: "bit", nullable: false),
                    Hakkinda = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FotoYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiller", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiller_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favoriler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favoriler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favoriler_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Favoriler_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IlanFotolari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlanId = table.Column<int>(type: "int", nullable: false),
                    Yol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlanFotolari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IlanFotolari_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sikayetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SikayetEdenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IlanId = table.Column<int>(type: "int", nullable: true),
                    SikayetEdilenKullaniciId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Neden = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sikayetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sikayetler_AspNetUsers_SikayetEdenId",
                        column: x => x.SikayetEdenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sikayetler_AspNetUsers_SikayetEdilenKullaniciId",
                        column: x => x.SikayetEdilenKullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sikayetler_Ilanlar_IlanId",
                        column: x => x.IlanId,
                        principalTable: "Ilanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_IlanId",
                table: "Favoriler",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Favoriler_KullaniciId_IlanId",
                table: "Favoriler",
                columns: new[] { "KullaniciId", "IlanId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IlanFotolari_IlanId",
                table: "IlanFotolari",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Ilanlar_KullaniciId",
                table: "Ilanlar",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiller_KullaniciId",
                table: "Profiller",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sikayetler_IlanId",
                table: "Sikayetler",
                column: "IlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Sikayetler_SikayetEdenId",
                table: "Sikayetler",
                column: "SikayetEdenId");

            migrationBuilder.CreateIndex(
                name: "IX_Sikayetler_SikayetEdilenKullaniciId",
                table: "Sikayetler",
                column: "SikayetEdilenKullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favoriler");

            migrationBuilder.DropTable(
                name: "IlanFotolari");

            migrationBuilder.DropTable(
                name: "Profiller");

            migrationBuilder.DropTable(
                name: "Sikayetler");

            migrationBuilder.DropTable(
                name: "Ilanlar");

            migrationBuilder.DropColumn(
                name: "Ad",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KayitTarihi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "KvkkOnayTarihi",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Soyad",
                table: "AspNetUsers");
        }
    }
}
