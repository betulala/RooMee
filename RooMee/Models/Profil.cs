using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RooMee.Models;

public class Profil
{
    public int Id { get; set; }

    [Required]
    public string KullaniciId { get; set; } = "";
    public ApplicationUser? Kullanici { get; set; }

    // ---------- Kişisel ----------
    [Display(Name = "Cinsiyet")]
    public Cinsiyet? Cinsiyet { get; set; }

    [Range(1950, 2010), Display(Name = "Doğum yılı")]
    public int? DogumYili { get; set; }

    [NotMapped]
    public int? Yas => DogumYili.HasValue ? DateTime.Today.Year - DogumYili.Value : null;

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

    // ---------- Eğitim ----------
    [StringLength(100), Display(Name = "Üniversite")]
    public string? Universite { get; set; }

    [StringLength(100), Display(Name = "Bölüm")]
    public string? Bolum { get; set; }

    [StringLength(20), Display(Name = "Sınıf")]
    public string? Sinif { get; set; }

    // ---------- Oda ve bütçe ----------
    // Boşsa profil "Ev arkadaşları" listesinde görünmez
    [Display(Name = "Listede nasıl görüneyim?")]
    public AramaTuru? AramaTuru { get; set; }

    [Display(Name = "Oda tipi tercihi")]
    public TercihOdaTipi? TercihOdaTipi { get; set; }

    [StringLength(10), Display(Name = "Ev tipi (örn. 2+1)")]
    public string? EvTipi { get; set; }

    [Range(1, 10), Display(Name = "Boş oda sayısı")]
    public int? BosOda { get; set; }

    [Range(0, 100000), Display(Name = "En az bütçe (TL)")]
    public int? ButceMin { get; set; }

    [Range(0, 100000), Display(Name = "En fazla bütçe (TL)")]
    public int? ButceMax { get; set; }

    // ---------- Yaşam tarzı ----------
    [Display(Name = "Yatış saati")]
    public TimeOnly? YatisSaati { get; set; }

    [Display(Name = "Kalkış saati")]
    public TimeOnly? KalkisSaati { get; set; }

    [Display(Name = "Uyku düzeni")]
    public UykuDuzeni? Uyku { get; set; }

    [Display(Name = "Ders / çalışma düzeni")]
    public CalismaDuzeni? CalismaDuzeni { get; set; }

    [Display(Name = "Evcil hayvan")]
    public EvcilHayvanDurumu? EvcilHayvan { get; set; }

    [Display(Name = "Sigara içiyorum")]
    public bool SigaraIciyor { get; set; }

    // "Spor;Müzik" biçiminde (bkz. Hobiler sınıfı)
    [StringLength(300), Display(Name = "Hobiler")]
    public string? HobilerMetni { get; set; }

    [StringLength(1000), Display(Name = "Hakkımda")]
    public string? Hakkinda { get; set; }

    public string? FotoYolu { get; set; }
    public DateTime GuncellemeTarihi { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public List<string> HobiListesi => Models.Hobiler.Ayir(HobilerMetni);

    // Kartlarda gösterilecek bütçe: varsa en fazla, yoksa en az
    [NotMapped]
    public int? Butce => ButceMax ?? ButceMin;
}
