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

    public Profil? Profil { get; set; }
    public List<Ilan> Ilanlar { get; set; } = new();
}