using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;
using RooMee.Services;

namespace RooMee.Controllers;

[Authorize]
public class KayitliAramaController : Controller
{
    private const int MaxArama = 10;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public KayitliAramaController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    private string BenimId => _userManager.GetUserId(User)!;

    // Kayıtlı aramalarım: her biri için son bakıştan sonra eklenen ilan sayısı
    public async Task<IActionResult> Index()
    {
        var aramalar = await _db.KayitliAramalar
            .Where(a => a.KullaniciId == BenimId)
            .OrderByDescending(a => a.OlusturmaTarihi)
            .ToListAsync();

        var liste = new List<KayitliAramaOzetViewModel>();
        foreach (var a in aramalar)
        {
            var q = IlanFiltresi.Uygula(_db.Ilanlar.Where(i => i.Aktif && i.KullaniciId != a.KullaniciId), a.Kriter);
            liste.Add(new KayitliAramaOzetViewModel
            {
                Arama = a,
                Toplam = await q.CountAsync(),
                Yeni = await q.CountAsync(i => i.OlusturmaTarihi > a.SonBakis)
            });
        }
        return View(liste);
    }

    // İlanlar sayfasındaki "Bu aramayı kaydet" formu
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Kaydet([FromForm] IlanAramaKriteri kriter, string? ad)
    {
        if (kriter.BosMu)
        {
            TempData["Mesaj"] = "Kaydetmek için önce en az bir filtre seç.";
            return RedirectToAction("Index", "Ilan");
        }
        if (await _db.KayitliAramalar.CountAsync(a => a.KullaniciId == BenimId) >= MaxArama)
        {
            TempData["Mesaj"] = $"En fazla {MaxArama} arama kaydedebilirsin. Önce eski bir aramayı sil.";
            return RedirectToAction("Index", "Ilan", kriter.RotaDegerleri());
        }

        var arama = new KayitliArama
        {
            KullaniciId = BenimId,
            Ad = string.IsNullOrWhiteSpace(ad) ? kriter.Ozet() : ad.Trim(),
            Kriter = kriter,
        };
        if (arama.Ad.Length > 80) arama.Ad = arama.Ad[..80];
        _db.KayitliAramalar.Add(arama);
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Arama kaydedildi. Uyan yeni bir ilan eklenince sana e-posta ile haber vereceğiz.";
        return RedirectToAction("Index", "Ilan", kriter.RotaDegerleri());
    }

    // Aramayı aç: sonuçlara git ve "yeni" sayısını sıfırla
    public async Task<IActionResult> Ac(int id)
    {
        var a = await _db.KayitliAramalar.FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == BenimId);
        if (a == null) return NotFound();
        a.SonBakis = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return RedirectToAction("Index", "Ilan", a.Kriter.RotaDegerleri());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Uyari(int id)
    {
        var a = await _db.KayitliAramalar.FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == BenimId);
        if (a == null) return NotFound();
        a.EpostaUyarisi = !a.EpostaUyarisi;
        await _db.SaveChangesAsync();
        TempData["Mesaj"] = a.EpostaUyarisi ? "E-posta uyarısı açıldı." : "E-posta uyarısı kapatıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var a = await _db.KayitliAramalar.FirstOrDefaultAsync(x => x.Id == id && x.KullaniciId == BenimId);
        if (a == null) return NotFound();
        _db.KayitliAramalar.Remove(a);
        await _db.SaveChangesAsync();
        TempData["Mesaj"] = "Kayıtlı arama silindi.";
        return RedirectToAction(nameof(Index));
    }
}
