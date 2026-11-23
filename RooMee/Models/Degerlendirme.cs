using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

// Bir kullanıcının ev arkadaşı (ya da adayı) hakkındaki puanı ve yorumu.
// Bir kişi, bir başkasını sadece bir kez değerlendirebilir (sonradan güncelleyebilir).
public class Degerlendirme
{
    public int Id { get; set; }

    public string YazanId { get; set; } = "";
    public ApplicationUser? Yazan { get; set; }

    public string HakkindaId { get; set; } = "";
    public ApplicationUser? Hakkinda { get; set; }

    [Range(1, 5)]
    public int Puan { get; set; }

    [StringLength(500)]
    public string? Yorum { get; set; }

    public DateTime Tarih { get; set; } = DateTime.UtcNow;
}
