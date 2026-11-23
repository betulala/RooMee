using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RooMee.Data.Migrations
{
    /// <inheritdoc />
    public partial class EvArkadasiProfili : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Not: EF bu iki eski alanı yeni alanlara "yeniden adlandırma" sanıyordu; elle düzeltildi.
            // Eski değerler yeni seçeneklere aşağıda SQL ile aktarılıp eski sütunlar en sonda silinir.
            migrationBuilder.AddColumn<bool>(
                name: "Ogrenci",
                table: "Profiller",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Calisiyor",
                table: "Profiller",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AramaTuru",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BosOda",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalismaDuzeni",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cinsiyet",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DogumYili",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvTipi",
                table: "Profiller",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EvcilHayvan",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HobilerMetni",
                table: "Profiller",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ilce",
                table: "Profiller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Meslek",
                table: "Profiller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sehir",
                table: "Profiller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sinif",
                table: "Profiller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TercihOdaTipi",
                table: "Profiller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Uyku",
                table: "Profiller",
                type: "int",
                nullable: true);

            // Eski verileri taşı:
            //   EvcilHayvanVar = 1     -> EvcilHayvan = Sahibi (0)
            //   SessizOrtamIster = 1   -> CalismaDuzeni = EvdeSessiz (0)
            //   Bölümü/üniversitesi olan ya da doğrulanmış kullanıcı -> Ogrenci = 1
            migrationBuilder.Sql(@"
                UPDATE Profiller SET EvcilHayvan = 0 WHERE EvcilHayvanVar = 1;
                UPDATE Profiller SET CalismaDuzeni = 0 WHERE SessizOrtamIster = 1;
                UPDATE p SET Ogrenci = 1
                FROM Profiller p JOIN AspNetUsers u ON u.Id = p.KullaniciId
                WHERE u.OgrenciDogrulandi = 1 OR p.Universite IS NOT NULL OR p.Bolum IS NOT NULL;");

            migrationBuilder.DropColumn(name: "EvcilHayvanVar", table: "Profiller");
            migrationBuilder.DropColumn(name: "SessizOrtamIster", table: "Profiller");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(name: "EvcilHayvanVar", table: "Profiller", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>(name: "SessizOrtamIster", table: "Profiller", type: "bit", nullable: false, defaultValue: false);
            migrationBuilder.Sql(@"
                UPDATE Profiller SET EvcilHayvanVar = 1 WHERE EvcilHayvan = 0;
                UPDATE Profiller SET SessizOrtamIster = 1 WHERE CalismaDuzeni = 0;");

            migrationBuilder.DropColumn(name: "Ogrenci", table: "Profiller");
            migrationBuilder.DropColumn(name: "Calisiyor", table: "Profiller");

            migrationBuilder.DropColumn(
                name: "AramaTuru",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "BosOda",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "CalismaDuzeni",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "Cinsiyet",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "DogumYili",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "EvTipi",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "EvcilHayvan",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "HobilerMetni",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "Ilce",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "Meslek",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "Sehir",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "Sinif",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "TercihOdaTipi",
                table: "Profiller");

            migrationBuilder.DropColumn(
                name: "Uyku",
                table: "Profiller");
        }
    }
}
