using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RooMee.Models;

namespace RooMee.Data;

// Geliştirme için örnek kullanıcılar ve ilanlar.
// Çalıştırmak için:  dotnet run --project RooMee -- --test-verisi
// Tekrar çalıştırılırsa var olan test kullanıcılarını atlar, iki kez eklemez.
// Test hesaplarının hepsinin şifresi: TestSifre123  (e-postalar @roomee.test ile biter)
public static class TestVerisi
{
    public const string Sifre = "TestSifre123";

    private record Kisi(string Ad, string Soyad, string Eposta, bool Ogrenci, string Universite, string Bolum,
        int ButceMin, int ButceMax, bool Sigara, bool Evcil, bool Sessiz, string Hakkinda);

    private record IlanTaslak(string Baslik, string Sehir, string Ilce, int Kira, OdaTipi Tip,
        bool SadeceOgrenci, bool SigaraIcmeyen, bool Sessiz, bool Evcil, string Aciklama);

    private static readonly (Kisi Kisi, IlanTaslak[] Ilanlar)[] Veriler =
    {
        (new Kisi("Mert", "Polat", "mert@roomee.test", true, "ODTÜ", "Mimarlık", 5000, 8000, false, true, false,
            "Mimarlık 3. sınıf öğrencisiyim. Düzenli ve sosyal biriyim, evde arkadaşlarla film gecesi severim."),
        new[]
        {
            new IlanTaslak("Çankaya'da 3+1 evde ortak oda", "Ankara", "Çankaya", 7500, OdaTipi.OrtakOda, true, true, false, true,
                "Kızılay'a yürüme mesafesinde, metroya 5 dakika. Evde 2 öğrenci kalıyoruz, üçüncü ev arkadaşımızı arıyoruz."),
            new IlanTaslak("ODTÜ'ye yakın tek kişilik oda", "Ankara", "Çankaya", 9000, OdaTipi.TekKisilik, true, false, false, true,
                "Eşyalı oda, internet ve aidat kiraya dahil. Kampüse otobüsle 10 dakika."),
            new IlanTaslak("Bahçelievler'de geniş oda", "Ankara", "Çankaya", 8200, OdaTipi.TekKisilik, false, true, true, false,
                "Sessiz bir apartman, balkonlu oda. Çalışan ya da yüksek lisans öğrencisi tercih edilir."),
        }),
        (new Kisi("Selin", "Kaya", "selin@roomee.test", true, "İstanbul Üniversitesi", "Hukuk", 8000, 12000, false, false, true,
            "Hukuk öğrencisiyim, sınav dönemlerinde sessiz bir ortamda çalışmayı seviyorum."),
        new[]
        {
            new IlanTaslak("Kadıköy Moda'da tek kişilik oda", "İstanbul", "Kadıköy", 12500, OdaTipi.TekKisilik, true, true, true, false,
                "Moda sahiline 5 dakika, eşyalı ve aydınlık oda. Kadın ev arkadaşı tercih edilir."),
            new IlanTaslak("Beşiktaş'ta ortak oda", "İstanbul", "Beşiktaş", 9500, OdaTipi.OrtakOda, true, true, false, false,
                "Metrobüs ve iskeleye yakın. Faturalar ortak bölünüyor."),
            new IlanTaslak("Üsküdar'da 2+1 ev bütünü", "İstanbul", "Üsküdar", 22000, OdaTipi.EvButunu, false, false, false, true,
                "Marmaray'a yürüme mesafesinde, yeni tadilatlı 2+1. İki kişilik ev arkadaşı grubuna uygun."),
        }),
        (new Kisi("Zeynep", "Han", "zeynep@roomee.test", false, "Ege Üniversitesi", "Eczacılık", 6000, 9000, false, true, false,
            "Eczane teknisyeni olarak çalışıyorum. Kedim var, hayvanları seven ev arkadaşı arıyorum."),
        new[]
        {
            new IlanTaslak("Bornova'da kedili evde oda", "İzmir", "Bornova", 7000, OdaTipi.TekKisilik, false, true, false, true,
                "Ege Üniversitesi kampüsüne yakın. Evde bir kedi var, hayvan sevgisi şart."),
            new IlanTaslak("Karşıyaka'da deniz manzaralı oda", "İzmir", "Karşıyaka", 9500, OdaTipi.TekKisilik, false, true, true, false,
                "Sahile 2 dakika, balkondan deniz görünüyor. Sakin bir ev arkadaşı arıyorum."),
            new IlanTaslak("Buca'da öğrenciye uygun ortak oda", "İzmir", "Buca", 5000, OdaTipi.OrtakOda, true, false, false, true,
                "DEÜ Buca kampüsüne yürüme mesafesi. Uygun fiyatlı, öğrenci evi."),
        }),
        (new Kisi("Yusuf", "Arslan", "yusuf@roomee.test", true, "Uludağ Üniversitesi", "Fizyoterapi", 3000, 5000, false, false, false,
            "Fizyoterapi öğrencisiyim, akşamları geç yatarım. Spor yapmayı seven biriyim."),
        new[]
        {
            new IlanTaslak("Nilüfer'de Görükle'ye yakın oda", "Bursa", "Nilüfer", 4500, OdaTipi.OrtakOda, true, true, false, false,
                "Uludağ Üniversitesi'ne 5 dakika. 3 öğrenci kalıyoruz, ortak mutfak ve salon."),
            new IlanTaslak("Osmangazi merkezde tek kişilik oda", "Bursa", "Osmangazi", 5500, OdaTipi.TekKisilik, false, true, false, false,
                "Metroya yakın, eşyalı oda. Çalışan ya da öğrenci olabilir."),
            new IlanTaslak("Selçuklu'da kampüse yakın oda", "Konya", "Selçuklu", 4000, OdaTipi.TekKisilik, true, true, true, false,
                "Selçuk Üniversitesi'ne tramvayla 10 dakika. Sessiz, ders çalışmaya uygun ev."),
        }),
        (new Kisi("Elif", "Demir", "elif@roomee.test", true, "Anadolu Üniversitesi", "Bilgisayar Mühendisliği", 4000, 6500, false, false, true,
            "Bilgisayar mühendisliği son sınıf öğrencisiyim. Erken yatar erken kalkarım, düzenli bir evde yaşamak istiyorum."),
        new[]
        {
            new IlanTaslak("Eskişehir Tepebaşı'nda öğrenci evi", "Eskişehir", "Tepebaşı", 5000, OdaTipi.OrtakOda, true, true, true, false,
                "Anadolu Üniversitesi'ne tramvayla 5 dakika. Temiz ve düzenli bir ev, kadın ev arkadaşı aranıyor."),
            new IlanTaslak("Odunpazarı'nda tarihi evde oda", "Eskişehir", "Odunpazarı", 6000, OdaTipi.TekKisilik, false, true, false, true,
                "Odunpazarı evlerinde, karakteristik ve aydınlık bir oda. Bahçe kullanımı var."),
            new IlanTaslak("Muratpaşa'da denize yakın oda", "Antalya", "Muratpaşa", 8500, OdaTipi.TekKisilik, false, false, false, true,
                "Konyaaltı sahiline yakın, klimalı oda. Yaz aylarında da kalınabilir."),
        }),
    };

