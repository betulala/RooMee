using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Controllers;

[Authorize]
public class DegerlendirmeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public DegerlendirmeController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // İki kişi birbirini, aralarında karşılıklı mesajlaşma olduysa değerlendirebilir
    public static async Task<bool> DegerlendirebilirMi(ApplicationDbContext db, string benimId, string onunId)
    {
        if (benimId == onunId) return false;
        var (k1, k2) = Konusma.Sirala(benimId, onunId);
        return await db.Konusmalar.AnyAsync(k => k.Kullanici1Id == k1 && k.Kullanici2Id == k2
            && k.Mesajlar.Any(m => m.GonderenId == benimId)
            && k.Mesajlar.Any(m => m.GonderenId == onunId));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Kaydet(string hakkindaId, int puan, string? yorum)
    {
        var benimId = _userManager.GetUserId(User)!;
        if (!await DegerlendirebilirMi(_db, benimId, hakkindaId))
        {
            TempData["Mesaj"] = "Sadece mesajlaştığın kişileri değerlendirebilirsin.";
            return RedirectToAction("Goruntule", "Profil", new { id = hakkindaId });
        }
        if (puan is < 1 or > 5)
        {
            TempData["Mesaj"] = "Puan 1 ile 5 yıldız arasında olmalı.";
            return RedirectToAction("Goruntule", "Profil", new { id = hakkindaId });
        }

        yorum = string.IsNullOrWhiteSpace(yorum) ? null : yorum.Trim();
        if (yorum?.Length > 500) yorum = yorum[..500];

        var d = await _db.Degerlendirmeler.FirstOrDefaultAsync(x => x.YazanId == benimId && x.HakkindaId == hakkindaId);
        if (d == null)
        {
            _db.Degerlendirmeler.Add(new Degerlendirme { YazanId = benimId, HakkindaId = hakkindaId, Puan = puan, Yorum = yorum });
            TempData["Mesaj"] = "Değerlendirmen kaydedildi, teşekkürler.";
        }
        else
        {
            d.Puan = puan;
            d.Yorum = yorum;
            d.Tarih = DateTime.UtcNow;
            TempData["Mesaj"] = "Değerlendirmen güncellendi.";
        }
        await _db.SaveChangesAsync();
        return Redirect(Url.Action("Goruntule", "Profil", new { id = hakkindaId }) + "#degerlendirmeler");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(string hakkindaId)
    {
        var benimId = _userManager.GetUserId(User)!;
        await _db.Degerlendirmeler.Where(x => x.YazanId == benimId && x.HakkindaId == hakkindaId).ExecuteDeleteAsync();
        TempData["Mesaj"] = "Değerlendirmen silindi.";
        return Redirect(Url.Action("Goruntule", "Profil", new { id = hakkindaId }) + "#degerlendirmeler");
    }
}
