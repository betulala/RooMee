using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

public class Profil
{
    public int Id { get; set; }

    [Required]
    public string KullaniciId { get; set; } = "";
    public ApplicationUser? Kullanici { get; set; }

    [StringLength(100), Display(Name = "Üniversite")]
    public string? Universite { get; set; }

    [StringLength(100), Display(Name = "Bölüm")]
    public string? Bolum { get; set; }

    [Range(0, 100000), Display(Name = "En az bütçe (TL)")]
    public int? ButceMin { get; set; }

    [Range(0, 100000), Display(Name = "En fazla bütçe (TL)")]
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

    public string? FotoYolu { get; set; }
    public DateTime GuncellemeTarihi { get; set; } = DateTime.UtcNow;
}