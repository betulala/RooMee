using RooMee.Models;
using RooMee.Services;

namespace RooMee.Tests;

// Ev arkadaşı uyum yüzdesi hesaplaması (Services/Uyum.cs)
public class UyumTestleri
{
    private static Profil TamProfil() => new()
    {
        Uyku = UykuDuzeni.Sabahci,
        CalismaDuzeni = CalismaDuzeni.EvdeSessiz,
        EvcilHayvan = EvcilHayvanDurumu.SorunDegil,
        SigaraIciyor = false,
        ButceMax = 5000,
        DogumYili = DateTime.Today.Year - 22,
        HobilerMetni = "Müzik;Kitap okuma;Spor",
    };

    [Fact]
    public void Ayni_tercihlere_sahip_iki_profil_yuzde_100_uyumludur()
    {
        var sonuc = Uyum.Hesapla(TamProfil(), TamProfil());

        Assert.NotNull(sonuc);
        Assert.Equal(100, sonuc!.Skor);
        Assert.All(sonuc.Satirlar, s => Assert.Equal("Uyumlu", s.Durum));
    }

    [Fact]
    public void Profillerden_biri_yoksa_uyum_hesaplanmaz()
    {
        Assert.Null(Uyum.Hesapla(null, TamProfil()));
        Assert.Null(Uyum.Hesapla(TamProfil(), null));
    }

    [Fact]
    public void Karsilastirilacak_yeterli_kriter_yoksa_uyum_hesaplanmaz()
    {
        // Sadece sigara bilgisi karşılaştırılabilir; yüzde anlamsız olacağı için null dönmeli
        Assert.Null(Uyum.Hesapla(new Profil(), new Profil()));
    }

    [Fact]
    public void Evcil_hayvan_istemeyen_ile_hayvan_sahibi_uyumsuzdur()
    {
        var ben = TamProfil();
        ben.EvcilHayvan = EvcilHayvanDurumu.Istemiyor;
        var o = TamProfil();
        o.EvcilHayvan = EvcilHayvanDurumu.Sahibi;

        var satir = Uyum.Hesapla(ben, o)!.Satirlar.Single(s => s.Ad == "Evcil hayvan");

        Assert.Equal(0, satir.Puan);
        Assert.Equal("Uyumsuz", satir.Durum);
    }

    [Fact]
    public void Uyku_duzeni_esnek_olan_kisi_kismen_uyumludur()
    {
        var ben = TamProfil();
        var o = TamProfil();
        o.Uyku = UykuDuzeni.Esnek;

        var satir = Uyum.Hesapla(ben, o)!.Satirlar.Single(s => s.Ad == "Uyku düzeni");

        Assert.Equal("Kısmen", satir.Durum);
    }

    [Fact]
    public void Ortak_hobiler_notta_listelenir()
    {
        var ben = TamProfil();
        var o = TamProfil();
        o.HobilerMetni = "Müzik;Oyun";

        var satir = Uyum.Hesapla(ben, o)!.Satirlar.Single(s => s.Ad == "Ortak hobiler");

        Assert.Equal("Müzik", satir.Not);
    }

    [Fact]
    public void Butce_farki_buyudukce_uyum_duser()
    {
        var ben = TamProfil();
        var yakin = TamProfil(); yakin.ButceMax = 5500;
        var uzak = TamProfil(); uzak.ButceMax = 12000;

        var yakinSkor = Uyum.Hesapla(ben, yakin)!.Skor;
        var uzakSkor = Uyum.Hesapla(ben, uzak)!.Skor;

        Assert.True(yakinSkor > uzakSkor, $"Yakın bütçe ({yakinSkor}) uzak bütçeden ({uzakSkor}) daha uyumlu olmalı");
    }
}
