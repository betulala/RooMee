using RooMee.Controllers;
using RooMee.Models;
using RooMee.Services;

namespace RooMee.Tests;

// Değerlendirme hakkı: sadece karşılıklı mesajlaşan iki kişi birbirini değerlendirebilir
public class DegerlendirmeHakkiTestleri : IDisposable
{
    private readonly TestVeritabani _vt = new();
    private readonly ApplicationUser _ali = TestVeritabani.Kullanici("Ali");
    private readonly ApplicationUser _veli = TestVeritabani.Kullanici("Veli");

    public DegerlendirmeHakkiTestleri()
    {
        using var db = _vt.YeniBaglam();
        db.Users.AddRange(_ali, _veli);
        db.SaveChanges();
    }

    public void Dispose() => _vt.Dispose();

    private void KonusmaEkle(params ApplicationUser[] gonderenler)
    {
        using var db = _vt.YeniBaglam();
        var (k1, k2) = Konusma.Sirala(_ali.Id, _veli.Id);
        var k = new Konusma { Kullanici1Id = k1, Kullanici2Id = k2 };
        foreach (var g in gonderenler) k.Mesajlar.Add(new Mesaj { GonderenId = g.Id, Metin = "test" });
        db.Konusmalar.Add(k);
        db.SaveChanges();
    }

    [Fact]
    public async Task Hic_konusma_yoksa_degerlendiremez()
    {
        using var db = _vt.YeniBaglam();
        Assert.False(await DegerlendirmeController.DegerlendirebilirMi(db, _ali.Id, _veli.Id));
    }

    [Fact]
    public async Task Sadece_tek_taraf_yazdiysa_degerlendiremez()
    {
        KonusmaEkle(_ali, _ali);
        using var db = _vt.YeniBaglam();
        Assert.False(await DegerlendirmeController.DegerlendirebilirMi(db, _ali.Id, _veli.Id));
    }

    [Fact]
    public async Task Karsilikli_mesajlasan_iki_kisi_birbirini_degerlendirebilir()
    {
        KonusmaEkle(_ali, _veli);
        using var db = _vt.YeniBaglam();
        Assert.True(await DegerlendirmeController.DegerlendirebilirMi(db, _ali.Id, _veli.Id));
        Assert.True(await DegerlendirmeController.DegerlendirebilirMi(db, _veli.Id, _ali.Id));
    }

    [Fact]
    public async Task Kimse_kendini_degerlendiremez()
    {
        using var db = _vt.YeniBaglam();
        Assert.False(await DegerlendirmeController.DegerlendirebilirMi(db, _ali.Id, _ali.Id));
    }
}

public class KucukYardimciTestleri
{
    [Theory]
    [InlineData("İzmir")]
    [InlineData("izmir")]
    [InlineData("Izmir")]
    [InlineData("  İZMİR ")]
    public void Sehir_konumu_farkli_yazimlarla_bulunur(string yazim)
    {
        var konum = SehirKonumlari.Bul(yazim);
        Assert.NotNull(konum);
        Assert.InRange(konum!.Value.Enlem, 38, 39);
    }

    [Fact]
    public void Bilinmeyen_sehir_icin_konum_yoktur()
    {
        Assert.Null(SehirKonumlari.Bul("Atlantis"));
        Assert.Null(SehirKonumlari.Bul(""));
    }

    [Fact]
    public void Konusma_kullanici_sirasi_her_iki_yonde_ayni_cikar()
    {
        Assert.Equal(Konusma.Sirala("b", "a"), Konusma.Sirala("a", "b"));
    }

    [Fact]
    public void Hobiler_sadece_listedekileri_kabul_eder()
    {
        Assert.Equal("Spor;Müzik", Hobiler.Birlestir(new[] { "Spor", "Uydurma hobi", "Müzik", "Spor" }));
        Assert.Null(Hobiler.Birlestir(Array.Empty<string>()));
        Assert.Equal(new[] { "Spor", "Müzik" }, Hobiler.Ayir("Spor; Müzik"));
    }
}
