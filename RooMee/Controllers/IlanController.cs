using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;

namespace RooMee.Controllers;

[Authorize]
public class IlanController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    private static readonly string[] IzinliUzantilar = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFotoBoyutu = 3 * 1024 * 1024; // 3 MB
    private const int MaxFotoSayisi = 5;

    private readonly RooMee.Services.AramaUyarilari _aramaUyarilari;

    public IlanController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env,
        RooMee.Services.AramaUyarilari aramaUyarilari)
    {
        _db = db;
        _userManager = userManager;
        _env = env;
        _aramaUyarilari = aramaUyarilari;
    }

    // Tüm yayındaki ilanlar (giriş yapmadan da görülebilir)
    [AllowAnonymous]
    public async Task<IActionResult> Index([FromQuery] IlanAramaViewModel f)
    {
        const int SayfaBoyutu = 12;

        var q = RooMee.Services.IlanFiltresi.Uygula(_db.Ilanlar.Where(i => i.Aktif), f);

        q = f.Siralama switch
        {
            "ucuz" => q.OrderBy(i => i.Kira),
            "pahali" => q.OrderByDescending(i => i.Kira),
            "eski" => q.OrderBy(i => i.OlusturmaTarihi),
            _ => q.OrderByDescending(i => i.OlusturmaTarihi)
        };

        f.ToplamSayi = await q.CountAsync();
        f.ToplamSayfa = Math.Max(1, (int)Math.Ceiling(f.ToplamSayi / (double)SayfaBoyutu));
        f.Sayfa = Math.Clamp(f.Sayfa, 1, f.ToplamSayfa);

        f.Sonuclar = await q
            .Include(i => i.Kullanici)
            .Include(i => i.Fotolar)
            .Skip((f.Sayfa - 1) * SayfaBoyutu)
            .Take(SayfaBoyutu)
            .ToListAsync();

        // Harita görünümü: filtreye uyan tüm ilanlar şehir merkezinde gruplanır
        if (f.Gorunum == "harita")
        {
            var hepsi = await q
                .Select(i => new { i.Id, i.Baslik, i.Sehir, i.Ilce, i.Kira })
                .Take(1000)
                .ToListAsync();
            foreach (var grup in hepsi.GroupBy(i => i.Sehir.Trim().ToUpper(new System.Globalization.CultureInfo("tr-TR"))))
            {
                var konum = RooMee.Services.SehirKonumlari.Bul(grup.Key);
                if (konum == null) { f.HaritadaGosterilemeyen += grup.Count(); continue; }
                f.HaritaNoktalari.Add(new HaritaNoktasi
                {
                    Sehir = grup.First().Sehir.Trim(),
                    Enlem = konum.Value.Enlem,
                    Boylam = konum.Value.Boylam,
                    Sayi = grup.Count(),
                    Ilanlar = grup.Take(8).Select(i => new HaritaIlani(i.Id, i.Baslik, i.Ilce, i.Kira)).ToList()
                });
            }
        }

        // Şehir kutusunda öneri olarak çıkacak şehirler
        f.Sehirler = await _db.Ilanlar
            .Where(i => i.Aktif)
            .Select(i => i.Sehir)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        // Giriş yapmış kullanıcının favorileri (kartlardaki kalp butonu için)
        var benimId = _userManager.GetUserId(User);
        if (benimId != null)
        {
            ViewData["BenimId"] = benimId;
            ViewData["FavoriIdleri"] = (await _db.Favoriler
                .Where(x => x.KullaniciId == benimId)
                .Select(x => x.IlanId)
                .ToListAsync()).ToHashSet();
        }

        return View(f);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Detay(int id)
    {
        var ilan = await _db.Ilanlar
            .Include(i => i.Kullanici).ThenInclude(u => u!.Profil)
            .Include(i => i.Fotolar)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ilan == null) return NotFound();

        var benimId = _userManager.GetUserId(User);
        var yonetici = User.IsInRole(Roller.Yonetici);
        // Yayında olmayan ya da sahibi askıya alınmış ilanı sadece sahibi ve yöneticiler görebilir
        if ((!ilan.Aktif || ilan.Kullanici!.AskiyaAlindi) && ilan.KullaniciId != benimId && !yonetici) return NotFound();

        ViewBag.Sahibi = ilan.KullaniciId == benimId;
        ViewBag.Favoride = benimId != null &&
    await _db.Favoriler.AnyAsync(f => f.KullaniciId == benimId && f.IlanId == id);
        return View(ilan);
    }

    public async Task<IActionResult> Ilanlarim()
    {
        var benimId = _userManager.GetUserId(User);
        var ilanlar = await _db.Ilanlar
            .Where(i => i.KullaniciId == benimId)
            .Include(i => i.Kullanici)
            .Include(i => i.Fotolar)
            .OrderByDescending(i => i.OlusturmaTarihi)
            .ToListAsync();
        return View(ilanlar);
    }

    [HttpGet]
    public IActionResult Olustur() => View("Form", new IlanFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Olustur(IlanFormViewModel vm)
    {
        FotolariKontrolEt(vm, mevcutSayi: 0);
        if (!ModelState.IsValid) return View("Form", vm);

        var ilan = new Ilan { KullaniciId = _userManager.GetUserId(User)! };
        FormdanAktar(vm, ilan);
        ilan.Aktif = true;
        _db.Ilanlar.Add(ilan);
        await FotolariKaydet(vm, ilan);
        await _db.SaveChangesAsync();

        // Bu ilana uyan kayıtlı araması olanlara e-posta ile haber ver
        await _aramaUyarilari.YeniIlanIcinAsync(ilan.Id,
            i => Url.Action(nameof(Detay), "Ilan", new { id = i.Id }, Request.Scheme)!,
            a => Url.Action("Ac", "KayitliArama", new { id = a.Id }, Request.Scheme)!);

        TempData["Mesaj"] = "İlanın yayınlandı.";
        return RedirectToAction(nameof(Detay), new { id = ilan.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Duzenle(int id)
    {
        var ilan = await SahibiOlunanIlan(id);
        if (ilan == null) return NotFound();

        return View("Form", new IlanFormViewModel
        {
            Id = ilan.Id,
            Baslik = ilan.Baslik,
            Sehir = ilan.Sehir,
            Ilce = ilan.Ilce,
            Kira = ilan.Kira,
            OdaTipi = ilan.OdaTipi,
            Aciklama = ilan.Aciklama,
            SadeceOgrenci = ilan.SadeceOgrenci,
            SigaraIcmeyen = ilan.SigaraIcmeyen,
            Sessiz = ilan.Sessiz,
            EvcilHayvanKabul = ilan.EvcilHayvanKabul,
            Aktif = ilan.Aktif,
            MevcutFotolar = ilan.Fotolar
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(int id, IlanFormViewModel vm)
    {
        var ilan = await SahibiOlunanIlan(id);
        if (ilan == null) return NotFound();

        var silinecekler = ilan.Fotolar.Where(f => vm.SilinecekFotolar.Contains(f.Id)).ToList();
        FotolariKontrolEt(vm, mevcutSayi: ilan.Fotolar.Count - silinecekler.Count);

        if (!ModelState.IsValid)
        {
            vm.Id = id;
            vm.MevcutFotolar = ilan.Fotolar;
            return View("Form", vm);
        }

        FormdanAktar(vm, ilan);
        ilan.Aktif = vm.Aktif;
        _db.IlanFotolari.RemoveRange(silinecekler);
        await FotolariKaydet(vm, ilan);
        await _db.SaveChangesAsync();

        foreach (var foto in silinecekler) DosyaSil(foto.Yol);

        TempData["Mesaj"] = "İlanın güncellendi.";
        return RedirectToAction(nameof(Detay), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Sil(int id)
    {
        var ilan = await SahibiOlunanIlan(id);
        if (ilan == null) return NotFound();

        var yollar = ilan.Fotolar.Select(f => f.Yol).ToList();
        _db.Ilanlar.Remove(ilan);
        await _db.SaveChangesAsync();
        foreach (var yol in yollar) DosyaSil(yol);

        TempData["Mesaj"] = "İlan silindi.";
        return RedirectToAction(nameof(Ilanlarim));
    }

    // ---------- Yardımcılar ----------

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> YayinDurumu(int id)
    {
        var ilan = await SahibiOlunanIlan(id);
        if (ilan == null) return NotFound();

        ilan.Aktif = !ilan.Aktif;
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = ilan.Aktif ? "İlan tekrar yayında." : "İlan yayından kaldırıldı.";
        return RedirectToAction(nameof(Ilanlarim));
    }
    private async Task<Ilan?> SahibiOlunanIlan(int id)
    {
        var benimId = _userManager.GetUserId(User);
        return await _db.Ilanlar
            .Include(i => i.Fotolar)
            .FirstOrDefaultAsync(i => i.Id == id && i.KullaniciId == benimId);
    }

    private static void FormdanAktar(IlanFormViewModel vm, Ilan ilan)
    {
        ilan.Baslik = vm.Baslik.Trim();
        ilan.Sehir = vm.Sehir.Trim();
        ilan.Ilce = vm.Ilce.Trim();
        ilan.Kira = vm.Kira ?? 0;
        ilan.OdaTipi = vm.OdaTipi;
        ilan.Aciklama = vm.Aciklama?.Trim();
        ilan.SadeceOgrenci = vm.SadeceOgrenci;
        ilan.SigaraIcmeyen = vm.SigaraIcmeyen;
        ilan.Sessiz = vm.Sessiz;
        ilan.EvcilHayvanKabul = vm.EvcilHayvanKabul;
    }

    private void FotolariKontrolEt(IlanFormViewModel vm, int mevcutSayi)
    {
        var yeniler = vm.YeniFotolar?.Where(f => f.Length > 0).ToList() ?? new();

        if (mevcutSayi + yeniler.Count > MaxFotoSayisi)
            ModelState.AddModelError(nameof(vm.YeniFotolar), $"Bir ilanda en fazla {MaxFotoSayisi} fotoğraf olabilir.");

        foreach (var f in yeniler)
        {
            var uzanti = Path.GetExtension(f.FileName).ToLowerInvariant();
            if (!IzinliUzantilar.Contains(uzanti))
                ModelState.AddModelError(nameof(vm.YeniFotolar), $"{f.FileName}: sadece JPG, PNG veya WEBP yüklenebilir.");
            else if (f.Length > MaxFotoBoyutu)
                ModelState.AddModelError(nameof(vm.YeniFotolar), $"{f.FileName}: en fazla 3 MB olabilir.");
        }
    }

    private async Task FotolariKaydet(IlanFormViewModel vm, Ilan ilan)
    {
        if (vm.YeniFotolar == null) return;

        var klasor = Path.Combine(_env.WebRootPath, "uploads", "ilan");
        Directory.CreateDirectory(klasor);

        foreach (var f in vm.YeniFotolar.Where(f => f.Length > 0))
        {
            var dosyaAdi = $"{Guid.NewGuid()}{Path.GetExtension(f.FileName).ToLowerInvariant()}";
            using (var akis = System.IO.File.Create(Path.Combine(klasor, dosyaAdi)))
                await f.CopyToAsync(akis);
            ilan.Fotolar.Add(new IlanFoto { Yol = $"/uploads/ilan/{dosyaAdi}" });
        }
    }

    private void DosyaSil(string? yol)
    {
        if (string.IsNullOrEmpty(yol)) return;
        var tamYol = Path.Combine(_env.WebRootPath, yol.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(tamYol)) System.IO.File.Delete(tamYol);
    }
}