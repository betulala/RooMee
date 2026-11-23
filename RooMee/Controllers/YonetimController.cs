using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Controllers;

// Yönetici paneli: genel bakış, kullanıcılar (askıya alma) ve ilanlar
[Authorize(Roles = Roller.Yonetici)]
public class YonetimController : Controller
{
    private const int SayfaBoyutu = 20;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public YonetimController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // ---------- Genel bakış ----------
    public async Task<IActionResult> Index()
    {
        var yediGunOnce = DateTime.UtcNow.AddDays(-7);
        var vm = new YonetimOzetViewModel
        {
            KullaniciSayisi = await _db.Users.CountAsync(),
            YeniKullanici7Gun = await _db.Users.CountAsync(u => u.KayitTarihi >= yediGunOnce),
            DogrulanmisOgrenci = await _db.Users.CountAsync(u => u.OgrenciDogrulandi),
            AskidakiKullanici = await _db.Users.CountAsync(u => u.AskiyaAlindi),
            AktifIlan = await _db.Ilanlar.CountAsync(i => i.Aktif),
            ToplamIlan = await _db.Ilanlar.CountAsync(),
            YeniIlan7Gun = await _db.Ilanlar.CountAsync(i => i.OlusturmaTarihi >= yediGunOnce),
            BekleyenSikayet = await _db.Sikayetler.CountAsync(s => s.Durum == SikayetDurumu.Beklemede),
            Mesaj7Gun = await _db.Mesajlar.CountAsync(m => m.Tarih >= yediGunOnce),
            KayitliArama = await _db.KayitliAramalar.CountAsync(),
            SonKayitlar = await _db.Users.OrderByDescending(u => u.KayitTarihi).Take(8).ToListAsync(),
            SonIlanlar = await _db.Ilanlar.Include(i => i.Kullanici).OrderByDescending(i => i.OlusturmaTarihi).Take(8).ToListAsync(),
        };

        // Son 14 günün günlük kayıt ve ilan sayıları (basit grafik için)
        var baslangic = DateTime.UtcNow.Date.AddDays(-13);
        var kayitlar = await _db.Users.Where(u => u.KayitTarihi >= baslangic).Select(u => u.KayitTarihi).ToListAsync();
        var ilanlar = await _db.Ilanlar.Where(i => i.OlusturmaTarihi >= baslangic).Select(i => i.OlusturmaTarihi).ToListAsync();
        for (var g = 0; g < 14; g++)
        {
            var gun = baslangic.AddDays(g);
            vm.Gunluk.Add((gun, kayitlar.Count(t => t.Date == gun), ilanlar.Count(t => t.Date == gun)));
        }
        return View(vm);
    }

    // ---------- Kullanıcılar ----------
    public async Task<IActionResult> Kullanicilar(string? q, string? durum, int sayfa = 1)
    {
        var sorgu = _db.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var a = q.Trim();
            sorgu = sorgu.Where(u => u.Email!.Contains(a) || u.Ad.Contains(a) || u.Soyad.Contains(a));
        }
        sorgu = durum switch
        {
            "askida" => sorgu.Where(u => u.AskiyaAlindi),
            "dogrulanmis" => sorgu.Where(u => u.OgrenciDogrulandi),
            "onaysiz" => sorgu.Where(u => !u.EmailConfirmed),
            _ => sorgu
        };

        var toplam = await sorgu.CountAsync();
        var toplamSayfa = Math.Max(1, (int)Math.Ceiling(toplam / (double)SayfaBoyutu));
        sayfa = Math.Clamp(sayfa, 1, toplamSayfa);

        var liste = await sorgu
            .OrderByDescending(u => u.KayitTarihi)
            .Skip((sayfa - 1) * SayfaBoyutu).Take(SayfaBoyutu)
            .Select(u => new YonetimKullaniciSatiri
            {
                Kullanici = u,
                IlanSayisi = _db.Ilanlar.Count(i => i.KullaniciId == u.Id),
                SikayetSayisi = _db.Sikayetler.Count(s => s.SikayetEdilenKullaniciId == u.Id),
            })
            .ToListAsync();

        var yoneticiler = (await _userManager.GetUsersInRoleAsync(Roller.Yonetici)).Select(u => u.Id).ToHashSet();
        foreach (var s in liste) s.Yonetici = yoneticiler.Contains(s.Kullanici.Id);

