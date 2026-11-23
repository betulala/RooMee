using RooMee.Models;

namespace RooMee.Services;

// İlan arama kriterleri. Hem İlanlar sayfası hem kayıtlı aramalar aynı kuralları kullanır.
public class IlanAramaKriteri
{
    public string? Sehir { get; set; }
    public string? Ilce { get; set; }
    public int? KiraMin { get; set; }
    public int? KiraMax { get; set; }
    public OdaTipi? OdaTipi { get; set; }
    public bool SadeceOgrenci { get; set; }
    public bool SigaraIcmeyen { get; set; }
    public bool Sessiz { get; set; }
    public bool EvcilHayvanKabul { get; set; }
    public bool SadeceDogrulanmis { get; set; }

    public bool BosMu =>
        string.IsNullOrWhiteSpace(Sehir) && string.IsNullOrWhiteSpace(Ilce) && !KiraMin.HasValue && !KiraMax.HasValue &&
        !OdaTipi.HasValue && !SadeceOgrenci && !SigaraIcmeyen && !Sessiz && !EvcilHayvanKabul && !SadeceDogrulanmis;

    // Sadece kriter alanlarını kopyalar (sıralama, sayfa gibi alanlar hariç)
    public IlanAramaKriteri Kopya() => new()
    {
        Sehir = Sehir?.Trim(), Ilce = Ilce?.Trim(), KiraMin = KiraMin, KiraMax = KiraMax, OdaTipi = OdaTipi,
        SadeceOgrenci = SadeceOgrenci, SigaraIcmeyen = SigaraIcmeyen, Sessiz = Sessiz,
        EvcilHayvanKabul = EvcilHayvanKabul, SadeceDogrulanmis = SadeceDogrulanmis
    };

    // "Konya · Selçuklu · 5.000₺'ye kadar · Tek kişilik oda" gibi okunur özet
    public string Ozet()
    {
        var tr = new System.Globalization.CultureInfo("tr-TR");
        var p = new List<string>();
        if (!string.IsNullOrWhiteSpace(Sehir)) p.Add(Sehir.Trim());
        if (!string.IsNullOrWhiteSpace(Ilce)) p.Add(Ilce.Trim());
        if (KiraMin.HasValue && KiraMax.HasValue) p.Add($"{KiraMin.Value.ToString("N0", tr)}–{KiraMax.Value.ToString("N0", tr)}₺");
        else if (KiraMax.HasValue) p.Add($"{KiraMax.Value.ToString("N0", tr)}₺'ye kadar");
        else if (KiraMin.HasValue) p.Add($"{KiraMin.Value.ToString("N0", tr)}₺ ve üzeri");
        if (OdaTipi.HasValue) p.Add(OdaTipi.Value.GorunenAd());
        if (SadeceOgrenci) p.Add("öğrenci arayan");
        if (SigaraIcmeyen) p.Add("sigara içmeyen");
        if (Sessiz) p.Add("sessiz");
        if (EvcilHayvanKabul) p.Add("evcil hayvan kabul");
        if (SadeceDogrulanmis) p.Add("doğrulanmış öğrenci");
        return p.Count == 0 ? "Tüm ilanlar" : string.Join(" · ", p);
    }

    // İlanlar sayfası adresi için (/Ilan?Sehir=Konya&KiraMax=5000 ...)
    public Dictionary<string, string> RotaDegerleri()
    {
        var d = new Dictionary<string, string>();
        if (!string.IsNullOrWhiteSpace(Sehir)) d["Sehir"] = Sehir;
        if (!string.IsNullOrWhiteSpace(Ilce)) d["Ilce"] = Ilce;
        if (KiraMin.HasValue) d["KiraMin"] = KiraMin.Value.ToString();
        if (KiraMax.HasValue) d["KiraMax"] = KiraMax.Value.ToString();
        if (OdaTipi.HasValue) d["OdaTipi"] = OdaTipi.Value.ToString();
        if (SadeceOgrenci) d["SadeceOgrenci"] = "true";
        if (SigaraIcmeyen) d["SigaraIcmeyen"] = "true";
        if (Sessiz) d["Sessiz"] = "true";
        if (EvcilHayvanKabul) d["EvcilHayvanKabul"] = "true";
        if (SadeceDogrulanmis) d["SadeceDogrulanmis"] = "true";
        return d;
    }
}

public static class IlanFiltresi
{
    // Sadece yayındaki ilanlara uygulanmalı (çağıran taraf Aktif filtresini ekler)
    public static IQueryable<Ilan> Uygula(IQueryable<Ilan> q, IlanAramaKriteri f)
    {
        if (!string.IsNullOrWhiteSpace(f.Sehir))
        {
            var sehir = f.Sehir.Trim();
            q = q.Where(i => i.Sehir.Contains(sehir));
        }
        if (!string.IsNullOrWhiteSpace(f.Ilce))
        {
            var ilce = f.Ilce.Trim();
            q = q.Where(i => i.Ilce.Contains(ilce));
        }
        if (f.KiraMin.HasValue)
        {
            var min = f.KiraMin.Value;
            q = q.Where(i => i.Kira >= min);
        }
        if (f.KiraMax.HasValue)
        {
            var max = f.KiraMax.Value;
            q = q.Where(i => i.Kira <= max);
        }
        if (f.OdaTipi.HasValue)
        {
            var tip = f.OdaTipi.Value;
            q = q.Where(i => i.OdaTipi == tip);
        }
        if (f.SadeceOgrenci) q = q.Where(i => i.SadeceOgrenci);
        if (f.SigaraIcmeyen) q = q.Where(i => i.SigaraIcmeyen);
        if (f.Sessiz) q = q.Where(i => i.Sessiz);
        if (f.EvcilHayvanKabul) q = q.Where(i => i.EvcilHayvanKabul);
        if (f.SadeceDogrulanmis) q = q.Where(i => i.Kullanici!.OgrenciDogrulandi);
        return q;
    }
}
