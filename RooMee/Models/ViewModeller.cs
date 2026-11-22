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