        ViewBag.Q = q;
        ViewBag.Durum = durum;
        ViewBag.Sayfa = sayfa;
        ViewBag.ToplamSayfa = toplamSayfa;
        ViewBag.Toplam = toplam;
        return View(liste);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AskiyaAl(string id, string? neden, string? returnUrl)
    {
        var k = await _userManager.FindByIdAsync(id);
        if (k == null) return NotFound();

        if (k.Id == _userManager.GetUserId(User))
            TempData["Mesaj"] = "Kendi hesabını askıya alamazsın.";
        else if (await _userManager.IsInRoleAsync(k, Roller.Yonetici))
            TempData["Mesaj"] = "Yönetici hesapları askıya alınamaz. Önce yönetici yetkisini kaldırmak gerekir.";
        else
        {
            k.AskiyaAlindi = true;
            k.AskiyaAlinmaTarihi = DateTime.UtcNow;
            k.AskiyaAlmaNedeni = string.IsNullOrWhiteSpace(neden) ? null : neden.Trim()[..Math.Min(neden.Trim().Length, 200)];
            await _userManager.UpdateAsync(k);
            // Açık oturumlarının kapanması için güvenlik damgasını yenile
            await _userManager.UpdateSecurityStampAsync(k);
            TempData["Mesaj"] = $"{k.Ad} {k.Soyad} askıya alındı. İlanları ve profili gizlendi, giriş yapamaz.";
        }
        return GeriDon(returnUrl);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AskiyiKaldir(string id, string? returnUrl)
    {
        var k = await _userManager.FindByIdAsync(id);
        if (k == null) return NotFound();
        k.AskiyaAlindi = false;
        k.AskiyaAlinmaTarihi = null;
        k.AskiyaAlmaNedeni = null;
        await _userManager.UpdateAsync(k);
        TempData["Mesaj"] = $"{k.Ad} {k.Soyad} hesabı tekrar açıldı.";
        return GeriDon(returnUrl);
    }

    // ---------- İlanlar ----------
    public async Task<IActionResult> Ilanlar(string? q, string? durum, int sayfa = 1)
    {
        var sorgu = _db.Ilanlar.Include(i => i.Kullanici).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var a = q.Trim();
            sorgu = sorgu.Where(i => i.Baslik.Contains(a) || i.Sehir.Contains(a) || i.Ilce.Contains(a) || i.Kullanici!.Email!.Contains(a));
        }
        sorgu = durum switch
        {
            "yayinda" => sorgu.Where(i => i.Aktif),
            "kapali" => sorgu.Where(i => !i.Aktif),
            "sikayetli" => sorgu.Where(i => _db.Sikayetler.Any(s => s.IlanId == i.Id)),
            _ => sorgu
        };

        var toplam = await sorgu.CountAsync();
        var toplamSayfa = Math.Max(1, (int)Math.Ceiling(toplam / (double)SayfaBoyutu));
        sayfa = Math.Clamp(sayfa, 1, toplamSayfa);

        var liste = await sorgu
            .OrderByDescending(i => i.OlusturmaTarihi)
            .Skip((sayfa - 1) * SayfaBoyutu).Take(SayfaBoyutu)
            .ToListAsync();
        var idler = liste.Select(i => i.Id).ToList();
        ViewBag.SikayetSayilari = await _db.Sikayetler
            .Where(s => s.IlanId != null && idler.Contains(s.IlanId.Value))
            .GroupBy(s => s.IlanId!.Value)
            .Select(g => new { g.Key, Sayi = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Sayi);

        ViewBag.Q = q;
        ViewBag.Durum = durum;
        ViewBag.Sayfa = sayfa;
        ViewBag.ToplamSayfa = toplamSayfa;
        ViewBag.Toplam = toplam;
        return View(liste);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> IlanYayini(int id, string? returnUrl)
    {
        var ilan = await _db.Ilanlar.FirstOrDefaultAsync(i => i.Id == id);
        if (ilan == null) return NotFound();
        ilan.Aktif = !ilan.Aktif;
        await _db.SaveChangesAsync();
        TempData["Mesaj"] = ilan.Aktif ? $"\"{ilan.Baslik}\" tekrar yayına alındı." : $"\"{ilan.Baslik}\" yayından kaldırıldı.";
        return GeriDon(returnUrl);
    }

    private IActionResult GeriDon(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction(nameof(Kullanicilar));
}
