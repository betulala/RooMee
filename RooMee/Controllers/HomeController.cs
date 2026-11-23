using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext db, Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Giriş yapan kişiye: profiline en uyumlu 3 ev arkadaşı ve şehrine/bütçesine uygun 3 ilan
    private async Task OnerileriHazirla()
    {
        var benimId = _userManager.GetUserId(User);
        if (benimId == null) return;

        var ben = await _db.Profiller.FirstOrDefaultAsync(p => p.KullaniciId == benimId);
        ViewBag.ProfilVar = ben != null && (ben.Uyku.HasValue || ben.CalismaDuzeni.HasValue || ben.Butce.HasValue);
        if (ben == null) return;

        // Uyumlu kişiler: uyum yüzdesine göre; aynı şehirdekilere +15 puan avantaj
        var adaylar = await _db.Users
            .Include(u => u.Profil)
            .Where(u => u.Id != benimId && u.Profil != null && u.Profil.AramaTuru != null && u.Profil.Sehir != null)
            .ToListAsync();
        var kartlar = adaylar
            .Select(u => new KullaniciKartViewModel { Kullanici = u, Uyum = RooMee.Services.Uyum.Hesapla(ben, u.Profil)?.Skor })
            .Where(k => k.Uyum.HasValue)
            .OrderByDescending(k => k.Uyum + (ben.Sehir != null && k.Profil.Sehir == ben.Sehir ? 15 : 0))
            .Take(3)
            .ToList();
        ViewBag.OneriKisiler = kartlar;

        // Uygun ilanlar: şehrimde ve bütçemin (en fazla) içinde, en yeniler
        var q = _db.Ilanlar.Where(i => i.Aktif && i.KullaniciId != benimId);
        if (!string.IsNullOrEmpty(ben.Sehir)) q = q.Where(i => i.Sehir == ben.Sehir);
        if (ben.ButceMax.HasValue) q = q.Where(i => i.Kira <= ben.ButceMax.Value);
        ViewBag.OneriIlanlar = await q
            .Include(i => i.Kullanici)
            .Include(i => i.Fotolar)
            .OrderByDescending(i => i.OlusturmaTarihi)
            .Take(3)
            .ToListAsync();
        ViewBag.OneriSehir = ben.Sehir;
        ViewBag.OneriButce = ben.ButceMax;
    }

    public async Task<IActionResult> Index()
    {
        // Ana sayfada gösterilecek en yeni ilanlar
        var sonIlanlar = await _db.Ilanlar
            .Where(i => i.Aktif)
            .Include(i => i.Kullanici)
            .Include(i => i.Fotolar)
            .OrderByDescending(i => i.OlusturmaTarihi)
            .Take(6)
            .ToListAsync();

        ViewBag.Sehirler = await _db.Ilanlar
            .Where(i => i.Aktif)
            .Select(i => i.Sehir)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        await OnerileriHazirla();
        return View(sonIlanlar);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Hakkimizda() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
