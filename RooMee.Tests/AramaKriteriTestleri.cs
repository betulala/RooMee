using RooMee.Models;
using RooMee.Services;

namespace RooMee.Tests;

// Kayıtlı aramalarda kullanılan kriter nesnesi (özet metni, adres değerleri, JSON saklama)
public class AramaKriteriTestleri
{
    [Fact]
    public void Hic_filtre_yoksa_kriter_bos_sayilir()
    {
        Assert.True(new IlanAramaKriteri().BosMu);
        Assert.False(new IlanAramaKriteri { Sessiz = true }.BosMu);
    }

    [Fact]
    public void Ozet_okunur_bir_metin_uretir()
    {
        var k = new IlanAramaKriteri { Sehir = "Konya", KiraMax = 5000, OdaTipi = OdaTipi.TekKisilik, SigaraIcmeyen = true };
        Assert.Equal("Konya · 5.000₺'ye kadar · Tek kişilik oda · sigara içmeyen", k.Ozet());
    }

    [Fact]
    public void Rota_degerleri_sadece_dolu_alanlari_icerir()
    {
        var d = new IlanAramaKriteri { Sehir = "Konya", KiraMin = 2000 }.RotaDegerleri();
        Assert.Equal(2, d.Count);
        Assert.Equal("Konya", d["Sehir"]);
        Assert.Equal("2000", d["KiraMin"]);
    }

    [Fact]
    public void Kayitli_arama_kriterleri_json_olarak_saklanip_geri_okunur()
    {
        var arama = new KayitliArama
        {
            Kriter = new IlanAramaKriteri { Sehir = "İzmir", KiraMax = 7000, OdaTipi = OdaTipi.OrtakOda, SadeceDogrulanmis = true }
        };

        var geri = arama.Kriter;

        Assert.Equal("İzmir", geri.Sehir);
        Assert.Equal(7000, geri.KiraMax);
        Assert.Equal(OdaTipi.OrtakOda, geri.OdaTipi);
        Assert.True(geri.SadeceDogrulanmis);
    }
}
