using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;

namespace RooMee.Controllers;

[Authorize]
public class SikayetController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RooMee.Services.Bildirimci _bildirimci;

    public SikayetController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RooMee.Services.Bildirimci bildirimci)
    {
        _db = db;
        _userManager = userManager;
        _bildirimci = bildirimci;
    }

    private string BenimId => _userManager.GetUserId(User)!;

    // ---------- Şikayet bildirme (ilan ya da kullanıcı) ----------

    [HttpGet]
    public async Task<IActionResult> Olustur(int? ilanId, string? kullaniciId)
    {
        var vm = new SikayetFormViewModel { IlanId = ilanId, KullaniciId = kullaniciId };
        var hata = await HedefiDoldur(vm);
        if (hata != null) return hata;
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(SikayetFormViewModel vm)
    {
        var hata = await HedefiDoldur(vm);
        if (hata != null) return hata;

        if (!SikayetFormViewModel.Nedenler.Contains(vm.Neden))
            ModelState.AddModelError(nameof(vm.Neden), "Listeden bir şikayet nedeni seç.");
        if (!ModelState.IsValid) return View(vm);

        _db.Sikayetler.Add(new Sikayet
        {
            SikayetEdenId = BenimId,
            IlanId = vm.IlanId,
            SikayetEdilenKullaniciId = vm.KullaniciId,
            Neden = vm.Neden,
            Aciklama = string.IsNullOrWhiteSpace(vm.Aciklama) ? null : vm.Aciklama.Trim(),
        });
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Şikayetin alındı, teşekkürler. Ekibimiz en kısa sürede inceleyecek.";
        return vm.IlanId.HasValue
            ? RedirectToAction("Detay", "Ilan", new { id = vm.IlanId })
            : RedirectToAction("Goruntule", "Profil", new { id = vm.KullaniciId });
    }

    // Şikayet edilen ilanı/kullanıcıyı bulur; geçersizse hata sonucu döner
    private async Task<IActionResult?> HedefiDoldur(SikayetFormViewModel vm)
    {
        var benimId = BenimId;

        if (vm.IlanId.HasValue)
        {
            var ilan = await _db.Ilanlar.Include(i => i.Kullanici).FirstOrDefaultAsync(i => i.Id == vm.IlanId);
            if (ilan == null) return NotFound();
            vm.KullaniciId = ilan.KullaniciId;   // ilan şikayetinde ilan sahibi de kaydedilir
            vm.IlanBaslik = ilan.Baslik;
            vm.KullaniciAd = ilan.Kullanici?.KisaAd;
        }
        else if (!string.IsNullOrEmpty(vm.KullaniciId))
        {
            var k = await _db.Users.FirstOrDefaultAsync(u => u.Id == vm.KullaniciId);
            if (k == null) return NotFound();
            vm.KullaniciAd = k.KisaAd;
        }
        else
        {
            return BadRequest();
        }

        if (vm.KullaniciId == benimId)
        {
            TempData["Mesaj"] = "Kendi ilanını ya da profilini şikayet edemezsin.";
            return Geri(vm);
        }

        // Aynı hedef için incelenmeyi bekleyen bir şikayetim varsa tekrar açtırma
        var bekleyenVar = await _db.Sikayetler.AnyAsync(s =>
            s.SikayetEdenId == benimId && s.Durum == SikayetDurumu.Beklemede &&
            (vm.IlanId.HasValue ? s.IlanId == vm.IlanId : s.IlanId == null && s.SikayetEdilenKullaniciId == vm.KullaniciId));
        if (bekleyenVar)
        {
            TempData["Mesaj"] = "Bu konuda zaten incelenmeyi bekleyen bir şikayetin var.";
            return Geri(vm);
        }
        return null;
    }

    private IActionResult Geri(SikayetFormViewModel vm) => vm.IlanId.HasValue
        ? RedirectToAction("Detay", "Ilan", new { id = vm.IlanId })
        : RedirectToAction("Goruntule", "Profil", new { id = vm.KullaniciId });

    // ---------- Yönetim ----------

    [Authorize(Roles = Roller.Yonetici)]
    public async Task<IActionResult> Yonetim(SikayetDurumu durum = SikayetDurumu.Beklemede)
    {
        ViewBag.Durum = durum;
        ViewBag.Sayilar = await _db.Sikayetler
            .GroupBy(s => s.Durum)
            .Select(g => new { g.Key, Sayi = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Sayi);

        var liste = await _db.Sikayetler
            .Where(s => s.Durum == durum)
            .Include(s => s.SikayetEden)
            .Include(s => s.SikayetEdilenKullanici)
            .Include(s => s.Ilan)
            .OrderByDescending(s => s.Tarih)
            .ToListAsync();

        // Aynı kişi/ilan hakkında kaç şikayet var (tekrar eden şikayetleri öne çıkarmak için)
        ViewBag.KullaniciSikayetSayisi = await _db.Sikayetler
            .Where(s => s.SikayetEdilenKullaniciId != null)
            .GroupBy(s => s.SikayetEdilenKullaniciId!)
            .Select(g => new { g.Key, Sayi = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Sayi);

        return View(liste);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = Roller.Yonetici)]
    public async Task<IActionResult> Karar(int id, SikayetDurumu durum, bool ilaniKaldir)
    {
        var s = await _db.Sikayetler.Include(x => x.Ilan).ThenInclude(i => i!.Kullanici).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return NotFound();

        var eskiDurum = s.Durum;
        s.Durum = durum;

        if (ilaniKaldir && s.Ilan != null && s.Ilan.Aktif)
        {
            s.Ilan.Aktif = false;
            TempData["Mesaj"] = $"Şikayet kapatıldı ve \"{s.Ilan.Baslik}\" ilanı yayından kaldırıldı. İlan sahibine e-posta ile bildirildi.";
            await _db.SaveChangesAsync();
            if (s.Ilan.Kullanici != null)
                await _bildirimci.IlanKaldirildiAsync(s.Ilan.Kullanici, s.Ilan.Baslik, s.Neden);
            return RedirectToAction(nameof(Yonetim), new { durum = eskiDurum });
        }
        else
        {
            TempData["Mesaj"] = durum switch
            {
                SikayetDurumu.Incelendi => "Şikayet incelendi olarak işaretlendi.",
                SikayetDurumu.Reddedildi => "Şikayet reddedildi.",
                _ => "Şikayet tekrar beklemeye alındı."
            };
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Yonetim), new { durum = eskiDurum });
    }
}
