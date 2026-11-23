using Microsoft.EntityFrameworkCore;
using RooMee.Data;

namespace RooMee.Services;

// Hesap silinmeden önce, veritabanında zincirleme (cascade) silinmeyen kayıtları temizler.
// Profil, ilanlar, ilan fotoğrafları ve kayıtlı aramalar kullanıcıyla birlikte otomatik silinir;
// başka kullanıcılarla bağlantılı kayıtlar (favori, şikayet, değerlendirme, konuşma) burada elle silinir.
public static class HesapSilme
{
    public static async Task VerileriTemizleAsync(ApplicationDbContext db, string kullaniciId)
    {
        await db.Favoriler.Where(f => f.KullaniciId == kullaniciId).ExecuteDeleteAsync();
        await db.Sikayetler
            .Where(s => s.SikayetEdenId == kullaniciId || s.SikayetEdilenKullaniciId == kullaniciId)
            .ExecuteDeleteAsync();
        await db.Degerlendirmeler
            .Where(d => d.YazanId == kullaniciId || d.HakkindaId == kullaniciId)
            .ExecuteDeleteAsync();
        // Konuşmaları sil (mesajlar zincirleme silinir)
        await db.Konusmalar
            .Where(x => x.Kullanici1Id == kullaniciId || x.Kullanici2Id == kullaniciId)
            .ExecuteDeleteAsync();
    }
}