    public static async Task YukleAsync(IServiceProvider servisler)
    {
        using var kapsam = servisler.CreateScope();
        var db = kapsam.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var kullanicilar = kapsam.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        var rastgele = new Random(42);
        int eklenenKisi = 0, eklenenIlan = 0;

        foreach (var (kisi, ilanlar) in Veriler)
        {
            if (await kullanicilar.FindByEmailAsync(kisi.Eposta) != null)
            {
                Console.WriteLine($"Atlandı (zaten var): {kisi.Eposta}");
                continue;
            }

            var k = new ApplicationUser
            {
                UserName = kisi.Eposta,
                Email = kisi.Eposta,
                EmailConfirmed = true,
                Ad = kisi.Ad,
                Soyad = kisi.Soyad,
                KvkkOnayTarihi = DateTime.UtcNow,
                OgrenciDogrulandi = kisi.Ogrenci,
                OgrenciEposta = kisi.Ogrenci ? $"{kisi.Ad.ToLowerInvariant()}@ogr.roomee.test" : null,
                OgrenciDogrulamaTarihi = kisi.Ogrenci ? DateTime.UtcNow : null,
            };
            var sonuc = await kullanicilar.CreateAsync(k, Sifre);
            if (!sonuc.Succeeded)
            {
                Console.WriteLine($"Oluşturulamadı: {kisi.Eposta} - {string.Join(", ", sonuc.Errors.Select(e => e.Description))}");
                continue;
            }
            eklenenKisi++;

            db.Profiller.Add(new Profil
            {
                KullaniciId = k.Id,
                Universite = kisi.Universite,
                Bolum = kisi.Bolum,
                ButceMin = kisi.ButceMin,
                ButceMax = kisi.ButceMax,
                YatisSaati = kisi.Sessiz ? new TimeOnly(23, 0) : new TimeOnly(1, 0),
                KalkisSaati = kisi.Sessiz ? new TimeOnly(7, 0) : new TimeOnly(9, 30),
                SigaraIciyor = kisi.Sigara,
                EvcilHayvanVar = kisi.Evcil,
                SessizOrtamIster = kisi.Sessiz,
                Hakkinda = kisi.Hakkinda,
            });

            foreach (var t in ilanlar)
            {
                db.Ilanlar.Add(new Ilan
                {
                    KullaniciId = k.Id,
                    Baslik = t.Baslik,
                    Sehir = t.Sehir,
                    Ilce = t.Ilce,
                    Kira = t.Kira,
                    OdaTipi = t.Tip,
                    SadeceOgrenci = t.SadeceOgrenci,
                    SigaraIcmeyen = t.SigaraIcmeyen,
                    Sessiz = t.Sessiz,
                    EvcilHayvanKabul = t.Evcil,
                    Aciklama = t.Aciklama,
                    // İlanlar son 3 haftaya dağılsın ki sıralama denenebilsin
                    OlusturmaTarihi = DateTime.UtcNow.AddHours(-rastgele.Next(1, 21 * 24)),
                });
                eklenenIlan++;
            }
            await db.SaveChangesAsync();
        }

        Console.WriteLine($"Test verisi yüklendi: {eklenenKisi} kullanıcı, {eklenenIlan} ilan.");
    }
}
