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

    [Display(Name = "Evcil hayvanım var")]
    public bool EvcilHayvanVar { get; set; }

    [Display(Name = "Sessiz ortam isterim")]
    public bool SessizOrtamIster { get; set; }

    [StringLength(1000), Display(Name = "Hakkımda")]
    public string? Hakkinda { get; set; }

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