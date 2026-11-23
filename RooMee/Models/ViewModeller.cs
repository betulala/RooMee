using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

public class ProfilDuzenleViewModel
{
    [Required(ErrorMessage = "Ad zorunludur."), StringLength(50), Display(Name = "Ad")]
    public string Ad { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur."), StringLength(50), Display(Name = "Soyad")]
    public string Soyad { get; set; } = "";

    [StringLength(100), Display(Name = "Üniversite")]
    public string? Universite { get; set; }

    [StringLength(100), Display(Name = "Bölüm")]
    public string? Bolum { get; set; }

    [Range(0, 100000, ErrorMessage = "Geçerli bir tutar gir."), Display(Name = "En az bütçe (TL)")]
    public int? ButceMin { get; set; }

    [Range(0, 100000, ErrorMessage = "Geçerli bir tutar gir."), Display(Name = "En fazla bütçe (TL)")]
    public int? ButceMax { get; set; }

    [Display(Name = "Yatış saati")]
    public TimeOnly? YatisSaati { get; set; }

    [Display(Name = "Kalkış saati")]
    public TimeOnly? KalkisSaati { get; set; }

    [Display(Name = "Sigara içiyorum")]
    public bool SigaraIciyor { get; set; }

    [StringLength(1000), Display(Name = "Hakkımda")]
    public string? Hakkinda { get; set; }

    // ---------- Ev arkadaşı profili ----------
    [Display(Name = "Cinsiyet")]
    public Cinsiyet? Cinsiyet { get; set; }

    [Range(1950, 2010, ErrorMessage = "Doğum yılı 1950 ile 2010 arasında olmalı."), Display(Name = "Doğum yılı")]
    public int? DogumYili { get; set; }

    [StringLength(50), Display(Name = "Şehir")]
    public string? Sehir { get; set; }

    [StringLength(50), Display(Name = "İlçe")]
    public string? Ilce { get; set; }

    [Display(Name = "Öğrenciyim")]
    public bool Ogrenci { get; set; }

    [Display(Name = "Çalışıyorum")]
    public bool Calisiyor { get; set; }

    [StringLength(100), Display(Name = "Meslek / iş")]
    public string? Meslek { get; set; }

    [StringLength(20), Display(Name = "Sınıf")]
    public string? Sinif { get; set; }

    [Display(Name = "Ev arkadaşları listesinde")]
    public AramaTuru? AramaTuru { get; set; }

    [Display(Name = "Oda tipi tercihi")]
    public TercihOdaTipi? TercihOdaTipi { get; set; }

    [StringLength(10), Display(Name = "Ev tipi")]
    public string? EvTipi { get; set; }

    [Range(1, 10, ErrorMessage = "Boş oda sayısı 1 ile 10 arasında olmalı."), Display(Name = "Boş oda sayısı")]
    public int? BosOda { get; set; }

    [Display(Name = "Uyku düzeni")]
    public UykuDuzeni? Uyku { get; set; }

    [Display(Name = "Ders / çalışma düzeni")]
    public CalismaDuzeni? CalismaDuzeni { get; set; }

    [Display(Name = "Evcil hayvan")]
    public EvcilHayvanDurumu? EvcilHayvan { get; set; }

    [Display(Name = "Hobiler")]
    public List<string> Hobiler { get; set; } = new();

    [Display(Name = "E-posta bildirimleri (yeni mesaj, kayıtlı arama uyarısı)")]
    public bool EpostaBildirim { get; set; } = true;

    [Display(Name = "Profil fotoğrafı")]
    public IFormFile? Foto { get; set; }

    public string? MevcutFotoYolu { get; set; }
}

public class OgrenciDogrulamaViewModel
{
    [Required(ErrorMessage = "Üniversite e-postası zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi gir.")]
    [RegularExpression(@"^[^@\s]+@([A-Za-z0-9-]+\.)*edu\.tr$",
        ErrorMessage = "Sadece .edu.tr uzantılı üniversite e-postaları kabul edilir.")]
    [Display(Name = "Üniversite e-postası")]
    public string OgrenciEposta { get; set; } = "";

    [Display(Name = "Doğrulama kodu")]
    public string? Kod { get; set; }

    public bool KodGonderildi { get; set; }
}

public class HesapSilViewModel
{
    [Required(ErrorMessage = "Şifreni gir."), DataType(DataType.Password), Display(Name = "Şifre")]
    public string Sifre { get; set; } = "";

    [Display(Name = "Hesabımın ve tüm verilerimin kalıcı olarak silineceğini anlıyorum.")]
    public bool Onay { get; set; }
}

public class IlanFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur."), StringLength(100), Display(Name = "İlan başlığı")]
    public string Baslik { get; set; } = "";

    [Required(ErrorMessage = "Şehir zorunludur."), StringLength(50), Display(Name = "Şehir")]
    public string Sehir { get; set; } = "";

    [Required(ErrorMessage = "İlçe zorunludur."), StringLength(50), Display(Name = "İlçe")]
    public string Ilce { get; set; } = "";

    [Required(ErrorMessage = "Kira zorunludur."), Range(0, 200000, ErrorMessage = "Geçerli bir tutar gir.")]
    [Display(Name = "Aylık kira (TL)")]
    public int? Kira { get; set; }

    [Display(Name = "Oda tipi")]
    public OdaTipi OdaTipi { get; set; }

