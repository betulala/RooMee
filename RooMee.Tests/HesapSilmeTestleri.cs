using Microsoft.EntityFrameworkCore;
using RooMee.Models;
using RooMee.Services;

namespace RooMee.Tests;

// Hesap silme (KVKK): kullanıcının tüm verileri silinmeli, başkalarının verileri kalmalı
public class HesapSilmeTestleri : IDisposable
{
    private readonly TestVeritabani _vt = new();
    public void Dispose() => _vt.Dispose();

    [Fact]
    public async Task Hesap_silinince_kullanicinin_tum_verileri_silinir_digerlerininki_kalir()
    {
        var silinecek = TestVeritabani.Kullanici("Silinecek");
        var diger = TestVeritabani.Kullanici("Diger");

        // ---- Örnek veriler ----
        using (var db = _vt.YeniBaglam())
        {
            db.Users.AddRange(silinecek, diger);
            db.Profiller.Add(new Profil { KullaniciId = silinecek.Id, Sehir = "Konya" });
            db.Profiller.Add(new Profil { KullaniciId = diger.Id, Sehir = "Ankara" });

            var benimIlanim = TestVeritabani.Ilan(silinecek, "Silinecek ilan", "Konya", 4000);
            var digerIlani = TestVeritabani.Ilan(diger, "Kalacak ilan", "Ankara", 5000);
            db.Ilanlar.AddRange(benimIlanim, digerIlani);
            await db.SaveChangesAsync();

            // Favoriler: benim favorim + başkasının benim ilanımı favorilemesi
            db.Favoriler.Add(new Favori { KullaniciId = silinecek.Id, IlanId = digerIlani.Id });
            db.Favoriler.Add(new Favori { KullaniciId = diger.Id, IlanId = benimIlanim.Id });

            // Konuşma ve mesajlar
            var (k1, k2) = Konusma.Sirala(silinecek.Id, diger.Id);
            var konusma = new Konusma { Kullanici1Id = k1, Kullanici2Id = k2 };
            konusma.Mesajlar.Add(new Mesaj { GonderenId = silinecek.Id, Metin = "Merhaba" });
            konusma.Mesajlar.Add(new Mesaj { GonderenId = diger.Id, Metin = "Selam" });
            db.Konusmalar.Add(konusma);

            // Şikayet, değerlendirme, kayıtlı arama
            db.Sikayetler.Add(new Sikayet { SikayetEdenId = diger.Id, SikayetEdilenKullaniciId = silinecek.Id, Neden = "Diğer" });
            db.Degerlendirmeler.Add(new Degerlendirme { YazanId = diger.Id, HakkindaId = silinecek.Id, Puan = 4 });
            db.KayitliAramalar.Add(new KayitliArama { KullaniciId = silinecek.Id, Ad = "Konya", Kriter = new IlanAramaKriteri { Sehir = "Konya" } });
            await db.SaveChangesAsync();
        }

        // ---- Hesap silme (ProfilController.HesapSil ile aynı adımlar) ----
        using (var db = _vt.YeniBaglam())
        {
            await HesapSilme.VerileriTemizleAsync(db, silinecek.Id);
            db.Users.Remove(await db.Users.SingleAsync(u => u.Id == silinecek.Id));
            await db.SaveChangesAsync();
        }

        // ---- Kontrol ----
        using (var db = _vt.YeniBaglam())
        {
            Assert.False(await db.Users.AnyAsync(u => u.Id == silinecek.Id));
            Assert.False(await db.Profiller.AnyAsync(p => p.KullaniciId == silinecek.Id));
            Assert.False(await db.Ilanlar.AnyAsync(i => i.KullaniciId == silinecek.Id));
            Assert.Empty(await db.Favoriler.ToListAsync());            // iki yöndeki favoriler de gitti
            Assert.Empty(await db.Konusmalar.ToListAsync());
            Assert.Empty(await db.Mesajlar.ToListAsync());
            Assert.Empty(await db.Sikayetler.ToListAsync());
            Assert.Empty(await db.Degerlendirmeler.ToListAsync());
            Assert.Empty(await db.KayitliAramalar.ToListAsync());

            // Diğer kullanıcının hesabı, profili ve ilanı yerinde
            Assert.True(await db.Users.AnyAsync(u => u.Id == diger.Id));
            Assert.True(await db.Profiller.AnyAsync(p => p.KullaniciId == diger.Id));
            Assert.True(await db.Ilanlar.AnyAsync(i => i.Baslik == "Kalacak ilan"));
        }
    }
}
