using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RooMee.Models;

namespace RooMee.Data;

// Geliştirme için örnek kullanıcılar, ev arkadaşı profilleri ve ilanlar.
// Çalıştırmak için (uygulama kapalıyken):  dotnet run --project RooMee -- --test-verisi
// Tekrar çalıştırılırsa: var olan test kullanıcılarının profilini günceller, ilanları ikinci kez eklemez.
// Test hesaplarının hepsinin şifresi: TestSifre123  (e-postalar @roomee.test ile biter)
public static class TestVerisi
{
    public const string Sifre = "TestSifre123";

    private record Kisi(
        string Ad, string Soyad, string Eposta, Cinsiyet Cinsiyet, int DogumYili, string Sehir, string Ilce,
        bool Ogrenci, bool Calisiyor, string? Meslek, string? Universite, string? Bolum, string? Sinif, bool Dogrulanmis,
        AramaTuru Tur, TercihOdaTipi Oda, string? EvTipi, int? BosOda, int ButceMin, int ButceMax,
        UykuDuzeni Uyku, CalismaDuzeni Calisma, EvcilHayvanDurumu Hayvan, bool Sigara, string Hobiler, string Hakkinda);

    private record IlanTaslak(string Baslik, string Sehir, string Ilce, int Kira, OdaTipi Tip,
        bool SadeceOgrenci, bool SigaraIcmeyen, bool Sessiz, bool Evcil, string Aciklama);

