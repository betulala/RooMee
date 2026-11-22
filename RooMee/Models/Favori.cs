namespace RooMee.Models;

public class Favori
{
    public int Id { get; set; }
    public string KullaniciId { get; set; } = "";
    public ApplicationUser? Kullanici { get; set; }
    public int IlanId { get; set; }
    public Ilan? Ilan { get; set; }
    public DateTime Tarih { get; set; } = DateTime.UtcNow;
}