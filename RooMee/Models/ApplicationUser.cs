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

    // Yönetici tarafından askıya alınan hesap giriş yapamaz; ilanları ve profili gizlenir
    public bool AskiyaAlindi { get; set; }
    public DateTime? AskiyaAlinmaTarihi { get; set; }
    [StringLength(200)]
    public string? AskiyaAlmaNedeni { get; set; }

    [NotMapped]
    public string KisaAd => string.IsNullOrEmpty(Soyad) ? Ad : $"{Ad} {Soyad[0]}.";

    public Profil? Profil { get; set; }
    public List<Ilan> Ilanlar { get; set; } = new();
}