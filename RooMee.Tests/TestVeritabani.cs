using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Tests;

// Her test için hafızada (RAM) çalışan, boş bir SQLite veritabanı.
// Gerçek veritabanına (LocalDB) dokunmaz; test bitince kaybolur.
public sealed class TestVeritabani : IDisposable
{
    private readonly SqliteConnection _baglanti;
    private readonly DbContextOptions<ApplicationDbContext> _ayarlar;

    public TestVeritabani()
    {
        _baglanti = new SqliteConnection("DataSource=:memory:");
        _baglanti.Open();
        _ayarlar = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_baglanti).Options;
        using var db = YeniBaglam();
        db.Database.EnsureCreated();
    }

    // Her çağrıda yeni bir DbContext (gerçek uygulamadaki gibi istek başına bağlam)
    public ApplicationDbContext YeniBaglam() => new(_ayarlar);

    public void Dispose() => _baglanti.Dispose();

    // ---------- Örnek veri yardımcıları ----------

    public static ApplicationUser Kullanici(string ad, bool dogrulanmis = false, bool askida = false) => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = $"{ad.ToLowerInvariant()}@roomee.test",
        Email = $"{ad.ToLowerInvariant()}@roomee.test",
        Ad = ad,
        Soyad = "Test",
        OgrenciDogrulandi = dogrulanmis,
        AskiyaAlindi = askida,
    };

    public static Ilan Ilan(ApplicationUser sahip, string baslik, string sehir, int kira,
        OdaTipi tip = OdaTipi.OrtakOda, bool aktif = true) => new()
    {
        KullaniciId = sahip.Id,
        Baslik = baslik,
        Sehir = sehir,
        Ilce = "Merkez",
        Kira = kira,
        OdaTipi = tip,
        Aktif = aktif,
    };
}
