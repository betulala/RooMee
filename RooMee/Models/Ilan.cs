using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

public class Ilan
{
    public int Id { get; set; }

    [Required]
    public string KullaniciId { get; set; } = "";
    public ApplicationUser? Kullanici { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur."), StringLength(100), Display(Name = "İlan başlığı")]
    public string Baslik { get; set; } = "";

    [Required(ErrorMessage = "Şehir zorunludur."), StringLength(50), Display(Name = "Şehir")]
    public string Sehir { get; set; } = "";

    [Required(ErrorMessage = "İlçe zorunludur."), StringLength(50), Display(Name = "İlçe")]
    public string Ilce { get; set; } = "";

    [Range(0, 200000), Display(Name = "Aylık kira (TL)")]
    public int Kira { get; set; }

    [Display(Name = "Oda tipi")]
    public OdaTipi OdaTipi { get; set; }

    [StringLength(2000), Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }

    // Aranan ev arkadaşı kriterleri
    [Display(Name = "Öğrenci olsun")]
    public bool SadeceOgrenci { get; set; } = true;

    [Display(Name = "Sigara içmesin")]
    public bool SigaraIcmeyen { get; set; }

    [Display(Name = "Sessiz olsun")]
    public bool Sessiz { get; set; }

    [Display(Name = "Evcil hayvan kabul")]
    public bool EvcilHayvanKabul { get; set; }

    public bool Aktif { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    public List<IlanFoto> Fotolar { get; set; } = new();
}