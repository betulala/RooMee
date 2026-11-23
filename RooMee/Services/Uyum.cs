using RooMee.Models;

namespace RooMee.Services;

public record UyumSatiri(string Ad, int Agirlik, double Puan, string? Not)
{
    public string Durum => Puan >= 0.8 ? "Uyumlu" : Puan >= 0.4 ? "Kısmen" : "Uyumsuz";
}

public record UyumSonucu(int Skor, List<UyumSatiri> Satirlar);

// İki profil arasındaki ev arkadaşı uyumunu hesaplar.
// Ağırlıklar: uyku 15, ders/çalışma 10, evcil hayvan 10, sigara 15, bütçe 20, yaş 10, ortak hobiler 20.
// İki profilde de dolu olmayan kriterler hesaba katılmaz.
public static class Uyum
{
    public static UyumSonucu? Hesapla(Profil? ben, Profil? o)
    {
        if (ben == null || o == null) return null;
        var satirlar = new List<UyumSatiri>();

        if (ben.Uyku.HasValue && o.Uyku.HasValue)
            satirlar.Add(new("Uyku düzeni", 15, Esnek(ben.Uyku == o.Uyku, ben.Uyku == UykuDuzeni.Esnek || o.Uyku == UykuDuzeni.Esnek), null));

        if (ben.CalismaDuzeni.HasValue && o.CalismaDuzeni.HasValue)
            satirlar.Add(new("Ders / çalışma düzeni", 10,
                Esnek(ben.CalismaDuzeni == o.CalismaDuzeni, ben.CalismaDuzeni == CalismaDuzeni.Esnek || o.CalismaDuzeni == CalismaDuzeni.Esnek), null));

        if (ben.EvcilHayvan.HasValue && o.EvcilHayvan.HasValue)
            satirlar.Add(new("Evcil hayvan", 10, Hayvan(ben.EvcilHayvan.Value, o.EvcilHayvan.Value), null));

        satirlar.Add(new("Sigara", 15, ben.SigaraIciyor == o.SigaraIciyor ? 1 : 0.2, null));

        if (ben.Butce is int b1 && o.Butce is int b2 && b1 > 0)
            satirlar.Add(new("Bütçe", 20, Sinirla(1 - Math.Abs(b2 - b1) / (double)b1), null));

        if (ben.Yas is int y1 && o.Yas is int y2)
            satirlar.Add(new("Yaş", 10, Sinirla(1 - Math.Abs(y2 - y1) / 10.0), null));

        var benimHobiler = ben.HobiListesi;
        var onunHobiler = o.HobiListesi;
        if (benimHobiler.Count > 0 && onunHobiler.Count > 0)
        {
            var ortak = onunHobiler.Where(benimHobiler.Contains).ToList();
            satirlar.Add(new("Ortak hobiler", 20, Sinirla(ortak.Count / 2.0), ortak.Count > 0 ? string.Join(", ", ortak) : null));
        }

        // Sadece sigara karşılaştırılabiliyorsa yüzde anlamsız olur
        if (satirlar.Count < 3) return null;

        var toplamAgirlik = satirlar.Sum(s => s.Agirlik);
        var skor = (int)Math.Round(satirlar.Sum(s => s.Agirlik * s.Puan) / toplamAgirlik * 100);
        return new UyumSonucu(skor, satirlar);
    }

    private static double Esnek(bool ayni, bool biriEsnek) => ayni ? 1 : biriEsnek ? 0.6 : 0;

    private static double Hayvan(EvcilHayvanDurumu a, EvcilHayvanDurumu b)
    {
        if (a == b) return 1;
        var ikisi = new[] { a, b };
        if (ikisi.Contains(EvcilHayvanDurumu.Istemiyor) && ikisi.Contains(EvcilHayvanDurumu.Sahibi)) return 0;
        if (ikisi.Contains(EvcilHayvanDurumu.Istemiyor)) return 0.6; // istemiyor + sorun değil
        return 1; // sahibi + sorun değil
    }

    private static double Sinirla(double x) => Math.Max(0, Math.Min(1, x));
}
