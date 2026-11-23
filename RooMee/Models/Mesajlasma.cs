using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

// İki kullanıcı arasındaki tek konuşma.
// Kullanici1Id her zaman alfabetik olarak küçük olan id'dir, böylece aynı ikili için tek kayıt olur.
public class Konusma
{
    public int Id { get; set; }

    public string Kullanici1Id { get; set; } = "";
    public ApplicationUser? Kullanici1 { get; set; }

    public string Kullanici2Id { get; set; } = "";
    public ApplicationUser? Kullanici2 { get; set; }

    // Konuşmanın başladığı ilan (ilan silinirse boş kalır)
    public int? IlanId { get; set; }
    public Ilan? Ilan { get; set; }

    public DateTime OlusturmaTarihi { get; set; } = DateTime.UtcNow;
    public DateTime SonMesajTarihi { get; set; } = DateTime.UtcNow;

    public List<Mesaj> Mesajlar { get; set; } = new();

    public string KarsiTarafId(string benimId) => Kullanici1Id == benimId ? Kullanici2Id : Kullanici1Id;
    public ApplicationUser? KarsiTaraf(string benimId) => Kullanici1Id == benimId ? Kullanici2 : Kullanici1;

    public static (string, string) Sirala(string a, string b) =>
        string.CompareOrdinal(a, b) < 0 ? (a, b) : (b, a);
}

public class Mesaj
{
    public int Id { get; set; }

    public int KonusmaId { get; set; }
    public Konusma? Konusma { get; set; }

    public string GonderenId { get; set; } = "";
    public ApplicationUser? Gonderen { get; set; }

    [Required, StringLength(1000)]
    public string Metin { get; set; } = "";

    public DateTime Tarih { get; set; } = DateTime.UtcNow;
    public bool Okundu { get; set; }
}
