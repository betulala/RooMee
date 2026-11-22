using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

public class Sikayet
{
    public int Id { get; set; }

    public string SikayetEdenId { get; set; } = "";
    public ApplicationUser? SikayetEden { get; set; }

    public int? IlanId { get; set; }
    public Ilan? Ilan { get; set; }

    public string? SikayetEdilenKullaniciId { get; set; }
    public ApplicationUser? SikayetEdilenKullanici { get; set; }

    [Required, StringLength(100), Display(Name = "Şikayet nedeni")]
    public string Neden { get; set; } = "";

    [StringLength(1000), Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    public SikayetDurumu Durum { get; set; } = SikayetDurumu.Beklemede;
    public DateTime Tarih { get; set; } = DateTime.UtcNow;
}