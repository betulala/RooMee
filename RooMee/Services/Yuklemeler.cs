namespace RooMee.Services;

// Kullanıcıların yüklediği fotoğrafların (ilan, profil) diskteki yeri.
// - "Yuklemeler:Klasor" ayarı verilmişse orası kullanılır.
// - Azure App Service'te ayar yoksa kalıcı veri klasörü (%HOME%\data\uploads) kullanılır;
//   site klasörü (wwwroot) her yeni yayında baştan yazıldığı için fotoğraflar orada silinirdi.
// - Bilgisayarda ise eskisi gibi wwwroot/uploads.
// Adresler her durumda /uploads/... olarak kalır.
public class Yuklemeler
{
    public string Kok { get; }

    public Yuklemeler(IConfiguration ayarlar, IWebHostEnvironment env)
    {
        var ayar = ayarlar["Yuklemeler:Klasor"];
        var home = Environment.GetEnvironmentVariable("HOME");
        var azureMi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));

        Kok = !string.IsNullOrWhiteSpace(ayar) ? ayar
            : azureMi && !string.IsNullOrEmpty(home) ? Path.Combine(home, "data", "uploads")
            : Path.Combine(env.WebRootPath, "uploads");

        Directory.CreateDirectory(Kok);
    }

    // "ilan" -> .../uploads/ilan (yoksa oluşturur)
    public string Klasor(string alt)
    {
        var yol = Path.Combine(Kok, alt);
        Directory.CreateDirectory(yol);
        return yol;
    }

    // "/uploads/ilan/abc.jpg" -> diskteki tam yol (uploads dışına çıkılmasına izin vermez)
    public string? TamYol(string? adres)
    {
        if (string.IsNullOrEmpty(adres) || !adres.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase)) return null;
        var goreli = adres["/uploads/".Length..].Replace('/', Path.DirectorySeparatorChar);
        var tam = Path.GetFullPath(Path.Combine(Kok, goreli));
        return tam.StartsWith(Path.GetFullPath(Kok), StringComparison.OrdinalIgnoreCase) ? tam : null;
    }

    public void Sil(string? adres)
    {
        var tam = TamYol(adres);
        if (tam != null && File.Exists(tam)) File.Delete(tam);
    }
}