    private static readonly (Kisi Kisi, IlanTaslak[] Ilanlar)[] Veriler =
    {
        (new Kisi("Mert", "Polat", "mert@roomee.test", Cinsiyet.Erkek, 2004, "Ankara", "Çankaya",
            true, false, null, "ODTÜ", "Mimarlık", "3. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.Paylasimli, null, null, 5000, 7500,
            UykuDuzeni.Esnek, CalismaDuzeni.Esnek, EvcilHayvanDurumu.SorunDegil, false, "Spor;Film / Dizi;Fotoğrafçılık",
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
        (new Kisi("Selin", "Kaya", "selin@roomee.test", Cinsiyet.Kadin, 2003, "İstanbul", "Kadıköy",
            true, false, null, "İstanbul Üniversitesi", "Hukuk", "4. sınıf", true,
            AramaTuru.KiralikEv, TercihOdaTipi.TekKisilik, "3+1", 1, 9000, 12500,
            UykuDuzeni.Sabahci, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.Istemiyor, false, "Kitap okuma;Müzik;Seyahat",
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
        (new Kisi("Zeynep", "Han", "zeynep@roomee.test", Cinsiyet.Kadin, 2000, "İzmir", "Bornova",
            false, true, "Eczane teknisyeni", null, null, null, false,
            AramaTuru.KiralikEv, TercihOdaTipi.Studyo, "2+1", 1, 6000, 9000,
            UykuDuzeni.Esnek, CalismaDuzeni.Disarida, EvcilHayvanDurumu.Sahibi, false, "Yemek yapma;Doğa yürüyüşü",
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
        (new Kisi("Yusuf", "Arslan", "yusuf@roomee.test", Cinsiyet.Erkek, 2006, "Bursa", "Nilüfer",
            true, false, null, "Uludağ Üniversitesi", "Fizyoterapi", "1. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.Paylasimli, null, null, 3000, 4500,
            UykuDuzeni.Gececi, CalismaDuzeni.Esnek, EvcilHayvanDurumu.SorunDegil, false, "Spor;Oyun;Müzik",
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
        (new Kisi("Elif", "Demir", "elif@roomee.test", Cinsiyet.Kadin, 2002, "Eskişehir", "Tepebaşı",
            true, true, "Yarı zamanlı yazılımcı", "Anadolu Üniversitesi", "Bilgisayar Mühendisliği", "4. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.TekKisilik, null, null, 4000, 6500,
            UykuDuzeni.Sabahci, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.SorunDegil, false, "Teknoloji;Kitap okuma;Oyun",
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

        // ---- Sadece ev arkadaşı profili olanlar (ilanı yok) ----
        (new Kisi("Ayşe", "Yıldız", "ayse@roomee.test", Cinsiyet.Kadin, 2005, "Konya", "Selçuklu",
            true, false, null, "Selçuk Üniversitesi", "Hemşirelik", "2. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.Paylasimli, null, null, 3000, 4500,
            UykuDuzeni.Sabahci, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.Istemiyor, false, "Kitap okuma;Yemek yapma",
            "Hemşirelik öğrencisiyim, nöbet dönemlerinde erken kalkıyorum. Temiz ve sessiz bir ev arıyorum."), Array.Empty<IlanTaslak>()),
        (new Kisi("Can", "Öztürk", "can@roomee.test", Cinsiyet.Erkek, 2003, "Konya", "Meram",
            true, true, "Kafe çalışanı", "Necmettin Erbakan Üniversitesi", "İşletme", "3. sınıf", false,
            AramaTuru.KiralikEv, TercihOdaTipi.Paylasimli, "3+1", 2, 3500, 4000,
            UykuDuzeni.Gececi, CalismaDuzeni.Disarida, EvcilHayvanDurumu.SorunDegil, true, "Müzik;Oyun;Film / Dizi",
            "Meram'daki 3+1 evimde 2 boş oda var. Akşamları çalışıyorum, gündüzleri genelde evde olmam."), Array.Empty<IlanTaslak>()),
        (new Kisi("Deniz", "Şahin", "deniz@roomee.test", Cinsiyet.Kadin, 2004, "Ankara", "Keçiören",
            true, false, null, "Hacettepe Üniversitesi", "Psikoloji", "3. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.TekKisilik, null, null, 6000, 8000,
            UykuDuzeni.Sabahci, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.SorunDegil, false, "Kitap okuma;Müzik;Seyahat;Film / Dizi",
            "Psikoloji öğrencisiyim. Kitap okumayı, yürüyüş yapmayı ve sakin akşamları severim."), Array.Empty<IlanTaslak>()),
        (new Kisi("Emre", "Koç", "emre@roomee.test", Cinsiyet.Erkek, 1999, "İstanbul", "Ataşehir",
            false, true, "Yazılım geliştirici", null, null, null, false,
            AramaTuru.KiralikEv, TercihOdaTipi.TekKisilik, "2+1", 1, 14000, 16000,
            UykuDuzeni.Esnek, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.Sahibi, false, "Teknoloji;Oyun;Doğa yürüyüşü",
            "Evden çalışan bir yazılımcıyım. Köpeğim var, gündüzleri sessiz bir ortam benim için önemli."), Array.Empty<IlanTaslak>()),
        (new Kisi("Buse", "Aydın", "buse@roomee.test", Cinsiyet.Kadin, 2006, "İzmir", "Buca",
            true, false, null, "Dokuz Eylül Üniversitesi", "Müzik Öğretmenliği", "1. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.Paylasimli, null, null, 4000, 5500,
            UykuDuzeni.Gececi, CalismaDuzeni.Esnek, EvcilHayvanDurumu.SorunDegil, false, "Müzik;Fotoğrafçılık;Seyahat",
            "Müzik öğretmenliği okuyorum, evde zaman zaman piyano çalışırım. Sosyal ve neşeli biriyim."), Array.Empty<IlanTaslak>()),
        (new Kisi("Burak", "Çelik", "burak@roomee.test", Cinsiyet.Erkek, 2002, "Eskişehir", "Odunpazarı",
            true, false, null, "Eskişehir Osmangazi Üniversitesi", "Makine Mühendisliği", "4. sınıf", true,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.Paylasimli, null, null, 3500, 5000,
            UykuDuzeni.Esnek, CalismaDuzeni.Disarida, EvcilHayvanDurumu.Istemiyor, true, "Spor;Oyun",
            "Makine mühendisliği son sınıftayım. Günün çoğu kampüste geçiyor, evde fazla vakit geçirmem."), Array.Empty<IlanTaslak>()),
        (new Kisi("Merve", "Kurt", "merve@roomee.test", Cinsiyet.Kadin, 2001, "Antalya", "Konyaaltı",
            false, true, "Otel resepsiyonisti", null, null, null, false,
            AramaTuru.KiralikEv, TercihOdaTipi.Studyo, "1+1", 1, 9000, 11000,
            UykuDuzeni.Gececi, CalismaDuzeni.Disarida, EvcilHayvanDurumu.SorunDegil, false, "Seyahat;Yemek yapma;Doğa yürüyüşü",
            "Vardiyalı çalışıyorum. Konyaaltı'ndaki evimi paylaşabileceğim, düzenli bir ev arkadaşı arıyorum."), Array.Empty<IlanTaslak>()),
        (new Kisi("Kerem", "Aksoy", "kerem@roomee.test", Cinsiyet.Erkek, 2005, "Bursa", "Osmangazi",
            true, false, null, "Bursa Teknik Üniversitesi", "Elektrik Mühendisliği", "2. sınıf", false,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.TekKisilik, null, null, 4000, 6000,
            UykuDuzeni.Sabahci, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.SorunDegil, false, "Teknoloji;Spor;Kitap okuma",
            "Elektrik mühendisliği öğrencisiyim. Sabahları spor yapar, akşamları ders çalışırım."), Array.Empty<IlanTaslak>()),
        (new Kisi("İrem", "Güneş", "irem@roomee.test", Cinsiyet.Kadin, 2003, "Ankara", "Çankaya",
            true, true, "Kütüphane asistanı", "Ankara Üniversitesi", "Tarih", "3. sınıf", true,
            AramaTuru.KiralikEv, TercihOdaTipi.TekKisilik, "3+1", 1, 6500, 7500,
            UykuDuzeni.Sabahci, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.Sahibi, false, "Kitap okuma;Fotoğrafçılık;Film / Dizi",
            "Çankaya'daki evimde bir oda boşalıyor. Bir kedim var; sakin, kitap seven bir ev arkadaşı arıyorum."), Array.Empty<IlanTaslak>()),
        (new Kisi("Oğuz", "Yılmaz", "oguz@roomee.test", Cinsiyet.Erkek, 2000, "İzmir", "Karşıyaka",
            false, true, "Grafik tasarımcı", null, null, null, false,
            AramaTuru.EvArkadasiAriyor, TercihOdaTipi.TekKisilik, null, null, 7000, 10000,
            UykuDuzeni.Gececi, CalismaDuzeni.EvdeSessiz, EvcilHayvanDurumu.SorunDegil, true, "Fotoğrafçılık;Müzik;Teknoloji",
            "Serbest çalışan grafik tasarımcıyım, çoğunlukla evden çalışıyorum. Balkonda sigara içerim."), Array.Empty<IlanTaslak>()),
    };

    public static async Task YukleAsync(IServiceProvider servisler)
    {
        using var kapsam = servisler.CreateScope();
        var db = kapsam.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var kullanicilar = kapsam.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.MigrateAsync();

        var rastgele = new Random(42);
        int eklenenKisi = 0, guncellenenKisi = 0, eklenenIlan = 0;

        foreach (var (kisi, ilanlar) in Veriler)
        {
            var k = await kullanicilar.FindByEmailAsync(kisi.Eposta);
            var yeni = k == null;
            if (yeni)
            {
                k = new ApplicationUser
                {
                    UserName = kisi.Eposta,
                    Email = kisi.Eposta,
                    EmailConfirmed = true,
                    Ad = kisi.Ad,
                    Soyad = kisi.Soyad,
                    KvkkOnayTarihi = DateTime.UtcNow,
                };
            }

            k!.OgrenciDogrulandi = kisi.Dogrulanmis;
            k.OgrenciEposta = kisi.Dogrulanmis ? $"{Kucuk(kisi.Ad)}@ogr.roomee.test" : null;
            k.OgrenciDogrulamaTarihi = kisi.Dogrulanmis ? DateTime.UtcNow : null;

            if (yeni)
            {
                var sonuc = await kullanicilar.CreateAsync(k, Sifre);
                if (!sonuc.Succeeded)
                {
                    Console.WriteLine($"Oluşturulamadı: {kisi.Eposta} - {string.Join(", ", sonuc.Errors.Select(e => e.Description))}");
                    continue;
                }
                eklenenKisi++;
            }
            else
            {
                await kullanicilar.UpdateAsync(k);
                guncellenenKisi++;
            }

            var p = await db.Profiller.FirstOrDefaultAsync(x => x.KullaniciId == k.Id);
            if (p == null)
            {
                p = new Profil { KullaniciId = k.Id };
                db.Profiller.Add(p);
            }
            p.Cinsiyet = kisi.Cinsiyet;
            p.DogumYili = kisi.DogumYili;
            p.Sehir = kisi.Sehir;
            p.Ilce = kisi.Ilce;
            p.Ogrenci = kisi.Ogrenci;
            p.Calisiyor = kisi.Calisiyor;
            p.Meslek = kisi.Meslek;
            p.Universite = kisi.Universite;
            p.Bolum = kisi.Bolum;
            p.Sinif = kisi.Sinif;
            p.AramaTuru = kisi.Tur;
            p.TercihOdaTipi = kisi.Oda;
            p.EvTipi = kisi.EvTipi;
            p.BosOda = kisi.BosOda;
            p.ButceMin = kisi.ButceMin;
            p.ButceMax = kisi.ButceMax;
            p.Uyku = kisi.Uyku;
            p.YatisSaati = kisi.Uyku == UykuDuzeni.Sabahci ? new TimeOnly(23, 0) : kisi.Uyku == UykuDuzeni.Gececi ? new TimeOnly(2, 0) : null;
            p.KalkisSaati = kisi.Uyku == UykuDuzeni.Sabahci ? new TimeOnly(7, 0) : kisi.Uyku == UykuDuzeni.Gececi ? new TimeOnly(10, 0) : null;
            p.CalismaDuzeni = kisi.Calisma;
            p.EvcilHayvan = kisi.Hayvan;
            p.SigaraIciyor = kisi.Sigara;
            p.HobilerMetni = kisi.Hobiler;
            p.Hakkinda = kisi.Hakkinda;
            // "En yeni" sıralaması denenebilsin diye güncelleme tarihleri son 3 haftaya dağılır
            p.GuncellemeTarihi = DateTime.UtcNow.AddHours(-rastgele.Next(1, 21 * 24));

            if (yeni)
            {
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
                        OlusturmaTarihi = DateTime.UtcNow.AddHours(-rastgele.Next(1, 21 * 24)),
                    });
                    eklenenIlan++;
                }
            }
            await db.SaveChangesAsync();
        }

        Console.WriteLine($"Test verisi yüklendi: {eklenenKisi} yeni kullanıcı, {guncellenenKisi} güncellenen kullanıcı, {eklenenIlan} ilan.");
    }

    private static string Kucuk(string s) => s.ToLower(new System.Globalization.CultureInfo("tr-TR"))
        .Replace('ş', 's').Replace('ı', 'i').Replace('ğ', 'g').Replace('ü', 'u').Replace('ö', 'o').Replace('ç', 'c');
}
