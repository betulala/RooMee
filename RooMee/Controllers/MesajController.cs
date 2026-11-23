using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Controllers;

[Authorize]
public class MesajController : Controller
{
    private const int MaxMetin = 1000;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MesajController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private string BenimId => _userManager.GetUserId(User)!;

    // Mesajlarım: tüm konuşmalar, en son mesaj en üstte
    public async Task<IActionResult> Index()
    {
        var benimId = BenimId;
        var konusmalar = await _db.Konusmalar
            .Where(k => k.Kullanici1Id == benimId || k.Kullanici2Id == benimId)
            .Where(k => k.Mesajlar.Any())
            .Include(k => k.Kullanici1).ThenInclude(u => u!.Profil)
            .Include(k => k.Kullanici2).ThenInclude(u => u!.Profil)
            .Include(k => k.Ilan)
            .OrderByDescending(k => k.SonMesajTarihi)
            .Select(k => new KonusmaOzetViewModel
            {
                Konusma = k,
                SonMesaj = k.Mesajlar.OrderByDescending(m => m.Tarih).First(),
                Okunmamis = k.Mesajlar.Count(m => m.GonderenId != benimId && !m.Okundu)
            })
            .ToListAsync();

        ViewBag.BenimId = benimId;
        return View(konusmalar);
    }

    // Bir konuşmanın mesajları
    public async Task<IActionResult> Konusma(int id)
    {
        var benimId = BenimId;
        var konusma = await _db.Konusmalar
            .Include(k => k.Kullanici1).ThenInclude(u => u!.Profil)
            .Include(k => k.Kullanici2).ThenInclude(u => u!.Profil)
            .Include(k => k.Ilan)
            .Include(k => k.Mesajlar.OrderBy(m => m.Tarih))
            .FirstOrDefaultAsync(k => k.Id == id);

        // Başkasının konuşmasını açmaya çalışırsa yok say
        if (konusma == null || (konusma.Kullanici1Id != benimId && konusma.Kullanici2Id != benimId))
            return NotFound();

        // Bana gelen okunmamış mesajları okundu yap
        await _db.Mesajlar
            .Where(m => m.KonusmaId == id && m.GonderenId != benimId && !m.Okundu)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.Okundu, true));

        ViewBag.BenimId = benimId;
        return View(konusma);
    }

    // "İletişime geç" / "Mesaj gönder" butonu buraya gelir
    public async Task<IActionResult> Yeni(string kullaniciId, int? ilanId)
    {
        var benimId = BenimId;
        if (kullaniciId == benimId) return RedirectToAction(nameof(Index));

        var karsi = await _db.Users.Include(u => u.Profil).FirstOrDefaultAsync(u => u.Id == kullaniciId);
        if (karsi == null) return NotFound();

        // Zaten konuşma varsa oraya git
        var (k1, k2) = Models.Konusma.Sirala(benimId, kullaniciId);
        var mevcut = await _db.Konusmalar.FirstOrDefaultAsync(k => k.Kullanici1Id == k1 && k.Kullanici2Id == k2);
        if (mevcut != null) return RedirectToAction(nameof(Konusma), new { id = mevcut.Id });

        Ilan? ilan = null;
        if (ilanId.HasValue)
            ilan = await _db.Ilanlar.FirstOrDefaultAsync(i => i.Id == ilanId && i.KullaniciId == kullaniciId);

        ViewBag.Karsi = karsi;
        ViewBag.Ilan = ilan;
        return View();
    }

    // İlk mesajı gönderir ve konuşmayı oluşturur
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Yeni(string kullaniciId, int? ilanId, string? metin)
    {
        var benimId = BenimId;
        if (kullaniciId == benimId) return RedirectToAction(nameof(Index));
        if (!await _db.Users.AnyAsync(u => u.Id == kullaniciId)) return NotFound();

        metin = metin?.Trim();
        if (string.IsNullOrEmpty(metin))
        {
            TempData["Hata"] = "Boş mesaj gönderilemez.";
            return RedirectToAction(nameof(Yeni), new { kullaniciId, ilanId });
        }
        if (metin.Length > MaxMetin) metin = metin[..MaxMetin];

        var (k1, k2) = Models.Konusma.Sirala(benimId, kullaniciId);
        var konusma = await _db.Konusmalar.FirstOrDefaultAsync(k => k.Kullanici1Id == k1 && k.Kullanici2Id == k2);
        if (konusma == null)
        {
            if (ilanId.HasValue && !await _db.Ilanlar.AnyAsync(i => i.Id == ilanId && i.KullaniciId == kullaniciId))
                ilanId = null;

            konusma = new Konusma { Kullanici1Id = k1, Kullanici2Id = k2, IlanId = ilanId };
            _db.Konusmalar.Add(konusma);
        }

        konusma.Mesajlar.Add(new Mesaj { GonderenId = benimId, Metin = metin });
        konusma.SonMesajTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Konusma), new { id = konusma.Id });
    }

    // Var olan konuşmaya mesaj gönderir
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Gonder(int konusmaId, string? metin)
    {
        var benimId = BenimId;
        var konusma = await _db.Konusmalar.FirstOrDefaultAsync(k => k.Id == konusmaId);
        if (konusma == null || (konusma.Kullanici1Id != benimId && konusma.Kullanici2Id != benimId))
            return NotFound();

        metin = metin?.Trim();
        if (!string.IsNullOrEmpty(metin))
        {
            if (metin.Length > MaxMetin) metin = metin[..MaxMetin];
            _db.Mesajlar.Add(new Mesaj { KonusmaId = konusmaId, GonderenId = benimId, Metin = metin });
            konusma.SonMesajTarihi = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Konusma), new { id = konusmaId });
    }
}
