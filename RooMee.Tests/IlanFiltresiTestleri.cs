using Microsoft.EntityFrameworkCore;
using RooMee.Models;
using RooMee.Services;

namespace RooMee.Tests;

// İlan arama filtreleri (Services/IlanFiltresi.cs): İlanlar sayfası ve kayıtlı aramalar bunu kullanır
public class IlanFiltresiTestleri : IDisposable
{
    private readonly TestVeritabani _vt = new();

    public IlanFiltresiTestleri()
    {
        using var db = _vt.YeniBaglam();
        var ali = TestVeritabani.Kullanici("Ali");
        var ayse = TestVeritabani.Kullanici("Ayse", dogrulanmis: true);
        var askida = TestVeritabani.Kullanici("Askida", askida: true);
        db.Users.AddRange(ali, ayse, askida);

        var a = TestVeritabani.Ilan(ali, "Konya ucuz", "Konya", 3000, OdaTipi.OrtakOda);
        var b = TestVeritabani.Ilan(ali, "Konya pahalı", "Konya", 9000, OdaTipi.TekKisilik);
        b.SigaraIcmeyen = true;
        var c = TestVeritabani.Ilan(ayse, "Ankara orta", "Ankara", 6000, OdaTipi.TekKisilik);
        c.SigaraIcmeyen = true;
        c.Sessiz = true;
        var d = TestVeritabani.Ilan(askida, "Askıdaki ilan", "Konya", 4000);
        db.Ilanlar.AddRange(a, b, c, d);
        db.SaveChanges();
    }

    public void Dispose() => _vt.Dispose();

    private List<string> Ara(IlanAramaKriteri k)
    {
        using var db = _vt.YeniBaglam();
        return IlanFiltresi.Uygula(db.Ilanlar.Where(i => i.Aktif), k)
            .OrderBy(i => i.Baslik).Select(i => i.Baslik).ToList();
    }

    [Fact]
    public void Bos_kriter_askidakiler_disindaki_tum_ilanlari_getirir()
    {
        var sonuc = Ara(new IlanAramaKriteri());
        Assert.Equal(new[] { "Ankara orta", "Konya pahalı", "Konya ucuz" }, sonuc);
    }

    [Fact]
    public void Askiya_alinmis_kullanicinin_ilani_hicbir_aramada_cikmaz()
    {
        Assert.DoesNotContain("Askıdaki ilan", Ara(new IlanAramaKriteri { Sehir = "Konya" }));
    }

    [Fact]
    public void Sehir_filtresi_sadece_o_sehri_getirir()
    {
        Assert.Equal(new[] { "Konya pahalı", "Konya ucuz" }, Ara(new IlanAramaKriteri { Sehir = "Konya" }));
    }

    [Fact]
    public void Kira_araligi_sinirlar_dahil_uygulanir()
    {
        Assert.Equal(new[] { "Ankara orta", "Konya ucuz" }, Ara(new IlanAramaKriteri { KiraMin = 3000, KiraMax = 6000 }));
    }

    [Fact]
    public void Oda_tipi_filtresi_calisir()
    {
        Assert.Equal(new[] { "Ankara orta", "Konya pahalı" }, Ara(new IlanAramaKriteri { OdaTipi = OdaTipi.TekKisilik }));
    }

    [Fact]
    public void Birden_fazla_kriter_birlikte_uygulanir()
    {
        Assert.Equal(new[] { "Ankara orta" }, Ara(new IlanAramaKriteri { SigaraIcmeyen = true, Sessiz = true }));
    }

    [Fact]
    public void Sadece_dogrulanmis_ogrenci_filtresi_ilan_sahibine_bakar()
    {
        Assert.Equal(new[] { "Ankara orta" }, Ara(new IlanAramaKriteri { SadeceDogrulanmis = true }));
    }
}
