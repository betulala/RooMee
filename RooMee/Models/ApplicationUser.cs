using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RooMee.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(50)]
    public string Ad { get; set; } = "";

    [Required, StringLength(50)]
    public string Soyad { get; set; } = "";

    public DateTime KvkkOnayTarihi { get; set; }
    public DateTime KayitTarihi { get; set; } = DateTime.UtcNow;
    // Öğrenci doğrulama (profilden yapılır)
    [StringLength(256)]
    public string? OgrenciEposta { get; set; }
    public bool OgrenciDogrulandi { get; set; }
    public DateTime? OgrenciDogrulamaTarihi { get; set; }
    // Yeni mesaj, kayıtlı arama uyarısı gibi e-posta bildirimleri
    public bool EpostaBildirim { get; set; } = true;

    [NotMapped]
    public string KisaAd => string.IsNullOrEmpty(Soyad) ? Ad : $"{Ad} {Soyad[0]}.";

    public Profil? Profil { get; set; }
    public List<Ilan> Ilanlar { get; set; } = new();
}