    [StringLength(2000), Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    [Display(Name = "Öğrenci olsun")]
    public bool SadeceOgrenci { get; set; } = true;

    [Display(Name = "Sigara içmesin")]
    public bool SigaraIcmeyen { get; set; }

    [Display(Name = "Sessiz olsun")]
    public bool Sessiz { get; set; }

    [Display(Name = "Evcil hayvan kabul")]
    public bool EvcilHayvanKabul { get; set; }

    [Display(Name = "İlan yayında")]
    public bool Aktif { get; set; } = true;

    [Display(Name = "Fotoğraf ekle")]
    public List<IFormFile>? YeniFotolar { get; set; }

    public List<int> SilinecekFotolar { get; set; } = new();
    public List<IlanFoto> MevcutFotolar { get; set; } = new();
}

// Ev arkadaşları listesi (/Kullanici). Alanlar adres çubuğundan gelir: /Kullanici?Sehir=Ankara&Uyku=Sabahci ...
// Mantık: gruplar arası VE, grup içi VEYA; boş bırakılan filtre uygulanmaz.
public class KullaniciFiltreViewModel
{
    public string? Sehir { get; set; }
    public int? ButceMin { get; set; }
    public int? ButceMax { get; set; }

    // Kişisel
    public int? YasMin { get; set; }
    public int? YasMax { get; set; }
    public List<Cinsiyet> Cinsiyet { get; set; } = new();
    public List<string> Statu { get; set; } = new();          // ogrenci, calisan
    public bool SadeceDogrulanmis { get; set; }

    // Eğitim
    public string? Universite { get; set; }
    public string? Q { get; set; }                             // bölüm / meslek

    // Yaşam
    public List<UykuDuzeni> Uyku { get; set; } = new();
    public List<CalismaDuzeni> Calisma { get; set; } = new();
    public List<EvcilHayvanDurumu> Hayvan { get; set; } = new();
    public List<string> Sigara { get; set; } = new();          // icmiyor, icer

    // Oda
    public List<AramaTuru> Tur { get; set; } = new();
    public List<TercihOdaTipi> Oda { get; set; } = new();

    public List<string> Hobi { get; set; } = new();

    public string? Siralama { get; set; }                      // yeni, uyum, butce-artan, butce-azalan, yas-artan
    public int Sayfa { get; set; } = 1;

    // Sonuçlar
    public List<KullaniciKartViewModel> Sonuclar { get; set; } = new();
    public int ToplamSayi { get; set; }
    public int ToplamSayfa { get; set; }
    public List<(string Sehir, int Sayi)> Sehirler { get; set; } = new();
    public List<string> Universiteler { get; set; } = new();
    public bool UyumHesaplanabilir { get; set; }               // giriş yapmış ve profili dolu mu
    public bool GirisYapti { get; set; }
}

public class KullaniciKartViewModel
{
    public ApplicationUser Kullanici { get; set; } = null!;
    public Profil Profil => Kullanici.Profil!;
    public int? Uyum { get; set; }
    public double? Puan { get; set; }
    public int PuanSayisi { get; set; }
}

public class KayitliAramaOzetViewModel
{
    public KayitliArama Arama { get; set; } = null!;
    public int Toplam { get; set; }
    public int Yeni { get; set; }
}

public class SikayetFormViewModel
{
    public static readonly string[] Nedenler =
    {
        "Sahte ilan veya profil",
        "Kapora / önden ödeme istiyor",
        "Yanıltıcı bilgi veya fotoğraf",
        "Uygunsuz içerik veya dil",
        "Taciz veya rahatsız edici mesaj",
        "Diğer"
    };

    public int? IlanId { get; set; }
    public string? KullaniciId { get; set; }

    [Required(ErrorMessage = "Bir şikayet nedeni seç."), StringLength(100), Display(Name = "Şikayet nedeni")]
    public string Neden { get; set; } = "";

    [StringLength(1000), Display(Name = "Açıklama (isteğe bağlı)")]
    public string? Aciklama { get; set; }

    // Formda gösterilecek bilgiler
    public string? IlanBaslik { get; set; }
    public string? KullaniciAd { get; set; }
}

public class KonusmaOzetViewModel
{
    public Konusma Konusma { get; set; } = null!;
    public Mesaj SonMesaj { get; set; } = null!;
    public int Okunmamis { get; set; }
}

// Filtreler (Sehir, KiraMax ...) IlanAramaKriteri'nden gelir: /Ilan?Sehir=Konya&KiraMax=6000 ...
public class HaritaNoktasi
{
    public string Sehir { get; set; } = "";
    public double Enlem { get; set; }
    public double Boylam { get; set; }
    public int Sayi { get; set; }
    public List<HaritaIlani> Ilanlar { get; set; } = new();
}

public record HaritaIlani(int Id, string Baslik, string Ilce, int Kira);

public class IlanAramaViewModel : RooMee.Services.IlanAramaKriteri
{
    public string? Siralama { get; set; }
    public string? Gorunum { get; set; }                       // "harita" ya da boş (liste)
    public List<HaritaNoktasi> HaritaNoktalari { get; set; } = new();
    public int HaritadaGosterilemeyen { get; set; }
    public int Sayfa { get; set; } = 1;

    // Sonuçlar
    public List<Ilan> Sonuclar { get; set; } = new();
    public int ToplamSayi { get; set; }
    public int ToplamSayfa { get; set; }
    public List<string> Sehirler { get; set; } = new();
}