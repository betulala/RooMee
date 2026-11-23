using System.ComponentModel.DataAnnotations;

namespace RooMee.Models;

public enum OdaTipi
{
    [Display(Name = "Tek kişilik oda")] TekKisilik,
    [Display(Name = "Ortak oda")] OrtakOda,
    [Display(Name = "Ev bütünü")] EvButunu
}

public enum SikayetDurumu
{
    [Display(Name = "Beklemede")] Beklemede,
    [Display(Name = "İncelendi")] Incelendi,
    [Display(Name = "Reddedildi")] Reddedildi
}

// ---------- Ev arkadaşı profili ----------

public enum Cinsiyet
{
    [Display(Name = "Kadın")] Kadin,
    [Display(Name = "Erkek")] Erkek
}

// Profilin ev arkadaşı listesinde hangi amaçla görüneceği
public enum AramaTuru
{
    [Display(Name = "Ev arkadaşı arıyor")] EvArkadasiAriyor,
    [Display(Name = "Kiralık ev ilanı")] KiralikEv
}

public enum TercihOdaTipi
{
    [Display(Name = "Paylaşımlı oda")] Paylasimli,
    [Display(Name = "Tek kişilik oda")] TekKisilik,
    [Display(Name = "Stüdyo daire")] Studyo
}

public enum UykuDuzeni
{
    [Display(Name = "Sabahçı")] Sabahci,
    [Display(Name = "Gececi")] Gececi,
    [Display(Name = "Uyku düzeni esnek")] Esnek
}

public enum CalismaDuzeni
{
    [Display(Name = "Evde sessiz ortam")] EvdeSessiz,
    [Display(Name = "Dışarıda çalışır")] Disarida,
    [Display(Name = "Çalışma düzeni esnek")] Esnek
}

public enum EvcilHayvanDurumu
{
    [Display(Name = "Evcil hayvanı var")] Sahibi,
    [Display(Name = "Evcil hayvan dostu")] SorunDegil,
    [Display(Name = "Evcil hayvan istemiyor")] Istemiyor
}

public static class Hobiler
{
    public static readonly string[] Liste =
    {
        "Spor", "Müzik", "Kitap okuma", "Oyun", "Yemek yapma", "Film / Dizi",
        "Seyahat", "Doğa yürüyüşü", "Fotoğrafçılık", "Teknoloji"
    };

    // Veritabanında "Spor;Müzik" şeklinde tutulur
    public static List<string> Ayir(string? deger) =>
        string.IsNullOrWhiteSpace(deger)
            ? new List<string>()
            : deger.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    public static string? Birlestir(IEnumerable<string>? hobiler)
    {
        var gecerli = (hobiler ?? Enumerable.Empty<string>()).Where(h => Liste.Contains(h)).Distinct().ToList();
        return gecerli.Count == 0 ? null : string.Join(";", gecerli);
    }
}
