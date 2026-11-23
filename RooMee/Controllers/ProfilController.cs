using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;

namespace RooMee.Controllers;

[Authorize]
public class ProfilController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender _eposta;
    private readonly IWebHostEnvironment _env;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private static readonly string[] IzinliUzantilar = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFotoBoyutu = 2 * 1024 * 1024; // 2 MB

    public ProfilController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
                        IEmailSender eposta, IWebHostEnvironment env,
                        SignInManager<ApplicationUser> signInManager)
    {
        _db = db;
        _userManager = userManager;
        _eposta = eposta;
        _env = env;
        _signInManager = signInManager;
    }

    public IActionResult Index() => RedirectToAction(nameof(Goruntule));

    // /Profil/Goruntule      -> kendi profilin
    // /Profil/Goruntule/abc  -> başkasının profili
    public async Task<IActionResult> Goruntule(string? id)
    {
        var benimId = _userManager.GetUserId(User);
        id ??= benimId;

        var kullanici = await _db.Users.Include(u => u.Profil).FirstOrDefaultAsync(u => u.Id == id);
        if (kullanici == null) return NotFound();

        ViewBag.KendiProfili = kullanici.Id == benimId;
        return View(kullanici);
    }

    [HttpGet]
    public async Task<IActionResult> Duzenle()
    {
        var k = await MevcutKullanici();
        var p = k.Profil ?? new Profil();

        return View(new ProfilDuzenleViewModel
        {
            Ad = k.Ad,
            Soyad = k.Soyad,
            Universite = p.Universite,
            Bolum = p.Bolum,
            ButceMin = p.ButceMin,
            ButceMax = p.ButceMax,
            YatisSaati = p.YatisSaati,
            KalkisSaati = p.KalkisSaati,
            SigaraIciyor = p.SigaraIciyor,
            EvcilHayvanVar = p.EvcilHayvanVar,
            SessizOrtamIster = p.SessizOrtamIster,
            Hakkinda = p.Hakkinda,
            MevcutFotoYolu = p.FotoYolu
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Duzenle(ProfilDuzenleViewModel vm)
    {
        var k = await MevcutKullanici();
        vm.MevcutFotoYolu = k.Profil?.FotoYolu;

        if (vm.ButceMin.HasValue && vm.ButceMax.HasValue && vm.ButceMin > vm.ButceMax)
            ModelState.AddModelError(nameof(vm.ButceMax), "En fazla bütçe, en az bütçeden küçük olamaz.");

        if (vm.Foto != null && vm.Foto.Length > 0)
        {
            var uzanti = Path.GetExtension(vm.Foto.FileName).ToLowerInvariant();
            if (!IzinliUzantilar.Contains(uzanti))
                ModelState.AddModelError(nameof(vm.Foto), "Sadece JPG, PNG veya WEBP yükleyebilirsin.");
            else if (vm.Foto.Length > MaxFotoBoyutu)
                ModelState.AddModelError(nameof(vm.Foto), "Fotoğraf en fazla 2 MB olabilir.");
        }

        if (!ModelState.IsValid) return View(vm);

        var p = k.Profil;
        if (p == null)
        {
            p = new Profil { KullaniciId = k.Id };
            _db.Profiller.Add(p);
        }

        if (vm.Foto != null && vm.Foto.Length > 0)
        {
            var klasor = Path.Combine(_env.WebRootPath, "uploads", "profil");
            Directory.CreateDirectory(klasor);
            var dosyaAdi = $"{Guid.NewGuid()}{Path.GetExtension(vm.Foto.FileName).ToLowerInvariant()}";

            using (var akis = System.IO.File.Create(Path.Combine(klasor, dosyaAdi)))
                await vm.Foto.CopyToAsync(akis);

            EskiFotoyuSil(p.FotoYolu);
            p.FotoYolu = $"/uploads/profil/{dosyaAdi}";
        }

        k.Ad = vm.Ad.Trim();
        k.Soyad = vm.Soyad.Trim();
        p.Universite = vm.Universite?.Trim();
        p.Bolum = vm.Bolum?.Trim();
        p.ButceMin = vm.ButceMin;
        p.ButceMax = vm.ButceMax;
        p.YatisSaati = vm.YatisSaati;
        p.KalkisSaati = vm.KalkisSaati;
        p.SigaraIciyor = vm.SigaraIciyor;
        p.EvcilHayvanVar = vm.EvcilHayvanVar;
        p.SessizOrtamIster = vm.SessizOrtamIster;
        p.Hakkinda = vm.Hakkinda?.Trim();
        p.GuncellemeTarihi = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Mesaj"] = "Profilin kaydedildi.";
        return RedirectToAction(nameof(Goruntule));
    }

    // ---------- Öğrenci doğrulama ----------

    private static string Amac(string eposta) => "OgrenciDogrulama:" + eposta.ToLowerInvariant();

    [HttpGet]
    public async Task<IActionResult> OgrenciDogrula()
    {
        var k = await MevcutKullanici();
        if (k.OgrenciDogrulandi)
        {
            TempData["Mesaj"] = "Öğrenciliğin zaten doğrulanmış.";
            return RedirectToAction(nameof(Goruntule));
        }
        return View(new OgrenciDogrulamaViewModel { OgrenciEposta = k.OgrenciEposta ?? "" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> KodGonder(OgrenciDogrulamaViewModel vm)
    {
        ModelState.Remove(nameof(vm.Kod));
        if (!ModelState.IsValid) return View("OgrenciDogrula", vm);

        var eposta = vm.OgrenciEposta.Trim().ToLowerInvariant();
        var k = await MevcutKullanici();

        var baskasindaVar = await _db.Users.AnyAsync(u =>
            u.Id != k.Id && u.OgrenciDogrulandi && u.OgrenciEposta == eposta);
        if (baskasindaVar)
        {
            ModelState.AddModelError(nameof(vm.OgrenciEposta), "Bu üniversite e-postası başka bir hesapta doğrulanmış.");
            return View("OgrenciDogrula", vm);
        }

        k.OgrenciEposta = eposta;
        k.OgrenciDogrulandi = false;
        await _db.SaveChangesAsync();

        var kod = await _userManager.GenerateUserTokenAsync(k, TokenOptions.DefaultEmailProvider, Amac(eposta));
        await _eposta.SendEmailAsync(eposta, "RooMee - Öğrenci doğrulama kodun",
            $"Merhaba {k.Ad},<br><br>Öğrenci doğrulama kodun: <b style='font-size:22px;letter-spacing:4px'>{kod}</b>" +
            "<br><br>Kod birkaç dakika geçerlidir. Bu isteği sen yapmadıysan bu e-postayı yok sayabilirsin.");

        ModelState.Clear();
        return View("OgrenciDogrula", new OgrenciDogrulamaViewModel { OgrenciEposta = eposta, KodGonderildi = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> KodDogrula(OgrenciDogrulamaViewModel vm)
    {
        var k = await MevcutKullanici();
        vm.KodGonderildi = true;
        ModelState.Clear();

        if (string.IsNullOrWhiteSpace(vm.Kod) || string.IsNullOrEmpty(k.OgrenciEposta))
        {
            ModelState.AddModelError(nameof(vm.Kod), "E-postana gelen kodu gir.");
            return View("OgrenciDogrula", vm);
        }

        var gecerli = await _userManager.VerifyUserTokenAsync(
            k, TokenOptions.DefaultEmailProvider, Amac(k.OgrenciEposta), vm.Kod.Trim());

        if (!gecerli)
        {
            ModelState.AddModelError(nameof(vm.Kod), "Kod hatalı ya da süresi dolmuş. Yeni kod isteyebilirsin.");
            return View("OgrenciDogrula", vm);
        }

        k.OgrenciDogrulandi = true;
        k.OgrenciDogrulamaTarihi = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Mesaj"] = "Tebrikler! Öğrenciliğin doğrulandı.";
        return RedirectToAction(nameof(Goruntule));
    }

    // ---------- Hesabı silme ----------

    [HttpGet]
    public IActionResult HesapSil() => View(new HesapSilViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> HesapSil(HesapSilViewModel vm)
    {
        if (!vm.Onay)
            ModelState.AddModelError(nameof(vm.Onay), "Devam etmek için kutuyu işaretlemelisin.");
        if (!ModelState.IsValid) return View(vm);

        var k = await MevcutKullanici();
        if (!await _userManager.CheckPasswordAsync(k, vm.Sifre))
        {
            ModelState.AddModelError(nameof(vm.Sifre), "Şifre hatalı.");
            return View(vm);
        }

        // Diskten silinecek fotoğrafları önceden topla
        var dosyalar = await _db.IlanFotolari
            .Where(f => f.Ilan!.KullaniciId == k.Id)
            .Select(f => f.Yol)
            .ToListAsync();
        if (!string.IsNullOrEmpty(k.Profil?.FotoYolu))
            dosyalar.Add(k.Profil.FotoYolu);

        await using var islem = await _db.Database.BeginTransactionAsync();

        // Zincirleme silinmeyen kayıtları elle sil
        await _db.Favoriler.Where(f => f.KullaniciId == k.Id).ExecuteDeleteAsync();
        await _db.Sikayetler
            .Where(s => s.SikayetEdenId == k.Id || s.SikayetEdilenKullaniciId == k.Id)
            .ExecuteDeleteAsync();
        // Konuşmaları sil (mesajlar zincirleme silinir)
        await _db.Konusmalar
            .Where(x => x.Kullanici1Id == k.Id || x.Kullanici2Id == k.Id)
            .ExecuteDeleteAsync();

        // Kullanıcıyı sil (profil, ilanlar ve ilan fotoğrafları kayıtları otomatik silinir)
        var sonuc = await _userManager.DeleteAsync(k);
        if (!sonuc.Succeeded)
        {
            await islem.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Hesap silinirken bir hata oluştu. Lütfen tekrar dene.");
            return View(vm);
        }
        await islem.CommitAsync();

        foreach (var yol in dosyalar)
            EskiFotoyuSil(yol);

        await _signInManager.SignOutAsync();
        TempData["Mesaj"] = "Hesabın ve tüm verilerin kalıcı olarak silindi.";
        return Redirect("/");
    }

    // ---------- Yardımcılar ----------

    private async Task<ApplicationUser> MevcutKullanici()
    {
        var id = _userManager.GetUserId(User);
        return await _db.Users.Include(u => u.Profil).FirstAsync(u => u.Id == id);
    }

    private void EskiFotoyuSil(string? yol)
    {
        if (string.IsNullOrEmpty(yol)) return;
        var tamYol = Path.Combine(_env.WebRootPath, yol.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(tamYol)) System.IO.File.Delete(tamYol);
    }
}