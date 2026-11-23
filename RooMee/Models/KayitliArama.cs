using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using RooMee.Services;

namespace RooMee.Models;

// Kullanıcının kaydettiği ilan araması. Uyan yeni ilan eklenince e-posta ile haber verilir.
public class KayitliArama
{
    public int Id { get; set; }

    public string KullaniciId { get; set; } = "";
    public ApplicationUser? Kullanici { get; set; }

    [Required, StringLength(80)]
    public string Ad { get; set; } = "";

    // Arama kriterleri JSON olarak (IlanAramaKriteri)
    [Required, StringLength(1000)]
    public string KriterJson { get; set; } = "{}";

    public bool EpostaUyarisi { get; set; } = true;

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;

    // Kullanıcının sonuçlara en son baktığı an; "yeni ilan" sayısı buna göre hesaplanır
    public DateTime SonBakis { get; set; } = DateTime.UtcNow;

    public IlanAramaKriteri Kriter
    {
        get => JsonSerializer.Deserialize<IlanAramaKriteri>(KriterJson) ?? new IlanAramaKriteri();
        set => KriterJson = JsonSerializer.Serialize(value.Kopya());
    }
}
