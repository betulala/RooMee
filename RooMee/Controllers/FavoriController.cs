using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;

namespace RooMee.Controllers;

[Authorize]
public class FavoriController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FavoriController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Favorilerim sayfası
    public async Task<IActionResult> Index()
    {
        var benimId = _userManager.GetUserId(User);
        var favoriler = await _db.Favoriler
            .Where(f => f.KullaniciId == benimId)
            .Include(f => f.Ilan!).ThenInclude(i => i.Kullanici)
            .Include(f => f.Ilan!).ThenInclude(i => i.Fotolar)
            .OrderByDescending(f => f.Tarih)
            .ToListAsync();
        return View(favoriler);
    }

    // Favoriye ekle / çıkar (aynı buton)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Degistir(int ilanId, string? returnUrl)
    {
        var benimId = _userManager.GetUserId(User)!;
        var ilan = await _db.Ilanlar.FirstOrDefaultAsync(i => i.Id == ilanId);
        if (ilan == null) return NotFound();

        var mevcut = await _db.Favoriler
            .FirstOrDefaultAsync(f => f.KullaniciId == benimId && f.IlanId == ilanId);

        if (mevcut != null)
        {
            _db.Favoriler.Remove(mevcut);
            TempData["Mesaj"] = "İlan favorilerinden çıkarıldı.";
        }
        else if (ilan.KullaniciId == benimId)
        {
            TempData["Mesaj"] = "Kendi ilanını favorilere ekleyemezsin.";
        }
        else if (!ilan.Aktif)
        {
            TempData["Mesaj"] = "Yayında olmayan bir ilan favorilere eklenemez.";
        }
        else
        {
            _db.Favoriler.Add(new Favori { KullaniciId = benimId, IlanId = ilanId });
            TempData["Mesaj"] = "İlan favorilerine eklendi.";
        }

        await _db.SaveChangesAsync();

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        return RedirectToAction("Detay", "Ilan", new { id = ilanId });
    }
}