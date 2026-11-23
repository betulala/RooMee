using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RooMee.Data;
using RooMee.Models;
using RooMee.Services;

namespace RooMee.Controllers;

// Ev arkadaşı arayan / kiralık evi olan kullanıcıların listesi ve filtreleri
public class KullaniciController : Controller
{
    private const int SayfaBoyutu = 12;

    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public KullaniciController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index([FromQuery] KullaniciFiltreViewModel f)
    {
        var benimId = _userManager.GetUserId(User);

        // Listede sadece "Listede nasıl görüneyim?" sorusunu cevaplamış ve şehrini yazmış profiller görünür
        var q = _db.Users
            .Include(u => u.Profil)
            .Where(u => u.Profil != null && u.Profil.AramaTuru != null && u.Profil.Sehir != null);
        if (benimId != null) q = q.Where(u => u.Id != benimId);

        // Filtre seçenekleri (kişi sayılarıyla), filtrelerden önce hesaplanır
        f.Sehirler = (await q.GroupBy(u => u.Profil!.Sehir!).Select(g => new { g.Key, Sayi = g.Count() }).ToListAsync())
            .OrderBy(x => x.Key, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), true))
            .Select(x => (x.Key, x.Sayi)).ToList();
        f.Universiteler = (await q.Where(u => u.Profil!.Universite != null).Select(u => u.Profil!.Universite!).Distinct().ToListAsync())
            .OrderBy(x => x, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), true)).ToList();

        // ---------- Veritabanında uygulanabilen filtreler ----------
        if (!string.IsNullOrWhiteSpace(f.Sehir)) q = q.Where(u => u.Profil!.Sehir == f.Sehir);
        if (!string.IsNullOrWhiteSpace(f.Universite)) q = q.Where(u => u.Profil!.Universite == f.Universite);

        // Bütçe: kişinin bütçe aralığı, aranan aralıkla kesişmeli
        if (f.ButceMin.HasValue) q = q.Where(u => (u.Profil!.ButceMax ?? u.Profil.ButceMin) >= f.ButceMin);
        if (f.ButceMax.HasValue) q = q.Where(u => (u.Profil!.ButceMin ?? u.Profil.ButceMax) <= f.ButceMax);

        var buYil = DateTime.Today.Year;
        if (f.YasMin.HasValue) { var enGec = buYil - f.YasMin.Value; q = q.Where(u => u.Profil!.DogumYili <= enGec); }
        if (f.YasMax.HasValue) { var enErken = buYil - f.YasMax.Value; q = q.Where(u => u.Profil!.DogumYili >= enErken); }

        if (f.Cinsiyet.Count > 0) q = q.Where(u => u.Profil!.Cinsiyet != null && f.Cinsiyet.Contains(u.Profil.Cinsiyet.Value));

        var ogr = f.Statu.Contains("ogrenci");
        var cal = f.Statu.Contains("calisan");
        if (ogr || cal) q = q.Where(u => (ogr && u.Profil!.Ogrenci) || (cal && u.Profil!.Calisiyor));
        if (f.SadeceDogrulanmis) q = q.Where(u => u.OgrenciDogrulandi);

        if (f.Uyku.Count > 0) q = q.Where(u => u.Profil!.Uyku != null && f.Uyku.Contains(u.Profil.Uyku.Value));
        if (f.Calisma.Count > 0) q = q.Where(u => u.Profil!.CalismaDuzeni != null && f.Calisma.Contains(u.Profil.CalismaDuzeni.Value));
        if (f.Hayvan.Count > 0) q = q.Where(u => u.Profil!.EvcilHayvan != null && f.Hayvan.Contains(u.Profil.EvcilHayvan.Value));

        var icmiyor = f.Sigara.Contains("icmiyor");
        var icer = f.Sigara.Contains("icer");
        if (icmiyor != icer) q = q.Where(u => u.Profil!.SigaraIciyor == icer);

        if (f.Tur.Count > 0) q = q.Where(u => f.Tur.Contains(u.Profil!.AramaTuru!.Value));
        if (f.Oda.Count > 0) q = q.Where(u => u.Profil!.TercihOdaTipi != null && f.Oda.Contains(u.Profil.TercihOdaTipi.Value));

        if (!string.IsNullOrWhiteSpace(f.Q))
        {
            foreach (var kelime in f.Q.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var k = kelime;
                q = q.Where(u => (u.Profil!.Bolum != null && u.Profil.Bolum.Contains(k))
                              || (u.Profil.Meslek != null && u.Profil.Meslek.Contains(k))
                              || u.Ad.Contains(k));
            }
        }

        var liste = await q.ToListAsync();

        // ---------- Bellekte uygulananlar: hobiler (en az biri ortak) ----------
        if (f.Hobi.Count > 0)
            liste = liste.Where(u => u.Profil!.HobiListesi.Any(f.Hobi.Contains)).ToList();

        // ---------- Uyum (giriş yapan kullanıcının profiline göre) ----------
        Profil? benimProfil = null;
        if (benimId != null)
        {
            f.GirisYapti = true;
            benimProfil = await _db.Profiller.FirstOrDefaultAsync(p => p.KullaniciId == benimId);
        }

        var kartlar = liste.Select(u => new KullaniciKartViewModel
        {
            Kullanici = u,
            Uyum = Uyum.Hesapla(benimProfil, u.Profil)?.Skor
        }).ToList();
        f.UyumHesaplanabilir = kartlar.Any(k => k.Uyum.HasValue);

        if (f.Siralama == "uyum" && !f.UyumHesaplanabilir) f.Siralama = null;
        kartlar = f.Siralama switch
        {
            "uyum" => kartlar.OrderByDescending(k => k.Uyum ?? -1).ToList(),
            "butce-artan" => kartlar.OrderBy(k => k.Profil.Butce ?? int.MaxValue).ToList(),
            "butce-azalan" => kartlar.OrderByDescending(k => k.Profil.Butce ?? -1).ToList(),
            "yas-artan" => kartlar.OrderBy(k => k.Profil.Yas ?? int.MaxValue).ToList(),
            "eski" => kartlar.OrderBy(k => k.Profil.GuncellemeTarihi).ToList(),
            _ => kartlar.OrderByDescending(k => k.Profil.GuncellemeTarihi).ToList(),
        };

        f.ToplamSayi = kartlar.Count;
        f.ToplamSayfa = Math.Max(1, (int)Math.Ceiling(f.ToplamSayi / (double)SayfaBoyutu));
        f.Sayfa = Math.Clamp(f.Sayfa, 1, f.ToplamSayfa);
        f.Sonuclar = kartlar.Skip((f.Sayfa - 1) * SayfaBoyutu).Take(SayfaBoyutu).ToList();

        // Bu sayfadaki kişilerin ortalama değerlendirme puanı
        var idler = f.Sonuclar.Select(k => k.Kullanici.Id).ToList();
        var puanlar = await _db.Degerlendirmeler
            .Where(d => idler.Contains(d.HakkindaId))
            .GroupBy(d => d.HakkindaId)
            .Select(g => new { g.Key, Ort = g.Average(d => d.Puan), Sayi = g.Count() })
            .ToDictionaryAsync(x => x.Key);
        foreach (var k in f.Sonuclar)
        {
            if (puanlar.TryGetValue(k.Kullanici.Id, out var p)) { k.Puan = p.Ort; k.PuanSayisi = p.Sayi; }
        }

        return View(f);
    }
}
