using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Services;

// Yeni bir ilan yayınlandığında, ona uyan kayıtlı aramaların sahiplerine e-posta gönderir.
public class AramaUyarilari
{
    private readonly ApplicationDbContext _db;
    private readonly Bildirimci _bildirimci;

    public AramaUyarilari(ApplicationDbContext db, Bildirimci bildirimci)
    {
        _db = db;
        _bildirimci = bildirimci;
    }

    public async Task YeniIlanIcinAsync(int ilanId, Func<Ilan, string> ilanAdresi, Func<KayitliArama, string> aramaAdresi)
    {
        var ilan = await _db.Ilanlar.Include(i => i.Kullanici).FirstOrDefaultAsync(i => i.Id == ilanId && i.Aktif);
        if (ilan == null) return;

        // İlan sahibinin kendi aramaları hariç
        var aramalar = await _db.KayitliAramalar
            .Include(a => a.Kullanici)
            .Where(a => a.EpostaUyarisi && a.KullaniciId != ilan.KullaniciId)
            .ToListAsync();

        // Bir kişiye, birden çok araması uysa da tek e-posta gitsin
        var bildirilenler = new HashSet<string>();
        foreach (var arama in aramalar)
        {
            if (bildirilenler.Contains(arama.KullaniciId) || arama.Kullanici == null) continue;

            var uyuyor = await IlanFiltresi.Uygula(_db.Ilanlar.Where(i => i.Id == ilan.Id), arama.Kriter).AnyAsync();
            if (!uyuyor) continue;

            bildirilenler.Add(arama.KullaniciId);
            await _bildirimci.YeniIlanlarAsync(arama.Kullanici, arama.Ad, new List<Ilan> { ilan }, ilanAdresi, aramaAdresi(arama));
        }
    }
}
